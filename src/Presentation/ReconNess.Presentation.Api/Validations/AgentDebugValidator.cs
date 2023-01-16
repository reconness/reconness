using FluentValidation;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Validations;

public class AgentDebugValidator : AbstractValidator<AgentDebugDto>
{
    public AgentDebugValidator()
    {
        RuleFor(x => x.TerminalOutput).NotEmpty();
        RuleFor(x => x.Script).NotEmpty();
    }
}
