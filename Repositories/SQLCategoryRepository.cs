using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookFlix.API.Repositories
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly BookFlixDbContext dbContext;

        public SQLCategoryRepository(BookFlixDbContext dbContext)
        {
            this.dbContext = dbContext;
        } 

        //get all
        public async Task<List<Category>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //collecting categories from db 'as queryable' to apply filter
            var categories = dbContext.Categories.AsQueryable();

            //filtering
            if (string.IsNullOrWhiteSpace(filterOn)==false && string.IsNullOrWhiteSpace(filterQuery)==false)
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    categories = categories.Where(x => x.Title.Contains(filterQuery));
                }
            }

            //sorting
            if(string.IsNullOrWhiteSpace(sortBy)==false)
            {
                //sort by title
                if(sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    categories = isAscending ? categories.OrderBy(x => x.Title) : categories.OrderByDescending(x => x.Title);
                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await categories.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        //get by id
        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(x  => x.Id == id);
        }

        //create
        public async Task<Category> CreateAsync(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return category;
        }

        //update
        public async Task<Category?> UpdateAsync(Guid id, Category category)
        {
            var categoriesDomainModel = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (categoriesDomainModel == null)
            {
                return null;
            }

            categoriesDomainModel.Title = category.Title;
            await dbContext.SaveChangesAsync();

            return categoriesDomainModel;
        }

        //delete
        public async Task<Category?> DeleteAsync(Guid id)
        {
            var categoriesDomainModel = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(categoriesDomainModel == null)
            {
                return null;
            }

            dbContext.Remove(categoriesDomainModel);
            await dbContext.SaveChangesAsync();

            return categoriesDomainModel;
        }

    }
}
