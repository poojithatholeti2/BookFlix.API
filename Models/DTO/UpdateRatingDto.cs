using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class UpdateRatingDto
    {
        [Required]
        [Range(0, 5)]
        public int RatingValue { get; set; }
    }
}
