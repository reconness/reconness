using AutoMapper;
using ReconNess.Application.Services;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;
using System.Collections.Generic;

namespace ReconNess.Presentation.Api.Mappers.Resolvers;

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