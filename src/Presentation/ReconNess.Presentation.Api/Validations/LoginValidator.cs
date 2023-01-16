using FluentValidation;
using ReconNess.Presentation.Api.Models;

namespace ReconNess.Presentation.Api.Validations;

public class LoginValidator : AbstractValidator<CredentialsViewModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }

}
