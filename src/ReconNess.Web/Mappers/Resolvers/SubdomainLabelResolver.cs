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
            source.Labels.ForEach(label =>
            {
                var labelDb = this.labelService.GetByCriteriaAsync(c => c.Name == label.Name).Result;

                if (labelDb != null)
                {
                    SubdomainLabels.Add(new SubdomainLabel
                    {
                        LabelId = labelDb.Id
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

            return SubdomainLabels;
        }
    }
}