using Application.Reservations.Commands.CreateReservation;
using Application.Reservations.Commands.UpdateReservation;
using Application.Reservations.DTOs;
using AutoMapper;
using Domain.Reservations;

namespace Application.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            // Mapeo de CreateReservationCommand a Reservation
            CreateMap<CreateReservationCommand, Reservation>();

            // Mapeo de Reservation a ReservationResponseDTO
            CreateMap<Reservation, ReservationResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.Value));

            CreateMap<UpdateReservationCommand, Reservation>();
                //.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => PhoneNumber.Create(src.PhoneNumber, src.CountryCode)))
                //.ForMember(dest => dest.Address, opt => opt.MapFrom(src => Address.Create(src.Country, src.Department, src.City, src.StreetType, src.StreetNumber, src.CrossStreetNumber, src.PropertyNumber, src.ZipCode)));
        }
    }
}
