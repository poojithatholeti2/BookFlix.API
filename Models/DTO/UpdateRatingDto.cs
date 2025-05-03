using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class UpdateRatingDto
    {
        [Required]
        public String RatingName { get; set; }
    }
}
