using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Validators
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(file => file.Length)
                .LessThanOrEqualTo(10 * 1024 * 1024) // 10MB
                .WithMessage("The file must not exceed 10MB.");

            RuleFor(file => file.ContentType)
                .Must(type => type == "image/png" || type == "image/jpeg")
                .WithMessage("Only PNG or JPEG files are allowed.");
        }
    }
}
