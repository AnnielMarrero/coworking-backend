using AutoMapper;
using coworking.Dtos;
using coworking.Dtos.Drones;
using coworking.Entities;


namespace coworking.Mapper.Profiles
{
    public class RolMap : Profile
    {
        public RolMap()
        {
           
            CreateMap<Rol, RolDto>()
              .ReverseMap();

        }
    }
}
