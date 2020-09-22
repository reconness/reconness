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
            source.Categories.ForEach(category =>
            {
                var categoryDb = this.categoryService.GetByCriteriaAsync(c => c.Name == category).Result;

                if (categoryDb != null)
                {
                    agentCategories.Add(new AgentCategory
                    {
                        CategoryId = categoryDb.Id
                    });
                }
                else
                {
                    agentCategories.Add(new AgentCategory
                    {
                        Category = new Category
                        {
                            Name = category
                        }
                    });
                }
            });

            return agentCategories;
        }
    }
}