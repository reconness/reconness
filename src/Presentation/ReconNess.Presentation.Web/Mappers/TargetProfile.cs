using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class TargetProfile : Profile
    {
        public TargetProfile()
        {
            CreateMap<Target, TargetDto>()
                .ReverseMap();
        }
    }
}
