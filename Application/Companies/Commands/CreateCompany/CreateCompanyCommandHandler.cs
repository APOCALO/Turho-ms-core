using Application.Common;
using Application.Companies.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Companies;
using Domain.Primitives;
using Domain.ValueObjects;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Commands.CreateCompany
{
    internal sealed class CreateCompanyCommandHandler : ApiBaseHandler<CreateCompanyCommand, CompanyResponseDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRedisCacheService _cache;

        private const string BUCKETNAME = "companies-photos";
        private const int URL_EXPIRY_SECONDS = 3600;

        public CreateCompanyCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateCompanyCommandHandler> logger,
            ICompanyRepository companyRepository,
            IFileStorageService fileStorageService,
            IRedisCacheService cache)
            : base(logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<ErrorOr<ApiResponse<CompanyResponseDTO>>> HandleRequest(
            CreateCompanyCommand request,
            CancellationToken cancellationToken)
        {
            // Validar y construir objetos de valor
            var phoneNumber = ValidatePhoneNumber(request.PhoneNumber, request.CountryCode);
            if (phoneNumber.IsError) return phoneNumber.Errors;

            var address = ValidateAddress(request);
            if (address.IsError) return address.Errors;

            // Crear entidad y persistir
            var company = _mapper.Map<Company>(request);

            // Subir fotos (si las hay)
            if (request.CompanyPhostos?.Any() == true)
            {
                var successfulPhotos = await UploadPhotosAsync(company.Id.Value, request.CompanyPhostos);
                company.SetPhotos(successfulPhotos);
            }

            await _companyRepository.AddAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Si no hay fotos, devolvemos directamente
            if (company.CompanyPhotos == null || company.CompanyPhotos.Count == 0)
            {
                return new ApiResponse<CompanyResponseDTO>(_mapper.Map<CompanyResponseDTO>(company), true);
            }

            // Generar URLs firmadas y cachearlas
            var signedUrls = await GenerateAndCacheSignedUrlsAsync(company.Id.Value, company.CompanyPhotos);
            company.SetCoverPhotoUrls(signedUrls);

            // Respuesta final
            var mappedResult = _mapper.Map<CompanyResponseDTO>(company);
            return new ApiResponse<CompanyResponseDTO>(mappedResult, true);
        }

        #region Helpers

        private static ErrorOr<PhoneNumber> ValidatePhoneNumber(string phone, string countryCode) =>
            PhoneNumber.Create(phone, countryCode) is PhoneNumber value
                ? value
                : Error.Validation("CreateCompany.PhoneNumber", "PhoneNumber has invalid format.");

        private static ErrorOr<Address> ValidateAddress(CreateCompanyCommand request) =>
            Address.Create(request.Country, request.Department, request.City, request.StreetType,
                           request.StreetNumber, request.CrossStreetNumber, request.PropertyNumber, request.ZipCode) is Address value
                ? value
                : Error.Validation("CreateCompany.Address", "Address has invalid format.");

        private async Task<List<string>> UploadPhotosAsync(Guid companyId, IEnumerable<IFormFile> photos)
        {
            var uploadTasks = photos.Select(async (photo, index) =>
            {
                var extension = Path.GetExtension(photo.FileName);
                var objectName = $"companies/{companyId}/photo_{index + 1}{extension}";
                var contentType = photo.ContentType ?? "image/png";

                await using var stream = photo.OpenReadStream();
                var uploadResult = await _fileStorageService.UploadFileAsync(BUCKETNAME, objectName, stream, contentType);

                return uploadResult.IsError ? null : objectName;
            });

            var uploaded = await Task.WhenAll(uploadTasks);
            return uploaded.Where(x => x != null).Cast<string>().ToList();
        }

        private async Task<List<string>> GenerateAndCacheSignedUrlsAsync(Guid companyId, IReadOnlyCollection<string> photoKeys)
        {
            var signedUrlTasks = photoKeys.Select(async key =>
            {
                var urlResult = await _fileStorageService.GetFileUrlAsync(BUCKETNAME, key, URL_EXPIRY_SECONDS);
                return urlResult.IsError ? null : urlResult.Value;
            });

            var signedUrls = (await Task.WhenAll(signedUrlTasks))
                .Where(url => url != null)
                .Cast<string>()
                .ToList();

            if (signedUrls.Count > 0)
            {
                var cacheKey = $"v1:company:{companyId}:photos";
                var cacheTTL = TimeSpan.FromSeconds(URL_EXPIRY_SECONDS - 60);
                await _cache.SetAsync(cacheKey, signedUrls, cacheTTL);
            }

            return signedUrls;
        }

        #endregion
    }
}
