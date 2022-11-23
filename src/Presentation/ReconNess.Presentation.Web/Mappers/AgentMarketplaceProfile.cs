using AutoMapper;
using ReconNess.Application.Models;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;
using ReconNess.Web.Mappers.Resolvers;

namespace ReconNess.Web.Mappers;

public class AgentMarketplaceProfile : Profile
{
    public AgentMarketplaceProfile()
    {
        CreateMap<AgentMarketplace, AgentMarketplaceDto>();

        CreateMap<AgentMarketplaceDto, Agent>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom<AgentMarketplaceCategoryResolver>())
            .ForMember(dest => dest.AgentType, opt => opt.MapFrom(src => src.IsByTarget ? AgentTypes.TARGET : src.IsByRootDomain ? AgentTypes.ROOTDOMAIN : AgentTypes.SUBDOMAIN));
    }
}
