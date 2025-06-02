namespace BookFlix.API.Services.Interfaces
{
    public interface IEmbeddingQueue
    {
        void Enqueue(Guid bookId, String embeddingText);
    }
}
