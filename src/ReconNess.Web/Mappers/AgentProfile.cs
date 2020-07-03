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
                .ForMember(dest => dest.AgentCategories, opt => opt.MapFrom<AgentCategoryResolver>())
                .ForMember(dest => dest.AgentNotification, opt => opt.MapFrom<AgentNotificationResolver>());

            CreateMap<Agent, AgentDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.AgentCategories.Select(c => c.Category.Name)))
                .ForMember(dest => dest.SubdomainPayload, opt => opt.MapFrom(src => src.AgentNotification.SubdomainPayload ?? string.Empty))
                .ForMember(dest => dest.IpAddressPayload, opt => opt.MapFrom(src => src.AgentNotification.IpAddressPayload ?? string.Empty))
                .ForMember(dest => dest.IsAlivePayload, opt => opt.MapFrom(src => src.AgentNotification.IsAlivePayload ?? string.Empty))
                .ForMember(dest => dest.HasHttpOpenPayload, opt => opt.MapFrom(src => src.AgentNotification.HasHttpOpenPayload ?? string.Empty))
                .ForMember(dest => dest.TakeoverPayload, opt => opt.MapFrom(src => src.AgentNotification.TakeoverPayload ?? string.Empty))
                .ForMember(dest => dest.DirectoryPayload, opt => opt.MapFrom(src => src.AgentNotification.DirectoryPayload ?? string.Empty))
                .ForMember(dest => dest.ServicePayload, opt => opt.MapFrom(src => src.AgentNotification.ServicePayload ?? string.Empty))
                .ForMember(dest => dest.NotePayload, opt => opt.MapFrom(src => src.AgentNotification.NotePayload ?? string.Empty));
        }
    }
}
