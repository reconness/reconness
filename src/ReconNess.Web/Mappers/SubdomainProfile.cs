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
                .ForMember(dest => dest.RootDomain, opt => opt.Ignore());

            CreateMap<Subdomain, SubdomainDto>()
                .ForMember(dest => dest.RootDomain, opt => opt.MapFrom(src => src.RootDomain.Name))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.RootDomain.Target.Name));
        }
    }
}
