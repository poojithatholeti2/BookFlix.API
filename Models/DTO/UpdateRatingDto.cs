using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class UpdateRatingDto
    {
        [Required]
        public string RatingName { get; set; }
    }
}
