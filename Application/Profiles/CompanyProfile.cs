using Application.Companies.Commands.CreateCompany;
using Application.Companies.DTOs;
using AutoMapper;
using Domain.Companies;
using Domain.ValueObjects;

namespace Application.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // Mapeo de CreateCompanyCommand a Company
            CreateMap<CreateCompanyCommand, Company>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => PhoneNumber.Create(src.PhoneNumber, src.CountryCode)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => Address.Create(src.Country, src.Department, src.City, src.StreetType, src.StreetNumber, src.CrossStreetNumber, src.PropertyNumber, src.ZipCode)))
                .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => WorkSchedule.Create(src.WorkingDays, src.OpeningHour, src.ClosingHour, src.LunchStart, src.LunchEnd, src.AllowAppointmentsDuringLunch, src.AppointmentDurationMinutes, src.MaxAppointmentsPerDay)));

            // Mapeo de Company a CustomerResponseDTO
            CreateMap<Company, CompanyResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value));
        }
    }
}
