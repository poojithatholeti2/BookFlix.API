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
        public async Task<List<Rating>> GetAllAsync(string? filterOn = null, int? filterQuery = 0,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //collecting ratings from db 'as queryable' to apply filter
            var ratings = dbContext.Ratings.AsQueryable();

            //filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && filterQuery!=0)
            {
                if (filterOn.Equals("RatingValue", StringComparison.OrdinalIgnoreCase))
                {
                    ratings = ratings.Where(x => x.RatingValue == filterQuery);
                }
            }

            //sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                //sort by ratingValue
                if (sortBy.Equals("RatingValue", StringComparison.OrdinalIgnoreCase))
                {
                    ratings = isAscending ? ratings.OrderBy(x => x.RatingValue) : ratings.OrderByDescending(x => x.RatingValue);
                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;

            //return filtered and sorted result with pagination
            //skips 'skipResults' number of pages and takes 'pageSize' number of records from there
            return await ratings.Skip(skipResults).Take(pageSize).ToListAsync();

            //without filtering, without sorting, without pagination
            //return await dbContext.Ratings.ToListAsync();
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

            ratingDomainModel.RatingValue = rating.RatingValue;
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
