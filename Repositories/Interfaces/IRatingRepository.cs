using BookFlix.API.Models.Domain;

namespace BookFlix.API.Repositories
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<Rating?> GetByIdAsync(Guid id);
        Task<Rating> CreateAsync(Rating rating);
        Task<Rating?> UpdateAsync(Guid id, Rating rating);
        Task<Rating?> DeleteAsync(Guid id);
    }
}
