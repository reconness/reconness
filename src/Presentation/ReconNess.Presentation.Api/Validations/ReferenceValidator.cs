using FluentValidation;
using ReconNess.Presentation.Api.Dtos;
using System;

namespace ReconNess.Presentation.Api.Validations;

public class ReferenceValidator : AbstractValidator<ReferenceDto>
{
    public ReferenceValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.Url).Must(url => IsValidUrl(url));
        RuleFor(x => x.Categories).NotEmpty();
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri result);
    }
}
