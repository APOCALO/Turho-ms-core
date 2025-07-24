using Application.Common;
using Application.Companies.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Queries.GetAllCompaniesPaged
{
    internal sealed class GetAllCompaniesPagedAsyncQueryHandler : ApiBaseHandler<GetAllCompaniesPagedAsyncQuery, IEnumerable<CompanyResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRedisCacheService _cache;
        // Bucket donde se guardan las fotos
        const string BUCKETNAME = "companies-photos";
        const int URL_EXPIRY_SECONDS = 3600;

        public GetAllCompaniesPagedAsyncQueryHandler(IMapper mapper, ILogger<GetAllCompaniesPagedAsyncQueryHandler> logger, ICompanyRepository companyRepository, IFileStorageService fileStorageService, IRedisCacheService cache) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<ErrorOr<ApiResponse<IEnumerable<CompanyResponseDTO>>>> HandleRequest(GetAllCompaniesPagedAsyncQuery request, CancellationToken cancellationToken)
        {
            var (companies, totalCount) = await _companyRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            if (!companies.Any())
            {
                var emptyResponse = new ApiResponse<IEnumerable<CompanyResponseDTO>>(Enumerable.Empty<CompanyResponseDTO>(), true);
                return emptyResponse;
            }

            // Generar todas las cacheKeys de esta página
            var companyKeys = companies
                .Where(c => c.CompanyPhotos != null && c.CompanyPhotos.Any())
                .Select(c => $"v1:company:{c.Id.Value}:photos")
                .ToList();

            // Una sola llamada a Redis para traer todo lo que exista en caché
            var cachedResults = await _cache.GetManyAsync<List<string>>(companyKeys);

            // Procesar cada compañía
            var processingTasks = companies.Select(async company =>
            {
                if (company.CompanyPhotos == null || !company.CompanyPhotos.Any())
                    return;

                var cacheKey = $"v1:company:{company.Id.Value}:photos";

                // Si ya tenemos en cache, usarlo
                if (cachedResults.TryGetValue(cacheKey, out var cachedUrls) && cachedUrls is not null && cachedUrls.Count > 0)
                {
                    company.SetCoverPhotoUrls(cachedUrls);
                    return;
                }

                // No estaba cacheado → Generamos URLs firmadas en paralelo
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

                if (signedUrls.Count > 0)
                {
                    // Cachear con TTL un poco menor que la expiración real
                    var cacheTTL = TimeSpan.FromSeconds(URL_EXPIRY_SECONDS - 60);
                    await _cache.SetAsync(cacheKey, signedUrls, cacheTTL);

                    company.SetCoverPhotoUrls(signedUrls);
                }
            });

            // Procesar todas las compañías en paralelo
            await Task.WhenAll(processingTasks);

            // Mapear resultado final
            var mappedResult = _mapper.Map<IEnumerable<CompanyResponseDTO>>(companies);

            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.Pagination.PageSize,
                PageNumber = request.Pagination.PageNumber
            };

            var response = new ApiResponse<IEnumerable<CompanyResponseDTO>>(mappedResult, true, pagination);

            return response;
        }

    }
}
