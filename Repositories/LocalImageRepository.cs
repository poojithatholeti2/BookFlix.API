using BookFlix.API.Data;
using BookFlix.API.Models.Domain;

namespace BookFlix.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly BookFlixDbContext dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, BookFlixDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<Image> Upload(Image image)
        {
            //making a local file path for image to go to our local folder
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");

            //upload image to local folder
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            //we need the image to be displayed as a URL so we need to make a url so the below type to access these images
            // https://localhost:1234/images/image.jpg
            //https --> scheme --> httpContextAccessor.HttpContext.Request.Scheme
            //localhost: --> host --> httpContextAccessor.HttpContext.Request.Host
            //1234 --> pathbase --> httpContextAccessor.HttpContext.Request.PathBase
            //Images --> images folder
            //image --> image.FileName, .jpg --> image.FileExtension
            var urlPath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlPath;

            //saving these changes to db
            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }
    }
}
