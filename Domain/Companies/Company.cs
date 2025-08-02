using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Companies
{
    public sealed class Company : AggregateRoot
    {
        public CompanyId Id { get; private set; }
        public string Name { get; private set; }
        public string Slogan { get; set; }
        public string? Description { get; private set; }
        public List<string> CoverPhotoUrls { get; set; }
        public List<string> CompanyPhotos { get; private set; } = new();
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
            string slogan, 
            string? description, 
            List<string> coverPhotoUrls, 
            List<string> companyPhotos, 
            Address address, 
            PhoneNumber phoneNumber, 
            string? website, 
            WorkSchedule schedule, 
            bool worksOnHolidays, 
            bool flexibleHours, 
            string timeZone, 
            bool isActive)
        {
            Id = id;
            Name = name;
            Slogan = slogan;
            Description = description;
            CoverPhotoUrls = coverPhotoUrls;
            CompanyPhotos = companyPhotos;
            Address = address;
            PhoneNumber = phoneNumber;
            Website = website;
            Schedule = schedule;
            WorksOnHolidays = worksOnHolidays;
            FlexibleHours = flexibleHours;
            TimeZone = timeZone;
            IsActive = isActive;
        }

        public void UpdateDescription(string? description) => Description = description;
        public void UpdateWebsite(string? website) => Website = website;
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        public void SetPhotos(List<string> photos)
        {
            if (photos == null || !photos.Any())
                throw new ArgumentException("Photos cannot be null or empty.", nameof(photos));
            CompanyPhotos.AddRange(photos);
        }

        public void SetCoverPhotoUrls(List<string> coverPhotoUrls)
        {
            if (coverPhotoUrls == null || !coverPhotoUrls.Any())
                throw new ArgumentException("Cover photo URLs cannot be null or empty.", nameof(coverPhotoUrls));
            CoverPhotoUrls = coverPhotoUrls;
        }

        public void RaiseCompanyCreatedEvent()
        {
            //Raise(new CompanyCreatedEvent(Guid.NewGuid(), Id, Name, EconomicSector));
        }
    }
}
