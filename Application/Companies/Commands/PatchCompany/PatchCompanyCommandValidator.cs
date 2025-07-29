using FluentValidation;

namespace Application.Companies.Commands.PatchCompany
{
    public class PatchCompanyCommandValidator : AbstractValidator<PatchCompanyCommand>
    {
        public PatchCompanyCommandValidator()
        {
            RuleFor(c => c.Name)
                .MaximumLength(100);

            RuleFor(c => c.Description)
                .MaximumLength(500);

            RuleFor(c => c.PhoneNumber)
                .MaximumLength(15);

            RuleFor(c => c.CountryCode)
                .MaximumLength(4);

            RuleFor(c => c.Country)
                .MaximumLength(100);

            RuleFor(c => c.Department)
                .MaximumLength(100);

            RuleFor(c => c.City)
                .MaximumLength(100);

            RuleFor(c => c.StreetType)
                .MaximumLength(20);

            RuleFor(c => c.StreetNumber)
                .MaximumLength(20);

            RuleFor(c => c.CrossStreetNumber)
                .MaximumLength(20);

            RuleFor(c => c.PropertyNumber)
                .MaximumLength(20);

            RuleFor(c => c.ZipCode)
                .MaximumLength(20);

            RuleFor(c => c.Website)
                .MaximumLength(200);

            RuleFor(c => c.AppointmentDurationMinutes)
                .GreaterThan(0)
                .When(c => c.AppointmentDurationMinutes.HasValue);

            RuleFor(c => c.LunchEnd)
                .GreaterThan(c => c.LunchStart.GetValueOrDefault())
                .When(c => c.LunchStart.HasValue && c.LunchEnd.HasValue);

            RuleFor(c => c.TimeZone)
                .MaximumLength(100);
        }
    }

}
