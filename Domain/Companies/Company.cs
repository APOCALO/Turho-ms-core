using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Companies
{
    public sealed class Company : AggregateRoot
    {
        public CompanyId Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public List<string> CoverPhotoUrls { get; private set; } = new();
        public Address Address { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public string? Website { get; private set; }
        public WorkSchedule Schedule { get; private set; }
        public bool WorksOnHolidays { get; private set; }
        public bool FlexibleHours { get; private set; }
        public string TimeZone { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Constructor privado para EF
        private Company() { }

        public Company(
            CompanyId id,
            string name,
            string? description,
            List<string> coverPhotoUrls,
            Address address,
            PhoneNumber phoneNumber,
            string? website,
            WorkSchedule schedule,
            bool worksOnHolidays,
            bool flexibleHours,
            string timeZone)
        {
            Id = id;
            Name = name;
            Description = description;
            CoverPhotoUrls = coverPhotoUrls;
            Address = address;
            PhoneNumber = phoneNumber;
            Website = website;
            Schedule = schedule;
            WorksOnHolidays = worksOnHolidays;
            FlexibleHours = flexibleHours;
            TimeZone = timeZone;
        }

        public void UpdateDescription(string? description) => Description = description;
        public void UpdateWebsite(string? website) => Website = website;
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public void RaiseCompanyCreatedEvent()
        {
            //Raise(new CompanyCreatedEvent(Guid.NewGuid(), Id, Name, EconomicSector));
        }
    }
}
