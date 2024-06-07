using AutoMapper;
using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class AboutUsProfile : Profile
    {
        public AboutUsProfile()
        {
            CreateMap<AboutUs, CreateAboutUsRequestDto>().ReverseMap();
            CreateMap<AboutUs, UpdateAboutUsRequestDto>().ReverseMap();

        }
    }
}
