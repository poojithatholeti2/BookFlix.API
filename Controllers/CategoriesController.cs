using System.Text.Json;
using AutoMapper;
using BookFlix.API.CustomActionFiler;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookFlix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BookFlixDbContext dbContext;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;

        public CategoriesController(BookFlixDbContext dbContext, ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoriesController> logger)
        {
            this.dbContext = dbContext;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        //GET: api/books?filterOn=Title&filterQuery="string"&sortBy=Title&isAscending=true&pageNumber=1&pageSize=1000
        [HttpGet]
        //[Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            //logger logging info
            //logger.LogInformation("GetAll Categories method is invoked");

            // Validate filterOn and sortBy parameters
            if (string.IsNullOrEmpty(filterOn)==false && !filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid filterOn parameter. Valid column is : Title");
            }

            if (!string.IsNullOrEmpty(sortBy) && !sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid sortBy parameter. Valid column is : Title");
            }

            //throwing custom exception to check global exception handling via exceptionHandlerMiddleware
            //throw new Exception("custom error occurred!");

            //controller fetching data from repository
            var categoryDomainModel = await categoryRepository.GetAllAsync(filterOn, filterQuery, 
                sortBy, isAscending ?? true, pageNumber, pageSize);

            //logging info again
            //logger.LogInformation($"Finished GetAll Categories request with data: {JsonSerializer.Serialize(categoryDomainModel)}");

            //domain model converted to dto and returned to client
            return Ok(mapper.Map<List<CategoryDto>>(categoryDomainModel));
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //controller fetching data from repository
            var categoryDomainModel = await categoryRepository.GetByIdAsync(id);

            if (categoryDomainModel == null)
            {
                return NotFound();
            }

            //domain model converted to dto and returned to client
            return Ok(mapper.Map<CategoryDto>(categoryDomainModel));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            //mapping createDto to domain model
            var categoryDomainModel = mapper.Map<Category>(createCategoryDto);

            //controller fetch data from repository
            categoryDomainModel = await categoryRepository.CreateAsync(categoryDomainModel);

            //mapping domain model to dto
            var categoryDto = mapper.Map<CategoryDto>(categoryDomainModel);

            //return dto
            return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            //mapping updateDto to domain model
            var categoryDomainModel = mapper.Map<Category>(updateCategoryDto);

            //controller fetch data from repository
            categoryDomainModel = await categoryRepository.UpdateAsync(id, categoryDomainModel);

            if (categoryDomainModel == null)
            {
                return BadRequest();
            }

            //mapping domain model to dto
            var categoryDto = mapper.Map<CategoryDto>(categoryDomainModel);

            //return dto
            return Ok(categoryDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //controller fetching data from repository
            var categoryDomainModel = await categoryRepository.DeleteAsync(id);

            if(categoryDomainModel == null)
            {
                return NotFound();
            }

            //return dto after mapping domain model to dto
            return Ok(mapper.Map<CategoryDto>(categoryDomainModel));
        }
    }
}
