using FluentValidation;
using ReconNess.Web.Dtos;
using System;
using System.Collections.Generic;

namespace ReconNess.Web.Validations;

public class TargetValidator : AbstractValidator<TargetDto>
{
    public TargetValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.RootDomains).NotEmpty();
        RuleFor(x => x.RootDomains).Must(r => IsValidDomainName(r));
    }

    private static bool IsValidDomainName(List<RootDomainDto> rootDomains)
    {
        foreach (var rootDomain in rootDomains)
        {
            if (Uri.CheckHostName(rootDomain.Name) == UriHostNameType.Unknown)
            {
                return false;
            }
        }

        return true;
    }
}
