using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace BookFlix.API.Models.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String? Description { get; set; } 
        public String Author { get; set; }  
        public int Price { get; set; }
        public Guid CategoryId { get; set; }   
        public Guid RatingId { get; set; }

        [Column(TypeName = "vector(384)")] 
        public Vector? Embedding { get; set; }


        //navigation properties
        public Category Category { get; set; }
        public Rating Rating { get; set; }  
    }
}
