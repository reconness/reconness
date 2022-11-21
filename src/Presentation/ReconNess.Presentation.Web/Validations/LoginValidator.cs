using FluentValidation;
using ReconNess.Web.Models;

namespace ReconNess.Web.Validations
{
    public class LoginValidator : AbstractValidator<CredentialsViewModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }

    }
}
