using FluentValidation;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Validations
{
    public class NotificationValidator : AbstractValidator<NotificationDto>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.Url).NotEmpty();
            RuleFor(x => x.Method).NotEmpty();
            RuleFor(x => x.Payload).NotEmpty();
        }

    }
}
