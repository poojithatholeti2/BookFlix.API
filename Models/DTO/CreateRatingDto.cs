using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class CreateRatingDto
    {
        [Required]
        public String RatingName { get; set; }
    }
}
