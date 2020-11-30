using AutoMapper;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;

namespace ReconNess.Web.Mappers.Resolvers
{
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
                    var label = this.labelService.GetByCriteriaAsync(c => c.Name == l.Name).Result;
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
}