using Application.Common;
using Application.Companies.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Companies;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Queries.GetCompanyById
{
    internal class GetCompanyByIdQueryHandler : ApiBaseHandler<GetCompanyByIdQuery, CompanyResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRedisCacheService _cache;
        // Bucket donde se guardan las fotos
        const string BUCKETNAME = "companies-photos";
        const int URL_EXPIRY_SECONDS = 3600;

        public GetCompanyByIdQueryHandler(IMapper mapper, ICompanyRepository companyRepository, ILogger<GetCompanyByIdQueryHandler> logger, IFileStorageService fileStorageService, IRedisCacheService cache) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<ErrorOr<ApiResponse<CompanyResponseDTO>>> HandleRequest(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(new CompanyId(request.Id), cancellationToken);

            if (company is null)
            {
                return Error.NotFound("Company.NotFound", "The company with the provided Id was not found.");
            }

            // Si no tiene fotos, devolvemos directo
            if (company.CompanyPhotos == null || !company.CompanyPhotos.Any())
            {
                var mappedNoPhotos = _mapper.Map<CompanyResponseDTO>(company);
                return new ApiResponse<CompanyResponseDTO>(mappedNoPhotos, true);
            }

            var cacheKey = $"company:{company.Id.Value}:photos";

            // Intentar obtener desde cache
            var cachedUrls = await _cache.GetAsync<List<string>>(cacheKey);

            if (cachedUrls is not null && cachedUrls.Count > 0)
            {
                company.SetCoverPhotoUrls(cachedUrls);

                var mappedCached = _mapper.Map<CompanyResponseDTO>(company);
                return new ApiResponse<CompanyResponseDTO>(mappedCached, true);
            }

            // Si no está cacheado, generamos URLs nuevas
            var signedUrlsTasks = company.CompanyPhotos.Select(async objectName =>
            {
                var urlResult = await _fileStorageService.GetFileUrlAsync(
                    BUCKETNAME,
                    objectName,
                    URL_EXPIRY_SECONDS
                );
                return urlResult.IsError ? null : urlResult.Value;
            });

            var signedUrls = (await Task.WhenAll(signedUrlsTasks))
                .Where(url => url is not null)
                .Cast<string>()
                .ToList();

            // Solo cachear si hay URLs válidas
            if (signedUrls.Count > 0)
            {
                // Cachear un poco menos que la expiración real para evitar URLs vencidas
                var cacheTTL = TimeSpan.FromSeconds(URL_EXPIRY_SECONDS - 60);
                await _cache.SetAsync(cacheKey, signedUrls, cacheTTL);

                company.SetCoverPhotoUrls(signedUrls);
            }

            var mappedResult = _mapper.Map<CompanyResponseDTO>(company);
            return new ApiResponse<CompanyResponseDTO>(mappedResult, true);
        }

    }
}
