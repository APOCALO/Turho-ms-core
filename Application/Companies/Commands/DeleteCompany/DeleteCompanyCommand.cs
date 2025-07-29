using Application.Common;
using MediatR;

namespace Application.Companies.Commands.DeleteCompany
{
    public record DeleteCompanyCommand : BaseResponse<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCompanyCommand(Guid id)
        {
            Id = id;
        }
    }
}
