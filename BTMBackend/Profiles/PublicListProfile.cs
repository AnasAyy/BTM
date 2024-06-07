using AutoMapper;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class PublicListProfile : Profile
    {
        public PublicListProfile()
        {
            CreateMap<PublicList, AddItemRequestDto>().ReverseMap();
            CreateMap<PublicList, UpdateItemRequestDto>().ReverseMap();
        }
    }
}
