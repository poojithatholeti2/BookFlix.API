using BookFlix.API.Models.Domain;

namespace BookFlix.API.Services.Interfaces
{
    public interface ILLMService
    {
        Task<string> GenerateRecommendationAsync(string systemPrompt, string query, string availableBooks);
    }
}
