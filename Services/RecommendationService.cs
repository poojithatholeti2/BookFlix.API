using System.Text.Json;
using AutoMapper;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories.Interfaces;
using BookFlix.API.Services.Interfaces;

namespace BookFlix.API.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IBookRepository _bookRepository;
        private readonly ILLMService _llmService;
        private readonly IMapper _mapper;
        public RecommendationService(IEmbeddingService embeddingService, IBookRepository bookRepository, ILLMService llmService, IMapper mapper)
        {
            _embeddingService = embeddingService;
            _bookRepository = bookRepository;
            _llmService = llmService;
            _mapper = mapper;
        }

        public async Task<RecommendationsDto> GetRecommendationAsync(string query)
        {
            //get embedding for the given query
            var embeddingVector = await _embeddingService.GetEmbeddingAsync(query);

            //get similar books from our db for the provided query
            var similarBooks = await _bookRepository.GetSimilarBooksAsync(embeddingVector);

            var compactBooks = new List<CompactBookDto>();
            foreach(var book in similarBooks)
            {
                compactBooks.Add(new CompactBookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Price = book.Price,
                    CategoryName = book.Category.Title
                });
            }

            //serialize for LLM input
            var availableBooksJson = JsonSerializer.Serialize(compactBooks);

            //Get LLM recommendation result (comma-separated string of GUIDs)
            var systemPrompt = """
                Imagine you are a book recommender which recommends AT MOST 2 books only based on user query, among 5 books given to you in a JSON format (input Books).

                Your task:
                - User query is given in user section and you have to select books according to this query.
                - You will take 5 books in a JSON format, you must return a comma separated string with no space between the Guids of AT MOST 2 book ids.
                - You must not combine guids and only return the Guids that are provided in the input Books.
                - Output must be a comma separated string of Guids with no space in between and with at most 2 book ids. No text before or after.
                - If no valid books are found or recommended based on the input books or query, return an empty string.
                - Emphasize more on the CategoryName. Only if the CategoryName is similar to the query, then recommend that book.
                - If you are not confident enough that any of the available books don't cover all the requirements from the user query, then don't recommend anything. 
                - Give me a total of 2 books only that fit best for the user query.

                Do NOT include any explanations or additional formatting. Return only the comma separated string of Guids.
                """;

            var llmResult = await _llmService.GenerateRecommendationAsync(systemPrompt, query, availableBooksJson);

            // Parse the comma-separated string of GUIDs
            var resultRecommendation = llmResult
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(guidStr => Guid.TryParse(guidStr, out var guid) ? guid : (Guid?)null)
                .Where(guid => guid.HasValue)
                .Select(guid => guid.Value)
                .ToList();

            var recommendedBooks = new List<RecommendedBookDto>();
            foreach(var book in similarBooks)
            {
                if (resultRecommendation.Contains(book.Id))
                    recommendedBooks.Add(new RecommendedBookDto
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Description = book.Description,
                        Author = book.Author,
                        Price = book.Price,
                        CategoryName = book.Category.Title,
                        RatingName = book.Rating.RatingName
                    });
            }

            var message = (recommendedBooks.Count() == 0) 
                ? "Sorry, we currently cannot recommend any Book for your query. Please make sure that the query is related to Book recommendation and try to be specific."
                : "Here are the recommended Books based on your query among our available Books.";

            return new RecommendationsDto
            {
                Message = message,
                Books = recommendedBooks
            };
        }
    }
}
