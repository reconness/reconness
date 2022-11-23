using FluentValidation;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Validations;

public class AgentMarketplaceValidator : AbstractValidator<AgentMarketplaceDto>
{
    public AgentMarketplaceValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Command).NotEmpty();
    }
}
