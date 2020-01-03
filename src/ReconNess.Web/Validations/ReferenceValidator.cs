using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class ReferenceValidator : AbstractValidator<ReferenceDto>
    {
        public ReferenceValidator()
        {
            RuleFor(x => x.Url).NotEmpty();
            RuleFor(x => x.Categories).NotEmpty();
        }
    }
}
