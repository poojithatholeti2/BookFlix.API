using BookFlix.API.Models.Domain;

namespace BookFlix.API.Models.DTO
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }

        //navigation properties
        public CategoryDto Category { get; set; }
        public RatingDto Rating { get; set; }
    }
}
