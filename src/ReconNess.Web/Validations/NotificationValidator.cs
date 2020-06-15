using FluentValidation;
using ReconNess.Web.Dtos;
using System;

namespace ReconNess.Web.Validations
{
    public class NotificationValidator : AbstractValidator<NotificationDto>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.Url).NotEmpty();
            RuleFor(x => x.Method).Must(m => "POST".Equals(m) || "GET".Equals(m));
            RuleFor(x => x.Payload).NotEmpty();
            RuleFor(x => x.Url).Must(url => IsValidUrl(url));
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result);
        }
    }
}
