using Application.Common;
using Application.Companies.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Queries.GetAllCompaniesPaged
{
    internal sealed class GetAllCompaniesPagedAsyncQueryHandler
        : ApiBaseHandler<GetAllCompaniesPagedAsyncQuery, IEnumerable<CompanyResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRedisCacheService _cache;

        private const string BUCKETNAME = "companies-photos";
        private const int URL_EXPIRY_SECONDS = 3600;

        public GetAllCompaniesPagedAsyncQueryHandler(
            IMapper mapper,
            ILogger<GetAllCompaniesPagedAsyncQueryHandler> logger,
            ICompanyRepository companyRepository,
            IFileStorageService fileStorageService,
            IRedisCacheService cache)
            : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<ErrorOr<ApiResponse<IEnumerable<CompanyResponseDTO>>>> HandleRequest(
            GetAllCompaniesPagedAsyncQuery request,
            CancellationToken cancellationToken)
        {
            // Obtener compañías paginadas
            var (companies, totalCount) = await _companyRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            if (!companies.Any())
            {
                return new ApiResponse<IEnumerable<CompanyResponseDTO>>(Enumerable.Empty<CompanyResponseDTO>(), true);
            }

            // Procesar URLs de fotos solo para las compañías que tienen fotos
            var companiesWithPhotos = companies
                .Where(c => c.CompanyPhotos != null && c.CompanyPhotos.Count > 0)
                .ToList();

            if (companiesWithPhotos.Count > 0)
            {
                await PopulatePhotoUrlsForCompaniesAsync(companiesWithPhotos);
            }

            // Mapear resultado final
            var mappedResult = _mapper.Map<IEnumerable<CompanyResponseDTO>>(companies);

            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.Pagination.PageSize,
                PageNumber = request.Pagination.PageNumber
            };

            return new ApiResponse<IEnumerable<CompanyResponseDTO>>(mappedResult, true, pagination);
        }

        #region Helpers

        /// <summary>
        /// Obtiene las URLs firmadas para varias compañías en lote, usando cache para las que ya existen.
        /// </summary>
        private async Task PopulatePhotoUrlsForCompaniesAsync(List<Domain.Companies.Company> companies)
        {
            // Generar las keys de cache para todas las compañías
            var cacheKeys = companies
                .Select(c => $"v1:company:{c.Id.Value}:photos")
                .ToList();

            // Intentar obtener todas en una sola llamada a Redis
            var cachedResults = await _cache.GetManyAsync<List<string>>(cacheKeys);

            // Procesar cada compañía en paralelo
            var tasks = companies.Select(async company =>
            {
                var cacheKey = $"v1:company:{company.Id.Value}:photos";

                // Si estaba en cache, usarlo directamente
                if (cachedResults.TryGetValue(cacheKey, out var cachedUrls) && cachedUrls is { Count: > 0 })
                {
                    company.SetCoverPhotoUrls(cachedUrls);
                    return;
                }

                // Si no está en cache → Generar nuevas URLs firmadas
                var signedUrls = await GenerateSignedUrlsAsync(company.CompanyPhotos);

                if (signedUrls.Count > 0)
                {
                    // Cachear con TTL menor que la expiración real
                    var cacheTTL = TimeSpan.FromSeconds(URL_EXPIRY_SECONDS - 60);
                    await _cache.SetAsync(cacheKey, signedUrls, cacheTTL);

                    company.SetCoverPhotoUrls(signedUrls);
                }
            });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Genera URLs firmadas para un conjunto de fotos.
        /// </summary>
        private async Task<List<string>> GenerateSignedUrlsAsync(IReadOnlyCollection<string> photoKeys)
        {
            var tasks = photoKeys.Select(async key =>
            {
                var urlResult = await _fileStorageService.GetFileUrlAsync(
                    BUCKETNAME,
                    key,
                    URL_EXPIRY_SECONDS
                );
                return urlResult.IsError ? null : urlResult.Value;
            });

            var signedUrls = (await Task.WhenAll(tasks))
                .Where(url => url != null)
                .Cast<string>()
                .ToList();

            return signedUrls;
        }

        #endregion
    }
}
