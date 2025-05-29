using Microsoft.EntityFrameworkCore;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookFlix.API.Repositories
{
    public class SQLRatingRepository : IRatingRepository
    {
        private readonly BookFlixDbContext dbContext;

        public SQLRatingRepository(BookFlixDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        //get all
        public async Task<List<Rating>> GetAllAsync(string? filterOn, string? filterQuery,
            string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            //collecting ratings from db 'as queryable' to apply filter
            var ratings = dbContext.Ratings.AsQueryable();

            //filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("RatingName", StringComparison.OrdinalIgnoreCase))
                {
                    ratings = ratings.Where(x => x.RatingName == filterQuery);
                }
            }

            //sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                //sort by ratingName
                if (sortBy.Equals("RatingName", StringComparison.OrdinalIgnoreCase))
                {
                    ratings = isAscending ? ratings.OrderBy(x => x.RatingName) : ratings.OrderByDescending(x => x.RatingName);
                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;

            //return filtered and sorted result with pagination
            //skips 'skipResults' number of pages and takes 'pageSize' number of records from there
            return await ratings.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        //get by id
        public async Task<Rating?> GetByIdAsync(Guid id)
        {
            return await dbContext.Ratings.FirstOrDefaultAsync(x  => x.Id == id);
        }

        //create
        public async Task<Rating> CreateAsync(Rating rating)
        {
            await dbContext.Ratings.AddAsync(rating);
            await dbContext.SaveChangesAsync();

            return rating;
        }

        //update
        public async Task<Rating?> UpdateAsync(Guid id, Rating rating)
        {
            var ratingDomainModel = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == id);

            if (ratingDomainModel == null)
            {
                return null;
            }

            ratingDomainModel.RatingName = rating.RatingName;
            await dbContext.SaveChangesAsync();

            return ratingDomainModel;
        }

        //delete
        public async Task<Rating?> DeleteAsync(Guid id)
        {
            var ratingDomainModel = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == id);

            if (ratingDomainModel == null)
            {
                return null;
            }

            dbContext.Remove(ratingDomainModel);
            await dbContext.SaveChangesAsync();

            return ratingDomainModel;
        }
    }
}
