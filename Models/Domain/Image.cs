using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace BookFlix.API.Models.Domain
{
    public class Image
    {
        public Guid Id { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
        public String FileName { get; set; }
        public String? FileDescription { get; set; }
        public String FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public String FilePath { get; set; }
    }
}
