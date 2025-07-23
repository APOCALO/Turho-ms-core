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
            _fileStorageService = fileStorageService;
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
            var companyPhotos = new List<string>();

            // Subir cada foto
            for (int i = 0; i < request.CompanyPhostos.Count; i++)
            {
                var photo = request.CompanyPhostos[i];

                // Nombre del archivo en MinIO → companies/{companyId}/photo_1.png
                var objectName = $"companies/{company.Id}/photo_{i + 1}_{Path.GetExtension(photo.FileName)}";

                // Obtener el contentType del archivo
                var contentType = photo.ContentType ?? "image/png";

                // Abrir el stream
                using var stream = photo.OpenReadStream();

                // Subir al almacenamiento privado
                var uploadResult = await _fileStorageService.UploadFileAsync(
                    BUCKETNAME,
                    objectName,
                    stream,
                    contentType
                );

                // Si la subida fue exitosa, guardamos solo el objectName
                if (!uploadResult.IsError)
                {
                    companyPhotos.Add(objectName);
                }
            }

            company.AddPhotos(companyPhotos);

            // Publish event domain
            //customer.RaiseCustomerCreatedEvent(Guid.NewGuid(), customer.Id, customer.Name, customer.Email);

            await _companyRepository.AddAsync(company, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new ApiResponse<Unit>(Unit.Value, true);

            return response;
        }
    }
}
