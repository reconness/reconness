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
                .ForMember(dest => dest.AgentTrigger, opt => opt.MapFrom<AgentTriggerResolver>());

            CreateMap<Agent, AgentDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.AgentCategories.Select(c => c.Category.Name)))
                .ForMember(dest => dest.TriggerSkipIfRunBefore, opt => opt.MapFrom(src => src.AgentTrigger.SkipIfRunBefore))
                .ForMember(dest => dest.TriggerTargetHasBounty, opt => opt.MapFrom(src => src.AgentTrigger.TargetHasBounty))
                .ForMember(dest => dest.TriggerTargetIncExcName, opt => opt.MapFrom(src => src.AgentTrigger.TargetIncExcName))
                .ForMember(dest => dest.TriggerTargetName, opt => opt.MapFrom(src => src.AgentTrigger.TargetName))
                .ForMember(dest => dest.TriggerRootdomainHasBounty, opt => opt.MapFrom(src => src.AgentTrigger.RootdomainHasBounty))
                .ForMember(dest => dest.TriggerRootdomainIncExcName, opt => opt.MapFrom(src => src.AgentTrigger.RootdomainIncExcName))
                .ForMember(dest => dest.TriggerRootdomainName, opt => opt.MapFrom(src => src.AgentTrigger.RootdomainName))
                .ForMember(dest => dest.TriggerSubdomainHasBounty, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainHasBounty))
                .ForMember(dest => dest.TriggerSubdomainIsAlive, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIsAlive))
                .ForMember(dest => dest.TriggerSubdomainIsMainPortal, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIsMainPortal))
                .ForMember(dest => dest.TriggerSubdomainHasHttpOrHttpsOpen, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainHasHttpOrHttpsOpen))
                .ForMember(dest => dest.TriggerSubdomainIncExcName, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIncExcName))
                .ForMember(dest => dest.TriggerSubdomainName, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainName))
                .ForMember(dest => dest.TriggerSubdomainIncExcServicePort, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIncExcServicePort))
                .ForMember(dest => dest.TriggerSubdomainServicePort, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainServicePort))
                .ForMember(dest => dest.TriggerSubdomainIncExcIP, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIncExcIP))
                .ForMember(dest => dest.TriggerSubdomainIP, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIP))
                .ForMember(dest => dest.TriggerSubdomainIncExcTechnology, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIncExcTechnology))
                .ForMember(dest => dest.TriggerSubdomainTechnology, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainTechnology))
                .ForMember(dest => dest.TriggerSubdomainIncExcLabel, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainIncExcLabel))
                .ForMember(dest => dest.TriggerSubdomainLabel, opt => opt.MapFrom(src => src.AgentTrigger.SubdomainLabel));
        }
    }
}
