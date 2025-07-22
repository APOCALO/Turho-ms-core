using FluentValidation;

namespace Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(r => r.Name).NotEmpty().MaximumLength(22);
            RuleFor(r => r.LastName).NotEmpty().MaximumLength(22);
            RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(250);
            RuleFor(r => r.Country).NotEmpty().MaximumLength(24);
            RuleFor(r => r.Department).NotEmpty().MaximumLength(24);
            RuleFor(r => r.City).NotEmpty().MaximumLength(24);
            RuleFor(r => r.StreetType).NotEmpty().MaximumLength(10);
            RuleFor(r => r.CountryCode).NotEmpty().MaximumLength(4);
        }
    }
}
