using System.ComponentModel.DataAnnotations;

namespace BookFlix.API.Models.DTO
{
    public class RecommendationQueryDto
    {
        [MinLength(24)]
        public string Query { get; set; }

        public bool IsExplanationNeeded { get; set; }
    }
}
