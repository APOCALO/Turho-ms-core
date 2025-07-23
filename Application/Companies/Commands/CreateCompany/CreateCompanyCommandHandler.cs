using Application.Common;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Companies;
using Domain.Primitives;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : ApiBaseHandler<CreateCompanyCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCompanyCommandHandler> logger, ICompanyRepository companyRepository)
            : base(logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        protected async override Task<ErrorOr<ApiResponse<Unit>>> HandleRequest(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            if (PhoneNumber.Create(request.PhoneNumber, request.CountryCode) is not PhoneNumber phoneNumber)
            {
                return Error.Validation("CreateCustomer.PhoneNumber", "PhoneNumber has not valid format.");
            }

            if (Address.Create(request.Country, request.Department, request.City, request.StreetType, request.StreetNumber, request.CrossStreetNumber, request.PropertyNumber, request.ZipCode) is not Address address)
            {
                return Error.Validation("CreateCustomer.Address", "Address has not valid format.");
            }

            var customer = _mapper.Map<Company>(request);

            // Publish event domain
            //customer.RaiseCustomerCreatedEvent(Guid.NewGuid(), customer.Id, customer.Name, customer.Email);

            await _companyRepository.AddAsync(customer, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new ApiResponse<Unit>(Unit.Value, true);

            return response;
        }
    }
}
