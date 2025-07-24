using Application.Common;
using Application.Companies.DTOs;

namespace Application.Companies.Queries.GetCompanyById
{
    public record GetCompanyByIdQuery : BaseResponse<CompanyResponseDTO>
    {
        public Guid Id { get; set; }

        public GetCompanyByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
