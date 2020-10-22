using FluentValidation;
using ReconNess.Web.Dtos;
using System;

namespace ReconNess.Web.Validations
{
    public class SubdomainValidator : AbstractValidator<SubdomainDto>
    {
        public SubdomainValidator()
        {
            RuleFor(x => x.Name).Must(domain => IsValidDomainName(domain));
        }

        private static bool IsValidDomainName(string domain)
        {
            return Uri.CheckHostName(domain) != UriHostNameType.Unknown;
        }
    }
}
