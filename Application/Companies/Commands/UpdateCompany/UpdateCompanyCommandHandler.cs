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

namespace Application.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandHandler : ApiBaseHandler<UpdateCompanyCommand, CompanyResponseDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICompanyRepository _companyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRedisCacheService _cache;

    private const string BUCKETNAME = "companies-photos";
    private const int URL_EXPIRY_SECONDS = 3600;

    public UpdateCompanyCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateCompanyCommandHandler> logger,
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
        UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var companyId = new CompanyId(request.Id);
        var existingCompany = await _companyRepository.GetByIdAsync(companyId, cancellationToken);

        if (existingCompany is null)
        {
            return Error.NotFound("UpdateCompany.NotFound", $"Company with ID {request.Id} not found.");
        }

        var phoneNumber = ValidatePhoneNumber(request.PhoneNumber, request.CountryCode);
        if (phoneNumber.IsError) return phoneNumber.Errors;

        var address = ValidateAddress(request);
        if (address.IsError) return address.Errors;

        _mapper.Map(request, existingCompany);

        // Procesar nuevas fotos (si las hay)
        if (request.NewCompanyPhotos?.Any() == true)
        {
            var oldPhotoKeys = existingCompany.CompanyPhotos?.ToList() ?? new();

            var newPhotoKeys = await UploadPhotosAsync(companyId.Value, request.NewCompanyPhotos);
            existingCompany.SetPhotos(newPhotoKeys);

            await DeletePhotosAsync(companyId.Value, oldPhotoKeys);
        }

        _companyRepository.Update(existingCompany);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reemplazar URLs en cache (si hay fotos)
        if (existingCompany.CompanyPhotos is { Count: > 0 })
        {
            var signedUrls = await GenerateAndCacheSignedUrlsAsync(companyId.Value, existingCompany.CompanyPhotos);
            existingCompany.SetCoverPhotoUrls(signedUrls);
        }

        var mapped = _mapper.Map<CompanyResponseDTO>(existingCompany);
        return new ApiResponse<CompanyResponseDTO>(mapped, true);
    }

    #region Helpers

    private static ErrorOr<PhoneNumber> ValidatePhoneNumber(string phone, string countryCode) =>
        PhoneNumber.Create(phone, countryCode) is PhoneNumber value
            ? value
            : Error.Validation("UpdateCompany.PhoneNumber", "Phone number format is invalid.");

    private static ErrorOr<Address> ValidateAddress(UpdateCompanyCommand request) =>
        Address.Create(request.Country, request.Department, request.City, request.StreetType,
                       request.StreetNumber, request.CrossStreetNumber, request.PropertyNumber, request.ZipCode) is Address value
            ? value
            : Error.Validation("UpdateCompany.Address", "Address format is invalid.");

    private async Task DeletePhotosAsync(Guid companyId, IEnumerable<string> photoKeys)
    {
        var deleteTasks = photoKeys.Select(async key =>
        {
            var deleteResult = await _fileStorageService.DeleteFileAsync(BUCKETNAME, key);

            if (deleteResult.IsError)
            {
                _logger.LogWarning("Failed to delete photo {PhotoKey} for company {CompanyId}. Error: {Error}",
                    key, companyId, deleteResult.FirstError.Description);
            }
        });

        await Task.WhenAll(deleteTasks);
    }


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
