using FluentValidation;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Validations;

public class AgentValidator : AbstractValidator<AgentDto>
{
    public AgentValidator()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Command).NotNull();
    }
}
