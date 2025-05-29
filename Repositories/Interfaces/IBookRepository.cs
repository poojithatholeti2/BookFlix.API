using BookFlix.API.Models.Domain;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookFlix.API.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> CreateAsync(Book book);
        Task<List<Book>?> CreateMultipleAsync(List<Book> books);
        Task<List<Book>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize=1000);
        Task<Book?> GetByIdAsync(Guid id);
        Task<Book?> UpdateAsync(Guid id, Book book);
        Task<Book?> DeleteAsync(Guid id);
    }
}
