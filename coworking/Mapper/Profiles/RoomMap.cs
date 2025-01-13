using AutoMapper;
using coworking.Dtos;
using coworking.Dtos.Drones;
using coworking.Entities;


namespace coworking.Mapper.Profiles
{
    public class RoomMap : Profile
    {
        public RoomMap()
        {
            CreateMap<Room, CreateRoomDto>()
                .ReverseMap();

            CreateMap<Room, EditRoomDto>()
                .ReverseMap();

            CreateMap<Room, RoomDto>()
              //.ForMember(dto => dto.ModelDronText, opt => opt.MapFrom(e => e.DronModel.DisplayName()))
              //.ForMember(dto => dto.DronStateText, opt => opt.MapFrom(e => e.DronState.DisplayName()))
              .ReverseMap();

        }
    }
}
