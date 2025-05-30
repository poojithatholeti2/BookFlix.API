using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookFlix.API.Data
{
    public class BookFlixAuthDbContext : IdentityDbContext
    {
        public BookFlixAuthDbContext(DbContextOptions<BookFlixAuthDbContext> options) : base(options)
        {

        }

        //seeding roles data
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "d8643559-5652-4efd-814f-c7d980d5813e";
            var writerRoleId = "5cf540de-ce4f-4bbf-8b9d-afc342321f52";
            var adminRoleId = "e35a0494-19e5-4a76-a48b-0a897478cd45";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
