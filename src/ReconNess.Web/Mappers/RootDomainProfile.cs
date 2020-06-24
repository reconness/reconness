using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class RootDomainProfile : Profile
    {
        public RootDomainProfile()
        {
            CreateMap<RootDomain, RootDomainDto>()
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes.Notes));
            CreateMap<RootDomainDto, RootDomain>()
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => new Note { Notes = src.Notes }));
        }
    }
}
