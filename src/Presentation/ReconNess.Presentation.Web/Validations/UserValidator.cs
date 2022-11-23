using FluentValidation;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Validations;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x).Custom((x, context) =>
        {
            if (!string.IsNullOrWhiteSpace(x.NewPassword))
            {
                if (!x.NewPassword.Equals(x.ConfirmationPassword))
                {
                    context.AddFailure(nameof(x.NewPassword), "Passwords should match");
                }

                if (x.NewPassword.Length <= 6)
                {
                    context.AddFailure(nameof(x.NewPassword), "Password must had more than 6 characters");
                }
            }
        });
    }
}
