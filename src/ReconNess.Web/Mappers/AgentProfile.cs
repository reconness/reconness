using AutoMapper;
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
                .ForMember(dest => dest.AgentCategories, opt => opt.MapFrom<AgentCategoryResolver>());

            CreateMap<Agent, AgentDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.AgentCategories.Select(c => c.Category.Name)));
        }
    }
}
