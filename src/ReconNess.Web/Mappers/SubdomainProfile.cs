using System.Linq;
using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using ReconNess.Web.Mappers.Resolvers;

namespace ReconNess.Web.Mappers
{
    public class SubdomainProfile : Profile
    {
        public SubdomainProfile()
        {
            CreateMap<SubdomainDto, Subdomain>()
                .ForMember(dest => dest.Labels, opt => opt.MapFrom<SubdomainLabelResolver>())
                .ForMember(dest => dest.Target, opt => opt.Ignore());

            CreateMap<Subdomain, SubdomainDto>()
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes.Notes))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target.Name))
                .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Labels.Select(l => l.Label))); ;
        }
    }
}
