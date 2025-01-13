
using AutoMapper;
using AutoMapper.Internal;
using coworking.Mapper.Profiles;

namespace coworking.Mapper
{
    public static class MappingProfile
    {
        public static void AddAutoMappers(this IServiceCollection services, MapperConfigurationExpression cfg)
        {
            IMapper mapper = new MapperConfiguration(cfg).CreateMapper();
            services.AddSingleton(mapper);
        }
        public static MapperConfigurationExpression AddAutoMapper(this MapperConfigurationExpression cfg)
        {
            cfg.AddProfile<RoomMap>();
            cfg.AddProfile<ReservationMap>();
            cfg.AddProfile<RolMap>();
            cfg.AddProfile<UserMap>();
            cfg.Internal().MethodMappingEnabled = false; //for work .net 7, otherwise throw error
            return cfg;
        }
        public static MapperConfigurationExpression CreateExpression()
        {
            return new MapperConfigurationExpression();
        }

    }
}