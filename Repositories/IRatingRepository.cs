using BookFlix.API.Models.Domain;

namespace BookFlix.API.Repositories
{
    public interface IRatingRepository
    {
        Task<List<Rating>> GetAllAsync(string? filterOn = null, int? filterQuery = 0,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Rating?> GetByIdAsync(Guid id);
        Task<Rating> CreateAsync(Rating rating);
        Task<Rating?> UpdateAsync(Guid id, Rating rating);
        Task<Rating?> DeleteAsync(Guid id);
    }
}
