namespace BookFlix.API.Models.DTO
{
    public class RecommendationsDto
    {
        public string Message { get; set; }
        public List<RecommendedBookDto> Books { get; set; }
        public string? Explanation { get; set; }
    }
}
