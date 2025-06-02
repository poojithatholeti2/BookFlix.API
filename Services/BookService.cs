using BookFlix.API.Models.Domain;
using BookFlix.API.Repositories.Interfaces;
using BookFlix.API.Services.Interfaces;

namespace BookFlix.API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IEmbeddingQueue _embeddingQueue;

        public BookService(IBookRepository bookRepository, IEmbeddingQueue embeddingQueue)
        {
            _bookRepository = bookRepository;
            _embeddingQueue = embeddingQueue;
        }

        public async Task<Book?> CreateAsync(Book book)
        {
            var createdBook = await _bookRepository.CreateAsync(book);
            if (createdBook == null)
            {
                return null;
            }

            var text = $"{createdBook.Title} is a book, " +
                $"with the following {createdBook.Description} " +
                $"written by {createdBook.Author} " +
                $"priced at {createdBook.Price}, " +
                $"of genre {createdBook.Category.Title} " +
                $"with a rating {createdBook.Rating.RatingName}";

            //embeddingqueue call
            _embeddingQueue.Enqueue(createdBook.Id, text);

            return createdBook;
        }

        public async Task<List<Book>?> CreateMultipleAsync(List<Book> books)
        {
            var createdBooks = new List<Book>();
            foreach (var book in books)
            {
                var currentBook = await CreateAsync(book);
                if (currentBook == null) return null;
                createdBooks.Add(currentBook);
            }
            return createdBooks;
        }
    }
}
