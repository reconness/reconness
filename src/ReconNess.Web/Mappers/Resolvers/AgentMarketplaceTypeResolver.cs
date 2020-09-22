using AutoMapper;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentMarketplaceTypeResolver : IValueResolver<AgentMarketplaceDto, Agent, ICollection<AgentType>>
    {
        private readonly IAgentTypeService agentTypeService;

        public AgentMarketplaceTypeResolver(IAgentTypeService agentTypeService)
        {
            this.agentTypeService = agentTypeService;
        }

        public ICollection<AgentType> Resolve(AgentMarketplaceDto source, Agent destination, ICollection<AgentType> member, ResolutionContext context)
        {
            var agentTypes = new List<AgentType>();

            var types = this.agentTypeService.GetAllAsync().Result;

            if (source.IsByTarget)
            {
                agentTypes.Add(new AgentType
                {
                    TypeId = types.FirstOrDefault(t => t.Name == AgentTypes.TARGET).Id
                });
            }

            if (source.IsByRootDomain)
            {
                agentTypes.Add(new AgentType
                {
                    TypeId = types.FirstOrDefault(t => t.Name == AgentTypes.ROOTDOMAIN).Id
                });
            }

            if (source.IsBySubdomain)
            {
                agentTypes.Add(new AgentType
                {
                    TypeId = types.FirstOrDefault(t => t.Name == AgentTypes.SUBDOMAIN).Id
                });
            }

            if (source.IsByDirectory)
            {
                agentTypes.Add(new AgentType
                {
                    TypeId = types.FirstOrDefault(t => t.Name == AgentTypes.DIRECTORY).Id
                });
            }

            if (source.IsByResource)
            {
                agentTypes.Add(new AgentType
                {
                    TypeId = types.FirstOrDefault(t => t.Name == AgentTypes.RESOURCE).Id
                });
            }

            return agentTypes;
        }
    }
}