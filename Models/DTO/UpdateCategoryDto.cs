using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class UpdateCategoryDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters")]
        [MinLength(3, ErrorMessage = "Min length is 3 characters")]
        public String Title { get; set; }
    }
}
