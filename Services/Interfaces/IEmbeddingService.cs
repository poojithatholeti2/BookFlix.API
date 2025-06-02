using Pgvector;

namespace BookFlix.API.Services.Interfaces
{
    public interface IEmbeddingService
    {
        Task<Vector> GetEmbeddingAsync(String text);
    }
}
