using AutoMapper;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentMarketplaceCategoryResolver : IValueResolver<AgentMarketplaceDto, Agent, ICollection<Category>>
    {
        private readonly IAgentCategoryService categoryService;

        public AgentMarketplaceCategoryResolver(IAgentCategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public ICollection<Category> Resolve(AgentMarketplaceDto source, Agent destination, ICollection<Category> member, ResolutionContext context)
        {
            var agentCategories = new List<Category>();

            var category = categoryService.GetByCriteriaAsync(c => c.Name == source.Category).Result;
            if (category != null)
            {
                agentCategories.Add(category);
            }
            else
            {
                agentCategories.Add(new Category
                {
                    Name = source.Category
                });
            }

            return agentCategories;
        }
    }
}