using BookFlix.API.Models.Domain;

namespace BookFlix.API.Services.Interfaces
{
    public interface IBookService
    {
        Task<Book?> CreateAsync(Book book);
        Task<List<Book>> CreateMultipleAsync(List<Book> books);
        Task<Book?> UpdateAsync(Guid id, Book book);
    }
}
