using System.Reflection;
using AutoMapper;
using BookFlix.API.CustomActionFiler;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories.Interfaces;
using BookFlix.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookFlix.API.Controllers
{
    //https://localhost:portnumber/api/books
    //[Route("api/books")]

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBookService bookService;
        private readonly IBookRepository bookRepository;
        private readonly IRecommendationService recommendationService;

        //list of allowed parameters for getAll request
        private readonly List<String> allowedParameters = new List<string> { 
            "filterOn", 
            "filterQuery", 
            "sortBy", 
            "isAscending", 
            "pageNumber", 
            "pageSize" 
        };

        // Define valid columns for filtering and sorting
        private static readonly List<String> filterValidColumns = new List<String> { 
            "Title", 
            "Author" 
        };

        private static readonly List<String> sortValidColumns = new List<String> { 
            "Title", 
            "Author", 
            "Price" 
        };

        public BooksController(IMapper mapper, IBookRepository bookRepository, IBookService bookService, IRecommendationService recommendationService)
        {
            this.mapper = mapper;
            this.bookRepository = bookRepository;
            this.bookService = bookService;
            this.recommendationService = recommendationService;
        }

        //create
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
        {
            //mapping dto to domain model
            var bookDomainModel = mapper.Map<Book>(createBookDto);

            //controller calling repository for create action on db
            var result = await bookService.CreateAsync(bookDomainModel);
            if (result == null)
            {
                return BadRequest("Book creation failed, the given book already exists.");
            }

            // Return the created book
            var createdBookDto = mapper.Map<BookDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = createdBookDto.Id }, createdBookDto);
        }

        //create multiple
        [HttpPost("bulk")]
        [ValidateModel]
        [Authorize(Roles = "Writer, Admin")]
        public async Task<IActionResult> CreateMultiple([FromBody] List<CreateBookDto> createMultipleBooksDto)
        {

            if (createMultipleBooksDto == null || !createMultipleBooksDto.Any())
            {
                return BadRequest("Books list cannot be empty.");
            }

            var count = createMultipleBooksDto.Count();

            if (count > 200)
            {
                return BadRequest($"Books creation count exceeded (got {count})! Atmost 200 books are allowed at once.");
            }

            //mapping dto to domain model
            var booksDomainModel = mapper.Map<List<Book>>(createMultipleBooksDto);

            //controller calling repository
            var result  = await bookService.CreateMultipleAsync(booksDomainModel);
            var resultCount = result.Count();

            if (resultCount != createMultipleBooksDto.Count())
            {
                return BadRequest($"Books creation failed at Book: {createMultipleBooksDto[resultCount].Title}, the given book already exists.");
            }

            //return dto after mapping domain model back to dto
            return Ok(mapper.Map<List<BookDto>>(result));
        }


        //get all
        //Get: api/books?filterOn=Title&filterQuery="any string"&sortBy=Title&isAscending=true&pageNumber=1&pageSize=1000
        [HttpGet]
        [Authorize(Roles = "Reader, Writer, Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // Check if any extra query parameters are passed
            var extraParameters = HttpContext.Request.Query.Keys.Except(allowedParameters, StringComparer.OrdinalIgnoreCase).ToList();
            if (extraParameters.Any())
            {
                return BadRequest($"Invalid query parameters: {string.Join(", ", extraParameters)}. Allowed parameters are: {string.Join(", ", allowedParameters)}");
            }

            // Validate filterOn and sortBy parameters
            if (!string.IsNullOrEmpty(filterOn) && !filterValidColumns.Contains(filterOn, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid filterOn parameter. Valid columns are: {string.Join(", ", filterValidColumns)}");
            }

            if (!string.IsNullOrEmpty(sortBy) && !sortValidColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid sortBy parameter. Valid columns are: {string.Join(", ", sortValidColumns)}");
            }

            //controller calling repository for get all action 
            var bookDomainModel = await bookRepository.GetAllAsync(filterOn, filterQuery,
                sortBy, isAscending ?? true, pageNumber, pageSize);

            //return dto after mapping domain model to dto
            return Ok(mapper.Map<List<BookDto>>(bookDomainModel));
        }

        //get by id
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader, Writer, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //controller calling repository for get all action 
            var bookDomainModel = await bookRepository.GetByIdAsync(id);

            if (bookDomainModel == null)
            {
                return NotFound();            
            }

            //return dto after mapping domain model to dto
            return Ok(mapper.Map<BookDto>(bookDomainModel));
        }

        //update
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer, Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            
            //mapping update book dto to book domain model
            var updatedBook = mapper.Map<Book>(updateBookDto);

            //controller calling repository for update action 
            var book = await bookService.UpdateAsync(id, updatedBook);

            if (book == null)
            {
                return BadRequest("Book creation failed, the given book already exists");
            }

            //return updated book bto
            var updatedBookdto = mapper.Map<BookDto>(book);
            return Ok(updatedBookdto);
        }

        //delete
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer, Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //controller calling repository for delete action 
            var bookDomainModel = await bookRepository.DeleteAsync(id);

            if (bookDomainModel == null)
            {
                return BadRequest();
            }

            //return dto
            return Ok(mapper.Map<BookDto>(bookDomainModel));
        }

        //recommendations
        [HttpPost("recommend")]
        [Authorize(Roles = "Reader, Writer, Admin")]
        public async Task<IActionResult> GetRecommendation([FromBody] RecommendationQueryDto queryDTO)
        {
            var recommendedBooks = await recommendationService.GetRecommendationAsync(queryDTO);
            return Ok(recommendedBooks);
        }
    }
}
