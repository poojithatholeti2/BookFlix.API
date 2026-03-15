using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        public async Task<RecommendationsDto> GetRecommendationAsync(RecommendationQueryDto queryDTO)
        {
            //Embedding for the given query
            var embeddingVector = await _embeddingService.GetEmbeddingAsync(queryDTO.Query);

            //Get similar books from our db for the provided query
            var similarBooks = await _bookRepository.GetSimilarBooksAsync(embeddingVector);

            var compactBooks = new List<CompactBookDto>();
            foreach(var book in similarBooks)
            {
                compactBooks.Add(new CompactBookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = book.Author,
                    Price = book.Price,
                    CategoryName = book.Category.Title
                });
            }

            //serialize for LLM input
            var availableBooksJson = JsonSerializer.Serialize(compactBooks);

            //Get LLM recommendation result (comma-separated string of GUIDs)
            var systemPrompt = """
                You are an expert book recommendation engine. Your sole task is to select AT MOST 2 books from a provided JSON list of candidate books that best match the user's query.

                Evaluate each candidate book against the user's query using the following criteria:

                1. Author (HARD FILTER when specified): If the user explicitly mentions an author name, ONLY consider books whose "Author" field matches or closely resembles that name. Do NOT recommend books by a different author in this case, even if other fields seem relevant.
                2. Title: Does the title directly reflect or relate to the user's query topic, genre, or intent?
                3. Description: Does the description closely match the themes, concepts, or subject matter the user is asking about?
                4. Price: If the user specifies a budget or price range, prefer books whose price falls within or closest to that range.
                5. CategoryName: The category should be relevant to the query. Do not recommend books from a completely unrelated category.

                Selection rules:
                - If the user specifies an author, treat the Author field as a hard filter — a strong single-signal query like "books by [author]" is sufficient to recommend a book if there is a match.
                - For general queries (no author specified), select books that match best across Title, Description, and CategoryName.
                - If the user specifies a price, pick the closest match within the candidate list.
                - If no candidate book satisfies the user's query well enough, return an empty string.
                - Return AT MOST 2 book IDs — never more.
                - Output ONLY a comma-separated string of the exact GUIDs from the input JSON, with NO spaces, NO extra text, NO explanation before or after the GUIDs.
                - Do NOT invent, modify, or combine GUIDs — copy them exactly as they appear in the input JSON.

                <think> tag rules:
                - Inside <think>, reason as a knowledgeable book expert speaking directly to the user.
                - For each candidate book, state how well it matches the query on Author, Title, Description, Price, and CategoryName.
                - Clearly justify why each book was selected or rejected.

                FINAL OUTPUT FORMAT (outside <think>): comma-separated GUIDs only — e.g. "guid1,guid2" or "guid1" or "". Absolutely nothing else.
                """;

            var llmResult = await _llmService.GenerateRecommendationAsync(systemPrompt, queryDTO.Query, availableBooksJson);

            // Process the LLM output
            // 1. Extract <think>...</think> data (if present)
            var thinkStart = "<think>";
            var thinkEnd = "</think>";
            var thinkStartIndex = llmResult.IndexOf(thinkStart, StringComparison.OrdinalIgnoreCase);
            var thinkEndIndex = llmResult.IndexOf(thinkEnd, StringComparison.OrdinalIgnoreCase);

            string? think = null;
            string? content = null;

            if (thinkStartIndex != -1 && thinkEndIndex != -1 && thinkEndIndex > thinkStartIndex)
            {
                int contentStart = thinkStartIndex + thinkStart.Length;
                think = llmResult.Substring(contentStart, thinkEndIndex - contentStart).Trim();

                content = llmResult.Substring(thinkEndIndex + thinkEnd.Length);
            }
            else
            {
                content = llmResult.Trim();
            }

            if (string.IsNullOrWhiteSpace(think) || string.IsNullOrWhiteSpace(content))
            {
                throw new Exception("LLM response is not in the expected format.");
            }

            // Parse the comma-separated string of GUIDs
            var resultRecommendation = content
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
                Books = recommendedBooks,
                Explanation = (queryDTO.IsExplanationNeeded) ? think : null
            };
        }
    }
}
