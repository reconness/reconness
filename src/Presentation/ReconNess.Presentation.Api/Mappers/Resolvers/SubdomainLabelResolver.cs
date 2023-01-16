using AutoMapper;
using ReconNess.Application;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;
using System.Collections.Generic;

namespace ReconNess.Presentation.Api.Mappers.Resolvers;

internal class SubdomainLabelResolver : IValueResolver<SubdomainDto, Subdomain, ICollection<Label>>
{
    private readonly ILabelService labelService;

    public SubdomainLabelResolver(ILabelService labelService)
    {
        this.labelService = labelService;
    }

    public ICollection<Label> Resolve(SubdomainDto source, Subdomain destination, ICollection<Label> member, ResolutionContext context)
    {
        var labels = new List<Label>();
        if (source.Labels != null)
        {
            source.Labels.ForEach(l =>
            {
                var label = labelService.GetByCriteriaAsync(c => c.Name == l.Name).Result;
                if (label != null)
                {
                    labels.Add(label);
                }
                else
                {
                    labels.Add(new Label
                    {
                        Name = label.Name
                    });
                }
            });
        }

        return labels;
    }
}