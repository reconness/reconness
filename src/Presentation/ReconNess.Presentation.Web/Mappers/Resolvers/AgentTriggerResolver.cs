using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers.Resolvers;

internal class AgentTriggerResolver : IValueResolver<AgentDto, Agent, AgentTrigger>
{
    public AgentTriggerResolver()
    {
    }

    public AgentTrigger Resolve(AgentDto source, Agent destination, AgentTrigger member, ResolutionContext context)
    {
        var agentTrigger = destination.AgentTrigger;
        if (agentTrigger == null)
        {
            agentTrigger = new AgentTrigger();
        }

        agentTrigger.SkipIfRunBefore = source.TriggerSkipIfRunBefore;
        agentTrigger.TargetHasBounty = source.TriggerTargetHasBounty;
        agentTrigger.TargetIncExcName = source.TriggerTargetIncExcName;
        agentTrigger.TargetName = source.TriggerTargetName;
        agentTrigger.RootdomainHasBounty = source.TriggerRootdomainHasBounty;
        agentTrigger.RootdomainIncExcName = source.TriggerRootdomainIncExcName;
        agentTrigger.RootdomainName = source.TriggerRootdomainName;
        agentTrigger.SubdomainIsAlive = source.TriggerSubdomainIsAlive;
        agentTrigger.SubdomainIsMainPortal = source.TriggerSubdomainIsMainPortal;
        agentTrigger.SubdomainHasHttpOrHttpsOpen = source.TriggerSubdomainHasHttpOrHttpsOpen;
        agentTrigger.SubdomainIncExcName = source.TriggerSubdomainIncExcName;
        agentTrigger.SubdomainName = source.TriggerSubdomainName;
        agentTrigger.SubdomainIncExcServicePort = source.TriggerSubdomainIncExcServicePort;
        agentTrigger.SubdomainServicePort = source.TriggerSubdomainServicePort;
        agentTrigger.SubdomainIncExcIP = source.TriggerSubdomainIncExcIP;
        agentTrigger.SubdomainIP = source.TriggerSubdomainIP;
        agentTrigger.SubdomainIncExcTechnology = source.TriggerSubdomainIncExcTechnology;
        agentTrigger.SubdomainTechnology = source.TriggerSubdomainTechnology;
        agentTrigger.SubdomainIncExcLabel = source.TriggerSubdomainIncExcLabel;
        agentTrigger.SubdomainLabel = source.TriggerSubdomainLabel;

        return agentTrigger;
    }
}