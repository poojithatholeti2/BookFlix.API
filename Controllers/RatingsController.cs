using AutoMapper;
using BookFlix.API.CustomActionFiler;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookFlix.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly BookFlixDbContext dbContext;
        private readonly IRatingRepository ratingRepository;
        private readonly IMapper mapper;

        //list of allowed parameters for getAll request
        private readonly List<String> allowedParameters = new List<string> { "filterOn", "filterQuery", "sortBy", "isAscending", "pageNumber", "pageSize" };

        public RatingsController(BookFlixDbContext dbContext, IRatingRepository ratingRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.ratingRepository = ratingRepository;
            this.mapper = mapper;
        }

        //GET: api/ratings?filterOn=ratingName&filterQuery="string"&sortBy=ratingName&isAscending=true&pageNumber=1&pageSize=1000
        [HttpGet]
        [Authorize(Roles = "Reader, Writer, Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // Check if any extra query parameters are passed
            var extraParameters = HttpContext.Request.Query.Keys.Except(allowedParameters, StringComparer.OrdinalIgnoreCase).ToList();
            if (extraParameters.Any())
            {
                return BadRequest($"Invalid query parameters: {string.Join(", ", extraParameters)}. Allowed parameters are: {string.Join(", ", allowedParameters)}");
            }

            // Validate filterOn and sortBy parameters
            if (string.IsNullOrEmpty(filterOn) == false && !filterOn.Equals("RatingName", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid filterOn parameter. Valid column is : RatingName");
            }

            if (!string.IsNullOrEmpty(sortBy) && !sortBy.Equals("ratingName", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid sortBy parameter. Valid column is : RatingName");
            }

            //controller fetching data from repository which fetches data from dataset(domain models)
            var ratingNamesDomain = await ratingRepository.GetAllAsync(filterOn, filterQuery, 
                sortBy, isAscending, pageNumber, pageSize);

            return Ok(mapper.Map<List<RatingDto>>(ratingNamesDomain));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader, Writer, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //controller fetching data from repository which fetches data from dataset(domain models)
            var ratingNamesDomain = await ratingRepository.GetByIdAsync(id);

            if (ratingNamesDomain == null)
            {
                return BadRequest();
            }

            return Ok(mapper.Map<RatingDto>(ratingNamesDomain));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRatingDto createRatingDto)
        {
            //map dto to domain model
            var ratingDomainModel = mapper.Map<Rating>(createRatingDto);

            //controller fetching data from repository which fetches data from db
            ratingDomainModel = await ratingRepository.CreateAsync(ratingDomainModel);

            //map domain model back to dto
            var ratingDto = mapper.Map<RatingDto>(ratingDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = ratingDto.Id }, ratingDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRatingDto updateRatingDto)
        {
            //map domain model to dto
            var ratingDomainModel = mapper.Map<Rating>(updateRatingDto);

            //fetching data from repository
            ratingDomainModel = await ratingRepository.UpdateAsync(id, ratingDomainModel);
 
            if (ratingDomainModel == null)
            {
                return BadRequest();
            }

            //convert domain model to dto if it's not null
            //create new dto to return to client
            var ratingDto = mapper.Map<RatingDto>(ratingDomainModel);

            //return ok response with dto;
            return Ok(ratingDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var ratingDomainModel = await ratingRepository.DeleteAsync(id);

            if(ratingDomainModel==null)
            {
                return NotFound();
            }
            
            return Ok(mapper.Map<RatingDto>(ratingDomainModel));
        }

    }
}
