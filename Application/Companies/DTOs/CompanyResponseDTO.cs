using Domain.ValueObjects;

namespace Application.Companies.DTOs
{
    public record CompanyResponseDTO
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Slogan { get; set; }
        public string? Description { get; private set; }
        public List<string> CoverPhotoUrls { get; private set; } = new();
        public Address Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Website { get; private set; }
        public WorkSchedule Schedule { get; private set; }
        public bool WorksOnHolidays { get; private set; }
        public bool FlexibleHours { get; private set; }
        public string TimeZone { get; private set; }
        public bool IsActive { get; private set; }
    }
}
