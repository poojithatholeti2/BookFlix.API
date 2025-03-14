using System.Reflection;
using AutoMapper;
using BookFlix.API.CustomActionFiler;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories;
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
        private readonly IBookRepository bookRepository;

        // Define valid columns for filtering and sorting
        private static readonly string[] filterValidColumns = { "Title", "Author" };
        private static readonly string[] sortValidColumns = { "Title", "Author", "Price" };

        public BooksController(IMapper mapper, IBookRepository bookRepository)
        {
            this.mapper = mapper;
            this.bookRepository = bookRepository;
        }

        //create
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
        {
            //mapping dto to domain model
            var bookDomainModel = mapper.Map<Book>(createBookDto);

            //controller calling repository for create action on db
            await bookRepository.CreateAsync(bookDomainModel);

            //return dto after mapping domain model back to dto
            return Ok(mapper.Map<BookDto>(bookDomainModel));
        }

        //get all
        //Get: api/books?filterOn=Title&filterQuery="any string"&sortBy=Title&isAscending=true&pageNumber=1&pageSize=1000
        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
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
        [Authorize(Roles = "Reader, Writer")]
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
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            
            //mapping update book dto to book domain model
            var bookDomainModel = mapper.Map<Book>(updateBookDto);

            //controller calling repository for update action 
            bookDomainModel = await bookRepository.UpdateAsync(id, bookDomainModel);

            if (bookDomainModel == null)
            {
                return BadRequest();
            }

            //return dto
            return Ok(mapper.Map<BookDto>(bookDomainModel));
        }

        //delete
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
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
    }
}
