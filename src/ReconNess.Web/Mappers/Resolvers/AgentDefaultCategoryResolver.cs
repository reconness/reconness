using System.Collections.Generic;
using AutoMapper;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentDefaultCategoryResolver : IValueResolver<AgentDefaultDto, Agent, ICollection<AgentCategory>>
    {
        private readonly ICategoryService categoryService;

        public AgentDefaultCategoryResolver(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public ICollection<AgentCategory> Resolve(AgentDefaultDto source, Agent destination, ICollection<AgentCategory> member, ResolutionContext context)
        {
            var agentCategories = new List<AgentCategory>();

            var categoryDb = this.categoryService.GetByCriteriaAsync(c => c.Name == source.Category).Result;

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
                        Name = source.Category
                    }
                });
            }

            return agentCategories;
        }
    }
}