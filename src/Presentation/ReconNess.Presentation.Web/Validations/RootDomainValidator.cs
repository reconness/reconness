using FluentValidation;
using ReconNess.Presentation.Api.Dtos;
using System;

namespace ReconNess.Presentation.Api.Validations;

public class RootDomainValidator : AbstractValidator<RootDomainDto>
{
    public RootDomainValidator()
    {
        RuleFor(x => x.Name).Must(rootDomain => IsValidDomainName(rootDomain));
    }

    private static bool IsValidDomainName(string rootDomain)
    {
        return Uri.CheckHostName(rootDomain) != UriHostNameType.Unknown;
    }
}
