using AutoMapper;
using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.BrandDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class BrandProfile:Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, CreateBrandRequestDto>().ReverseMap();

        }
    }
}
