using Application.Common;
using Application.Companies.DTOs;

namespace Application.Companies.Queries.GetAllCompaniesPaged
{
    public record GetAllCompaniesPagedAsyncQuery : BaseResponse<IEnumerable<CompanyResponseDTO>>
    {
        public PaginationParameters Pagination { get; set; }

        public GetAllCompaniesPagedAsyncQuery(PaginationParameters pagination)
        {
            Pagination = pagination;
        }
    }
}
