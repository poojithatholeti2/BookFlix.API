using BookFlix.API.Models.Domain;

namespace BookFlix.API.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
