using BookFlix.API.Models.Domain;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pgvector;

namespace BookFlix.API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> CreateAsync(Book book);
        Task<List<Book>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Book?> GetByIdAsync(Guid id);
        Task<Book?> UpdateAsync(Guid id, Book book);
        Task<Book?> DeleteAsync(Guid id);
        Task SaveEmbeddingAsync(Guid id, Vector embedding);
        Task<List<Book>?> GetSimilarBooksAsync(Vector inputVector);
    }
}
