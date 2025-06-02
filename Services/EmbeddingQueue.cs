using System.Threading.Channels;
using BookFlix.API.Services.Interfaces;
using BookFlix.API.Repositories.Interfaces;

namespace BookFlix.API.Services
{
    public class EmbeddingQueue : BackgroundService, IEmbeddingQueue
    {
        private readonly Channel<(Guid bookId, string embeddingText)> _queue = Channel.CreateUnbounded<(Guid, string)> ();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmbeddingQueue> _logger;

        public EmbeddingQueue(IServiceProvider serviceProvider, ILogger<EmbeddingQueue> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Enqueue(Guid bookId, string embeddingText)
        {
            _queue.Writer.TryWrite((bookId, embeddingText));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(var (bookId, text) in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    //'using' destroys scope when control goes beyond this block
                    //prevents memory leaks
                    using var scope = _serviceProvider.CreateScope();
                    var bookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
                    var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

                    var embeddingResult = await embeddingService.GetEmbeddingAsync(text);
                    await bookRepository.SaveEmbeddingAsync(bookId, embeddingResult);
                }
                catch (Exception ex)
                {
                    // Log manually since background services are not covered by HTTP pipeline
                    _logger.LogError(ex, $"Embedding failed for Book: {bookId}");
                }
            }
        }
    }
}
