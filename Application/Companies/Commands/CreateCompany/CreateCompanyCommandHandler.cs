using Application.Common;
using Application.Interfaces;
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
        private readonly IFileStorageService _fileStorageService;
        // Bucket donde se guardan las fotos
        const string BUCKETNAME = "companies-photos";

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCompanyCommandHandler> logger, ICompanyRepository companyRepository, IFileStorageService fileStorageService)
            : base(logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        protected async override Task<ErrorOr<ApiResponse<Unit>>> HandleRequest(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            if (PhoneNumber.Create(request.PhoneNumber, request.CountryCode) is not PhoneNumber phoneNumber)
            {
                return Error.Validation("CreateCompany.PhoneNumber", "PhoneNumber has not valid format.");
            }

            if (Address.Create(request.Country, request.Department, request.City, request.StreetType, request.StreetNumber, request.CrossStreetNumber, request.PropertyNumber, request.ZipCode) is not Address address)
            {
                return Error.Validation("CreateCompany.Address", "Address has not valid format.");
            }

            var company = _mapper.Map<Company>(request);

            // Creamos las tareas de subida
            var uploadTasks = request.CompanyPhostos.Select(async (photo, index) =>
            {
                var extension = Path.GetExtension(photo.FileName);
                var objectName = $"companies/{company.Id.Value}/photo_{index + 1}{extension}";
                var contentType = photo.ContentType ?? "image/png";

                await using var stream = photo.OpenReadStream();

                var uploadResult = await _fileStorageService.UploadFileAsync(
                    BUCKETNAME,
                    objectName,
                    stream,
                    contentType
                );

                return uploadResult.IsError ? null : objectName;
            });

            // Ejecutamos en paralelo todas las subidas
            var uploadedObjects = await Task.WhenAll(uploadTasks);

            // Filtramos las que fallaron
            var successfulPhotos = uploadedObjects.Where(x => x is not null).Cast<string>().ToList();

            company.AddPhotos(successfulPhotos);

            // Publish event domain
            //customer.RaiseCustomerCreatedEvent(Guid.NewGuid(), customer.Id, customer.Name, customer.Email);

            await _companyRepository.AddAsync(company, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new ApiResponse<Unit>(Unit.Value, true);

            return response;
        }
    }
}
