using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class AgentDebugValidator : AbstractValidator<AgentDebugDto>
    {
        public AgentDebugValidator()
        {
            RuleFor(x => x.TerminalOutput).NotEmpty();
            RuleFor(x => x.Script).NotEmpty();
        }
    }
}
