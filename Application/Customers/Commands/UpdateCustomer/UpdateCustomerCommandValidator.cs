using FluentValidation;

namespace Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(r => r.Name).NotEmpty().MaximumLength(22);
            RuleFor(r => r.LastName).NotEmpty().MaximumLength(22);
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.Country).NotEmpty().MaximumLength(24);
            RuleFor(r => r.Department).NotEmpty().MaximumLength(24);
            RuleFor(r => r.City).NotEmpty().MaximumLength(24);
            RuleFor(r => r.StreetType).NotEmpty().MaximumLength(10);
            RuleFor(r => r.CountryCode).NotEmpty().MaximumLength(4);
        }
    }
}
