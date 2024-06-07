using AutoMapper;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CreateCategoryRequestDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryRequestDto>().ReverseMap();

        }
    }
}
