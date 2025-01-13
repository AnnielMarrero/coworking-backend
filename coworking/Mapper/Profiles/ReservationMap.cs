
using AutoMapper;
using coworking.Dtos;
using coworking.Entities;

namespace coworking.Mapper.Profiles
{
    public class ReservationMap : Profile
    {
        public ReservationMap()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dto => dto.RoomLocation, opt => opt.MapFrom(e => e.Room.Location))
                .ReverseMap();

            CreateMap<Reservation, CreateReservationDto>()
                .ReverseMap();

            CreateMap<Reservation, EditReservationDto>()
               .ReverseMap();
        }

    }
}
