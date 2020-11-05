using AutoMapper;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using System.Collections.Generic;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class SubdomainLabelResolver : IValueResolver<SubdomainDto, Subdomain, ICollection<SubdomainLabel>>
    {
        private readonly ILabelService labelService;

        public SubdomainLabelResolver(ILabelService labelService)
        {
            this.labelService = labelService;
        }

        public ICollection<SubdomainLabel> Resolve(SubdomainDto source, Subdomain destination, ICollection<SubdomainLabel> member, ResolutionContext context)
        {
            var SubdomainLabels = new List<SubdomainLabel>();
            if (source.Labels != null)
            {
                source.Labels.ForEach(l =>
                {
                    var label = this.labelService.GetByCriteriaAsync(c => c.Name == l.Name).Result;
                    if (label != null)
                    {
                        SubdomainLabels.Add(new SubdomainLabel
                        {
                            LabelId = label.Id
                        });
                    }
                    else
                    {
                        SubdomainLabels.Add(new SubdomainLabel
                        {
                            Label = new Label
                            {
                                Name = label.Name
                            }
                        });
                    }
                });
            }

            return SubdomainLabels;
        }
    }
}