using AutoMapper;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.PartDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, CreateProductRequestDto>().ReverseMap();
            CreateMap<Product, UpdateProductRequestDto>().ReverseMap();
            CreateMap<Part, CreatePartRequestDto>().ReverseMap();
            CreateMap<Part, UpdatePartRequestDto>().ReverseMap();

        }
    }
}
