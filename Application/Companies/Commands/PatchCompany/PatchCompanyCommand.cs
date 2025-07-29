using Application.Common;
using Application.Companies.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Companies.Commands.PatchCompany
{
    public record PatchCompanyCommand : BaseResponse<CompanyResponseDTO>
    {
        public Guid Id { get; init; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? NewCompanyPhotos { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; }
        public string? StreetType { get; set; }
        public string? StreetNumber { get; set; }
        public string? CrossStreetNumber { get; set; }
        public string? PropertyNumber { get; set; }
        public string? ZipCode { get; set; }
        public string? Website { get; set; }
        public required  IReadOnlyList<DayOfWeek>? WorkingDays { get; set; }
        public TimeSpan? OpeningHour { get; set; } = null;
        public TimeSpan? ClosingHour { get; set; } = null;
        public TimeSpan? LunchStart { get; set; } = null;
        public TimeSpan? LunchEnd { get; set; } = null;
        public bool? AllowAppointmentsDuringLunch { get; set; }
        public int? AppointmentDurationMinutes { get; set; }
        public bool? WorksOnHolidays { get; set; }
        public bool? FlexibleHours { get; set; }
        public string? TimeZone { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
