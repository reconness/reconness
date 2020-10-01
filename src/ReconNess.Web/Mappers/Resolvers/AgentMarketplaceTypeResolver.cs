using AutoMapper;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentMarketplaceTypeResolver : IValueResolver<AgentMarketplaceDto, Agent, Entities.AgentType>
    {
        private readonly IAgentTypeService agentTypeService;

        public AgentMarketplaceTypeResolver(IAgentTypeService agentTypeService)
        {
            this.agentTypeService = agentTypeService;
        }

        public Entities.AgentType Resolve(AgentMarketplaceDto source, Agent destination, Entities.AgentType member, ResolutionContext context)
        {
            if (source.IsByTarget)
            {
                return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(Core.Models.AgentTypes.TARGET)).Result;
            }

            if (source.IsByRootDomain)
            {
                return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(Core.Models.AgentTypes.ROOTDOMAIN)).Result;
            }

            if (source.IsBySubdomain)
            {
                return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(Core.Models.AgentTypes.SUBDOMAIN)).Result;
            }

            if (source.IsByDirectory)
            {
                return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(Core.Models.AgentTypes.DIRECTORY)).Result;
            }

            if (source.IsByResource)
            {
                return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(Core.Models.AgentTypes.RESOURCE)).Result;
            }

            return null;
        }
    }
}