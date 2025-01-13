using AutoMapper;
using coworking.Dtos;
using coworking.Dtos.Drones;
using coworking.Entities;


namespace coworking.Mapper.Profiles
{
    public class UserMap : Profile
    {
        public UserMap()
        {
            CreateMap<User, CreateUserDto>()
                .ReverseMap();

            CreateMap<User, EditUserDto>()
                .ReverseMap();

            CreateMap<User, UserDto>()
              .ForMember(dto => dto.Rol, opt => opt.MapFrom(e => e.Rol.Name))
              //.ForMember(dto => dto.DronStateText, opt => opt.MapFrom(e => e.DronState.DisplayName()))
              .ReverseMap();

        }
    }
}
