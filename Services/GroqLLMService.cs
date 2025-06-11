using System.Text.Json.Nodes;
using BookFlix.API.Services.Interfaces;
using GroqApiLibrary;

namespace BookFlix.API.Services
{
    public class GroqLLMService : ILLMService
    {
        private readonly GroqApiClient _groqApiClient;
        private readonly string _llmModel = Environment.GetEnvironmentVariable("GroqLLMModel");
        public GroqLLMService(GroqApiClient groqApiClient)
        {
            _groqApiClient = groqApiClient;
        }
        public async Task<string> GenerateRecommendationAsync(string systemPrompt, string query, string availableBooks)
        {
            var request = new JsonObject
            {
                ["model"] = _llmModel,
                ["temperature"] = 0.2,
                ["max_tokens"] = 150,
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "system",
                        ["content"] = systemPrompt
                    },
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $"User Query: {query} \nAvailable Books: {availableBooks}"
                    }
                }
            };

            var result = await _groqApiClient.CreateChatCompletionAsync(request);
            string? recommendedBooks = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            if (recommendedBooks == null)
                throw new Exception("Failed to generate recommendation.");

            return recommendedBooks;
        }
    }
}
