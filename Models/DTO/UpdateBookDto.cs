using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class UpdateBookDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters")]
        [MinLength(3, ErrorMessage = "Min length is 3 characters")]
        public string Title { get; set; }


        [MaxLength(1000, ErrorMessage = "Max length of title is 1000 characters")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters")]
        public string Author { get; set; }

        [Required]
        [Range(0, 100000)]
        public int Price { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid RatingId { get; set; }
    }
}
