using BookFlix.API.Models.Domain;

namespace BookFlix.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<Category?> DeleteAsync(Guid id);
    }
}
