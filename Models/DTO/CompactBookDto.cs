namespace BookFlix.API.Models.DTO
{
    public class CompactBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
    }
}
