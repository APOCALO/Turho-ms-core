using Application.Customers.Commands.CreateCustomer;
using Application.Customers.Commands.UpdateCustomer;
using Application.Customers.DTOs;
using AutoMapper;
using Domain.Customers;
using Domain.ValueObjects;

namespace Application.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Mapeo clave primaria
            //CreateMap<CustomerId, Guid>();

            // Mapeo de CreateCustomerCommand a Customer
            CreateMap<CreateCustomerCommand, Customer>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => PhoneNumber.Create(src.PhoneNumber, src.CountryCode)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => Address.Create(src.Country, src.Department, src.City, src.StreetType, src.StreetNumber, src.CrossStreetNumber, src.PropertyNumber, src.ZipCode)));

            // Mapeo de Customer a CustomerResponseDTO
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value));

            CreateMap<UpdateCustomerCommand, Customer>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => PhoneNumber.Create(src.PhoneNumber, src.CountryCode)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => Address.Create(src.Country, src.Department, src.City, src.StreetType, src.StreetNumber, src.CrossStreetNumber, src.PropertyNumber, src.ZipCode)));
        }
    }
}
