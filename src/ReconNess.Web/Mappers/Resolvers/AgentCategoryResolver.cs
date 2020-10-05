using AutoMapper;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentCategoryResolver : IValueResolver<AgentDto, Agent, ICollection<AgentCategory>>
    {
        private readonly IAgentCategoryService categoryService;

        public AgentCategoryResolver(IAgentCategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public ICollection<AgentCategory> Resolve(AgentDto source, Agent destination, ICollection<AgentCategory> member, ResolutionContext context)
        {
            var agentCategories = new List<AgentCategory>();
            source.Categories.ForEach(cat =>
            {
                var category = this.categoryService.GetByCriteriaAsync(c => c.Name == cat).Result;
                if (category != null)
                {
                    agentCategories.Add(new AgentCategory
                    {
                        CategoryId = category.Id
                    });
                }
                else
                {
                    agentCategories.Add(new AgentCategory
                    {
                        Category = new Category
                        {
                            Name = cat
                        }
                    });
                }
            });

            return agentCategories;
        }
    }
}