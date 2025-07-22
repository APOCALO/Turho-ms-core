using Application.Common;
using MediatR;

namespace Application.Customers.Commands.DeleteCustomer
{
    public record DeleteCustomerCommand : BaseResponse<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCustomerCommand(Guid id)
        {
            Id = id;
        }
    }
}
