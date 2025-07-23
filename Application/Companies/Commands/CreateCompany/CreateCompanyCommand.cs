using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Companies.Commands.CreateCompany
{
    public record CreateCompanyCommand : BaseResponse<Unit>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public List<IFormFile> CompanyPhostos { get; init; } = new();
        public string PhoneNumber { get; init; }
        public string CountryCode { get; init; }
        public string Country { get; init; }
        public string Department { get; init; }
        public string City { get; init; }
        public string StreetType { get; init; }
        public string StreetNumber { get; init; }
        public string CrossStreetNumber { get; init; }
        public string PropertyNumber { get; init; }
        public string? ZipCode { get; init; }
        public string? Website { get; init; }
        public required IReadOnlyList<DayOfWeek> WorkingDays { get; init; }
        public TimeSpan OpeningHour { get; init; }
        public TimeSpan ClosingHour { get; init; }
        public TimeSpan? LunchStart { get; init; }
        public TimeSpan? LunchEnd { get; init; }
        public bool AllowAppointmentsDuringLunch { get; init; }
        public int AppointmentDurationMinutes { get; init; }
        public int MaxAppointmentsPerDay { get; init; }
        public bool WorksOnHolidays { get; init; }
        public bool FlexibleHours { get; init; }
        public string TimeZone { get; init; }
        public bool IsActive { get; init; } = true;
    }
}
