using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class ImageUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public String FileName { get; set;}

        public String? FileDescription { get; set; }
    }
}
