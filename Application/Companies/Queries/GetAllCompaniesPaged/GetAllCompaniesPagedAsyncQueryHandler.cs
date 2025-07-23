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

        protected async override Task<ErrorOr<ApiResponse<IEnumerable<CompanyResponseDTO>>>> HandleRequest(GetAllCompaniesPagedAsyncQuery request, CancellationToken cancellationToken)
        {
            var (companies, totalCount) = await _companyRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            // Recorremos las compañías para generar URLs firmadas
            foreach (var company in companies)
            {
                var cacheKey = $"company:{company.Id}:photos";

                // Intentar obtener desde cache
                var cachedUrls = await _cache.GetAsync<List<string>>(cacheKey);

                if (cachedUrls is not null && cachedUrls.Count > 0)
                {
                    company.SetCoverPhotoUrls(cachedUrls);
                    continue;
                }

                // Si no está cacheado, generamos URLs nuevas
                var signedUrls = new List<string>();
                foreach (var objectName in company.CompanyPhotos)
                {
                    var urlResult = await _fileStorageService.GetFileUrlAsync(
                        BUCKETNAME,
                        objectName,
                        URL_EXPIRY_SECONDS
                    );

                    if (!urlResult.IsError)
                    {
                        signedUrls.Add(urlResult.Value);
                    }
                }

                // Guardar en cache por el mismo tiempo que dura la URL (1h)
                await _cache.SetAsync(cacheKey, signedUrls, TimeSpan.FromSeconds(URL_EXPIRY_SECONDS));

                company.SetCoverPhotoUrls(signedUrls);
            }

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
