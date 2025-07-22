using Application.Common;
using Application.Interfaces;
using Domain.Customers;
using Domain.Primitives;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.DeleteCustomer
{
    internal sealed class DeleteCustomerCommandHandler : ApiBaseHandler<DeleteCustomerCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, ILogger<DeleteCustomerCommandHandler> logger) : base(logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected async override Task<ErrorOr<ApiResponse<Unit>>> HandleRequest(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            if (await _customerRepository.GetByIdAsync(new CustomerId(request.Id), cancellationToken) is not Customer customer)
            {
                return Error.NotFound("DeleteCustomerCommandHandler.NotFound", "The customer with the provide Id was not found.");
            }

            _customerRepository.Delete(customer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new ApiResponse<Unit>(Unit.Value, true);

            return response;
        }
    }
}
