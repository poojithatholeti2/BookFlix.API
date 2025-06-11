using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace BookFlix.API.Repositories
{
    public class SQLBookRepository : IBookRepository    
    {
        private readonly BookFlixDbContext dbContext;

        public SQLBookRepository(BookFlixDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //create
        public async Task<Book?> CreateAsync(Book book)
        {
            await dbContext.Books.AddAsync(book);
            await dbContext.SaveChangesAsync();

            var createdBook = await dbContext.Books.Include("Category").Include("Rating").FirstOrDefaultAsync(b => book.Id == b.Id);

            return createdBook==null ? null : createdBook;
        }

        //create multiple
        public async Task<List<Book>?> CreateMultipleAsync(List<Book> books)
        {
            var createdBooks = new List<Book>();

            foreach(var b in books)
            {
                var createdBook = await CreateAsync(b);
                if (createdBook == null) return null;
                createdBooks.Add(createdBook);
            }
            return createdBooks;
        }

        //get all
        public async Task<List<Book>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            //collecting books from db 'as queryable' to apply filter
            var books = dbContext.Books.Include("Category").Include("Rating").AsQueryable();

            //filtering
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                //filter by title
                if(filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = books.Where(x => x.Title.Contains(filterQuery));
                }

                //filter by author
                if (filterOn.Equals("Author", StringComparison.OrdinalIgnoreCase))
                {
                    books = books.Where(x => x.Author.Contains(filterQuery));
                }
            }

            //sorting
            if(string.IsNullOrWhiteSpace(sortBy)==false)
            {
                //sort by title
                if (sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(x => x.Title) : books.OrderByDescending(x => x.Title);
                }

                //sort by author
                if (sortBy.Equals("Author", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(x => x.Author) : books.OrderByDescending(x => x.Author);
                }

                //sort by price
                if(sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(x => x.Price) : books.OrderByDescending(x => x.Price);
                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;
            
            //return filtered and sorted result with pagination
            //skips 'skipResults' number of pages and takes 'pageSize' number of records from there
            return await books.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        //get by id
        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await dbContext.Books.Include("Category").Include("Rating").FirstOrDefaultAsync(x => x.Id == id);
        }

        //update
        public async Task<Book?> UpdateAsync(Guid id, Book book)
        {
            var bookDomainModel = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (bookDomainModel == null)
            {
                return null;
            }

            bookDomainModel.Title = book.Title;
            bookDomainModel.Description = book.Description;
            bookDomainModel.Author = book.Author;
            bookDomainModel.Price = book.Price;
            bookDomainModel.CategoryId = book.CategoryId;
            bookDomainModel.RatingId = book.RatingId;

            await dbContext.SaveChangesAsync();

            var result = await GetByIdAsync(id);

            return result;
        }

        //delete
        public async Task<Book?> DeleteAsync(Guid id)
        {
            var bookDomainModel = await GetByIdAsync(id);

            if(bookDomainModel == null)
            {
                return null;
            }

            dbContext.Books.Remove(bookDomainModel);
            await dbContext.SaveChangesAsync();

            return bookDomainModel;
        }

        //save embedding into db
        public async Task SaveEmbeddingAsync(Guid id, Vector embedding)
        {
            var book = await dbContext.Books.FindAsync(id);

            if(book==null) 
                throw new KeyNotFoundException($"Book not found for Id: {id}");
            
            book.Embedding = embedding;
            await dbContext.SaveChangesAsync();
        }

        //fetching books that fall into the closest range for the given inputVector(prompt)
        public async Task<List<Book>?> GetSimilarBooksAsync(Vector inputVector)
        {
            var topBooks = await dbContext.Books
                        .FromSqlRaw(@"
                            SELECT * FROM ""Books""
                            WHERE ""Embedding"" <=> {0} < 0.7 -- Distance threshold
                            ORDER BY ""Embedding"" <=> {0}
                            LIMIT 5", inputVector)
                        .Include("Category").Include("Rating").ToListAsync();

            return topBooks;
        }
    }
}
