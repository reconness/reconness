using FluentValidation;
using ReconNess.Web.Dtos;
using System;

namespace ReconNess.Web.Validations
{
    public class SubdmainValidator : AbstractValidator<SubdomainDto>
    {
        public SubdmainValidator()
        {
            RuleFor(x => x.Name).Must(rootDomain => IsValidDomainName(rootDomain));
        }

        private static bool IsValidDomainName(string rootDomain)
        {
            return Uri.CheckHostName(rootDomain) != UriHostNameType.Unknown;
        }
    }
}
