using BookFlix.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookFlix.API.Data
{
    public class BookFlixDbContext: DbContext 
    {
        public BookFlixDbContext(DbContextOptions<BookFlixDbContext> dbContextOptions): base(dbContextOptions) 
        {
            //this.dbContextOptions = dbContextOptions;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //including indexing (hashing) to remove Book triplet duplicates
            modelBuilder.Entity<Book>()
                .HasIndex(book => new { book.Title, book.Author, book.Price })
                .IsUnique();

            //seed data for category database
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = Guid.Parse("bc54bb0c-1d77-48c2-95ea-fd55f691db4c"),
                    Title = "Fiction"
                },
                new Category()
                {
                    Id = Guid.Parse("17ea39ed-3066-44f6-a0c1-d97be6b15de9"),
                    Title = "Literature"
                },
                new Category()
                {
                    Id = Guid.Parse("f0e0164e-a932-4a8b-ba41-291df0d439d5"),
                    Title = "Technology"
                },
                new Category()
                {
                    Id = Guid.Parse("3edb015b-ec4e-45df-8390-b9c66281ab3f"),
                    Title = "History"
                },
                new Category()
                {
                    Id = Guid.Parse("2fa56e3d-d9fb-4453-824a-9094580e5d52"),
                    Title = "Finance"
                },
            };

            //seed data into db
            modelBuilder.Entity<Category>().HasData(categories);


            //seed data for rating database
            var ratings = new List<Rating>()
            {
                new Rating()
                {
                    Id = Guid.Parse("a3c7d69e-0c07-47db-a0fe-f7eb6160f568"),
                    RatingName = "Good"
                },
                new Rating()
                {
                    Id = Guid.Parse("4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"),
                    RatingName = "Average"
                },
                new Rating()
                {
                    Id = Guid.Parse("91f9aee4-d7d3-4ea1-b4ba-e6c11c37efe3"),
                    RatingName = "Bad"
                }
            };

            //seed data into db
            modelBuilder.Entity<Rating>().HasData(ratings);
        }
    }
}
