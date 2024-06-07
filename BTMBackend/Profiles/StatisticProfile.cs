using AutoMapper;
using BTMBackend.Dtos.StatisticDto;
using BTMBackend.Models;

namespace BTMBackend.Profiles
{
    public class StatisticProfile : Profile
    {
        public StatisticProfile()
        {
            CreateMap<Statistic, AddStatisticRequestDto>().ReverseMap();
            CreateMap<Statistic, UpdateUpdateStatisticRequestDto>().ReverseMap();

        }
    }
}
