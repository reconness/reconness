using System;
using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class TargetValidator : AbstractValidator<TargetDto>
    {
        public TargetValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.RootDomain).Must(rootDomain => IsValidDomainName(rootDomain));
        }

        private static bool IsValidDomainName(string rootDomain)
        {
            return Uri.CheckHostName(rootDomain) != UriHostNameType.Unknown;
        }
    }
}
