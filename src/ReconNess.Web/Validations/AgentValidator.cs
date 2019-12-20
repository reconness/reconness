using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class AgentValidator : AbstractValidator<AgentDto>
    {
        public AgentValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Command).NotNull();
        }
    }
}
