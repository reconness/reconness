using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class AgentMarketplaceValidator : AbstractValidator<AgentMarketplaceDto>
    {
        public AgentMarketplaceValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Command).NotEmpty();
        }
    }
}
