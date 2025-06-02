using BookFlix.API.Models.Domain;

namespace BookFlix.API.Models.DTO
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String? Description { get; set; }
        public String Author { get; set; }
        public int Price { get; set; }

        //navigation properties
        public CategoryDto Category { get; set; }
        public RatingDto Rating { get; set; }
    }
}
