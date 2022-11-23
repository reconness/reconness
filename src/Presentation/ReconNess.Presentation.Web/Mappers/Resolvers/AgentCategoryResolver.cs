using AutoMapper;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;

namespace ReconNess.Web.Mappers.Resolvers;

internal class AgentCategoryResolver : IValueResolver<AgentDto, Agent, ICollection<Category>>
{
    private readonly IAgentCategoryService categoryService;

    public AgentCategoryResolver(IAgentCategoryService categoryService)
    {
        this.categoryService = categoryService;
    }

    public ICollection<Category> Resolve(AgentDto source, Agent destination, ICollection<Category> member, ResolutionContext context)
    {
        var agentCategories = new List<Category>();
        source.Categories.ForEach(cat =>
        {
            var category = categoryService.GetByCriteriaAsync(c => c.Name == cat).Result;
            if (category != null)
            {
                agentCategories.Add(category);
            }
            else
            {
                agentCategories.Add(new Category
                {
                    Name = cat
                });
            }
        });

        return agentCategories;
    }
}