using AutoMapper;
using ReconNess.Core.Models;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using ReconNess.Web.Mappers.Resolvers;
using System.Linq;

namespace ReconNess.Web.Mappers
{
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            CreateMap<AgentDto, Agent>()
                .ForMember(dest => dest.AgentCategories, opt => opt.MapFrom<AgentCategoryResolver>())
                .ForMember(dest => dest.AgentTypes, opt => opt.MapFrom<AgentTypeResolver>());

            CreateMap<Agent, AgentDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.AgentCategories.Select(c => c.Category.Name)))
                .ForMember(dest => dest.IsByTarget, opt => opt.MapFrom(src => src.AgentTypes.Any(c => c.Type.Name == AgentTypes.TARGET)))
                .ForMember(dest => dest.IsByRootDomain, opt => opt.MapFrom(src => src.AgentTypes.Any(c => c.Type.Name == AgentTypes.ROOTDOMAIN)))
                .ForMember(dest => dest.IsBySubdomain, opt => opt.MapFrom(src => src.AgentTypes.Any(c => c.Type.Name == AgentTypes.SUBDOMAIN)))
                .ForMember(dest => dest.IsByDirectory, opt => opt.MapFrom(src => src.AgentTypes.Any(c => c.Type.Name == AgentTypes.DIRECTORY)))
                .ForMember(dest => dest.IsByResource, opt => opt.MapFrom(src => src.AgentTypes.Any(c => c.Type.Name == AgentTypes.RESOURCE)));
        }
    }
}
