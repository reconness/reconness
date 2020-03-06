using System.Linq;
using AutoMapper;
using ReconNess.Core.Models;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using ReconNess.Web.Mappers.Resolvers;

namespace ReconNess.Web.Mappers
{
    public class AgentDefaultProfile : Profile
    {
        public AgentDefaultProfile()
        {
            CreateMap<AgentDefault, AgentDefaultDto>();

            CreateMap<AgentDefaultDto, Agent>()
                .ForMember(dest => dest.AgentCategories, opt => opt.MapFrom<AgentDefaultCategoryResolver>());
        }
    }
}
