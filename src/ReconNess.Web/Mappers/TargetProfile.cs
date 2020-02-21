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
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes.Notes));
            CreateMap<TargetDto, Target>()
                .ForMember(dest => dest.Subdomains, opt => opt.Ignore());
        }
    }
}
