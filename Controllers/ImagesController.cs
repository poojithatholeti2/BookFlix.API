using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookFlix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }


        //Post: api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            Validate(imageUploadRequestDto);

            if (ModelState.IsValid)
            {
                //convert dto to domain model
                var imageDomainModel = new Image
                {
                    File = imageUploadRequestDto.File,
                    FileName = imageUploadRequestDto.FileName,
                    FileDescription = imageUploadRequestDto.FileDescription,
                    FileExtension = Path.GetExtension(imageUploadRequestDto.File.FileName),
                    FileSizeInBytes = imageUploadRequestDto.File.Length
                };

                //use repository to upload image
                await imageRepository.Upload(imageDomainModel);

                return Ok(imageDomainModel);
            }

            return BadRequest(ModelState);
        }

        private void Validate(ImageUploadRequestDto imageUploadRequestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if(!allowedExtensions.Contains(Path.GetExtension(imageUploadRequestDto.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if(imageUploadRequestDto.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size larger than 10MB. Please upload a smaller size file.");
            }
        }
    }
}
