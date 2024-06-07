using AutoMapper;
using BTMBackend.Dtos.ExternalLinkDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class ExternalLinkProfile : Profile
    {
        public ExternalLinkProfile()
        {
            CreateMap<ExternalLink, AddExternalLinkRequestDto>().ReverseMap();
            CreateMap<ExternalLink, UpdateExternalLinkRequestDto>().ReverseMap();
        }
    }
}
