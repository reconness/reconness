using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;
using ReconNess.Presentation.Api.Mappers.Resolvers;

namespace ReconNess.Presentation.Api.Mappers;

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
