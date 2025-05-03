using AutoMapper;
using BookFlix.API.CustomActionFiler;
using BookFlix.API.Data;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;
using BookFlix.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public RatingsController(BookFlixDbContext dbContext, IRatingRepository ratingRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.ratingRepository = ratingRepository;
            this.mapper = mapper;
        }

        //GET: api/ratings?filterOn=ratingName&filterQuery="string"&sortBy=ratingName&isAscending=true&pageNumber=1&pageSize=1000
        [HttpGet]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool isAscending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            //fetching data directly from dataset(domain models)
            //var ratingNamesDomain = await dbContext.Ratings.ToListAsync();

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

            //map domain models to dtos
            /*var ratingNamesDto = new List<RatingDto>();
            foreach (var rating in RatingNamesDomain)
            {
                ratingNamesDto.Add(new RatingDto()
                {
                    Id = rating.Id,
                    ratingName = rating.RatingName
                });
            }*/

            //returing data from dto
            //return Ok(RatingNamesDto);
            return Ok(mapper.Map<List<RatingDto>>(ratingNamesDomain));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // using find(id) method is one way of getting data using id, but find methos works only with primary keys
            //var ratingNames = dbContext.Ratings.Find(id);

            // using LINQ method - firstordefault
            //var ratingNamesDomain = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == id);

            //controller fetching data from repository which fetches data from dataset(domain models)
            var ratingNamesDomain = await ratingRepository.GetByIdAsync(id);

            if (ratingNamesDomain == null)
            {
                return BadRequest();
            }

            /*var RatingNamesDto = new RatingDto
            {
                Id = RatingNamesDomain.Id,
                RatingName = RatingNamesDomain.RatingName
            };*/

            //return Ok(ratingNamesDto);
            return Ok(mapper.Map<RatingDto>(ratingNamesDomain));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] CreateRatingDto createRatingDto)
        {
            //map dto to domain model
            /*var ratingDomainModel = new Rating
            {
                RatingName = createRatingDto.RatingName
            };*/
            var ratingDomainModel = mapper.Map<Rating>(createRatingDto);

            //add this domain model to database using dbcontext (using the domain model to create a new rating in db)
            //await dbContext.Ratings.AddAsync(ratingDomainModel);
            //await dbContext.SaveChangesAsync();

            //controller fetching data from repository which fetches data from dataset(domain models)
            ratingDomainModel = await ratingRepository.CreateAsync(ratingDomainModel);

            //map domain model back to dto (for us to return this to client)
            /*var ratingDto = new RatingDto
            {
                Id = ratingDomainModel.Id,
                RatingName = ratingDomainModel.RatingName
            };*/
            var ratingDto = mapper.Map<RatingDto>(ratingDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = ratingDto.Id }, ratingDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRatingDto updateRatingDto)
        {
            
            //check if the given id is valid (if any record with such id exists)
            //var ratingDomainModel = await dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == id);
            //if (ratingDomainModel == null) 
            //{ 
            //    return BadRequest(); 
            //}

            //map domain model to dto
            //ratingDomainModel.RatingName = updateRatingDto.RatingName;

            //save these changes in DB
            //await dbContext.SaveChangesAsync();

            //creating new domain model from given dto
            /*var ratingDomainModel = new Rating
            {
                RatingName = updateRatingDto.RatingName
            };*/
            var ratingDomainModel = mapper.Map<Rating>(updateRatingDto);

            //fetching data from repository
            ratingDomainModel = await ratingRepository.UpdateAsync(id, ratingDomainModel);

            //convert domain model to dto if it's not null (create new dto to return to client)
            if (ratingDomainModel == null)
            {
                return BadRequest();
            }
            /*var ratingDto = new RatingDto
            {
                Id = ratingDomainModel.Id,
                RatingName = updateRatingDto.RatingName
            };*/
            var ratingDto = mapper.Map<RatingDto>(ratingDomainModel);

            //return ok response with dto;
            return Ok(ratingDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //check if the given id is valid (record exists)
            var ratingDomainModel = await ratingRepository.DeleteAsync(id);

            if(ratingDomainModel==null)
            {
                return NotFound();
            }

            //remove that record from DB using dbContext
            //dbContext.Remove(ratingDomainModel);
            //save the changes
            //await dbContext.SaveChangesAsync();

            //map domain model to dto if it's not null (to return deleted rating back to client)
            //(this is optional, we can send empty 'ok' response as well)
            /*var ratingDto = new RatingDto
            {
                Id = ratingDomainModel.Id,
                RatingName = ratingDomainModel.RatingName
            };*/

            return Ok(mapper.Map<RatingDto>(ratingDomainModel));
        }

    }
}
