using AutoMapper;
using BTMBackend.Dtos.ContactUsDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class ContactUsProfile : Profile
    {
        public ContactUsProfile()
        {
            CreateMap<ContactUs, ContactUsRequestDto>().ReverseMap();
           
        }
    }
}
