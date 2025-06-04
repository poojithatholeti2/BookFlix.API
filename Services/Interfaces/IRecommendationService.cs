using BookFlix.API.Models.DTO;

namespace BookFlix.API.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<RecommendationsDto> GetRecommendationAsync(string query);
    }
}
