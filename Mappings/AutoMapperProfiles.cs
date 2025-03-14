using AutoMapper;
using BookFlix.API.Models.Domain;
using BookFlix.API.Models.DTO;

namespace BookFlix.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //rating mappings
            CreateMap<Rating, RatingDto>().ReverseMap();
            CreateMap<CreateRatingDto, Rating>().ReverseMap();
            CreateMap<UpdateRatingDto, Rating>().ReverseMap();

            //category mappings
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            //book mappings
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<CreateBookDto, Book>().ReverseMap();
            CreateMap<UpdateBookDto, Book>().ReverseMap();
        }
    }
}
