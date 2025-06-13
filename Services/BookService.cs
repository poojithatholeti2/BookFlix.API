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

            saveEmbedding(createdBook);
            return createdBook;
        }

        public async Task<List<Book>> CreateMultipleAsync(List<Book> books)
        {
            var createdBooks = new List<Book>();
            foreach (var book in books)
            {
                var currentBook = await CreateAsync(book);
                if (currentBook == null)
                {
                    return createdBooks;
                }
                createdBooks.Add(currentBook);
            }
            return createdBooks;
        }

        public async Task<Book?> UpdateAsync(Guid id, Book book)
        {
            var updatedBook = await _bookRepository.UpdateAsync(id, book);

            if(updatedBook == null)
            {
                return null;
            }

            saveEmbedding(updatedBook);
            return updatedBook;
        }

        private void saveEmbedding(Book book)
        {
            string embeddingText;
            bool hasDescription = string.IsNullOrWhiteSpace(book.Description);

            if (hasDescription)
            {
                embeddingText = $"{book.Title} is a book, " +
                    $"with the following {book.Description} " +
                    $"written by {book.Author} " +
                    $"priced at {book.Price}, " +
                    $"of genre {book.Category.Title} " +
                    $"with a rating {book.Rating.RatingName}";
            }
            else
            {
                embeddingText = $"{book.Title} is a book, " +
                    $"written by {book.Author} " +
                    $"priced at {book.Price}, " +
                    $"of genre {book.Category.Title} " +
                    $"with a rating {book.Rating.RatingName}";
            }

            //embeddingqueue call
            _embeddingQueue.Enqueue(book.Id, embeddingText);
        }
    }
}
