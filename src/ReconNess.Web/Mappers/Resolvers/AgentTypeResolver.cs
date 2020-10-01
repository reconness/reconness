using AutoMapper;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentTypeResolver : IValueResolver<AgentDto, Agent, Entities.AgentType>
    {
        private readonly IAgentTypeService agentTypeService;

        public AgentTypeResolver(IAgentTypeService agentTypeService)
        {
            this.agentTypeService = agentTypeService;
        }

        public Entities.AgentType Resolve(AgentDto source, Agent destination, Entities.AgentType member, ResolutionContext context)
        {            
            return this.agentTypeService.GetByCriteriaAsync(t => t.Name.Equals(source.AgentType)).Result;            
        }
    }
}