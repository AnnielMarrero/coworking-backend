using coworking.Domain.Interfaces;
using coworking.UnitOfWork.Repositories.Base;
//using coworking.Domain.Interfaces.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using FluentValidation.AspNetCore;
using coworking.Mapper;
using coworking.Mapper.Profiles;
using coworking.UnitOfWork.Interfaces.Base;

using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Hangfire.SqlServer;
using coworking.Data;
using coworking.Domain.Services;
using coworking.Domain.Services.Base;
using coworking.Authorization;
using System.Text;
using coworking.Helpers;
using Microsoft.IdentityModel.Tokens;
using coworking.Entities;
using coworking.Dtos;

namespace coworking
{
    public static class IocRegister
    {
        public static IServiceCollection AddRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.RegisterDataContext(configuration);
            services.RegisterServices();
            services.RegisterAuthentication(configuration);
            services.RegisterRepositories();
            services.RegisterDomainServices();
            services.RegisterSwagger();
            services.RegisterHangfire(configuration);

            return services;
        }


        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {

            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddAutoMappers(MappingProfile.CreateExpression().AddAutoMapper());

            services.AddHttpContextAccessor();

            services.AddCors();

            //Add services to validation
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<Program>();

            
            return services;
        }

        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                //validte token
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings").Get<AppSettings>()!.Secret)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                };
            });

            //adding polices or roles for use in each endpoint
            services.AddAuthorizationBuilder()
                .AddPolicy("admin_policy", policy => policy.RequireRole(RolEnum.ADMIN.ToString()))
                .AddPolicy("estandar_policy", policy => policy.RequireRole(RolEnum.ESTANDAR.ToString()));


        }


        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ConnectionStrings:CoworkingManager").Value;

            services.AddHangfire(configuration =>
            {
                IGlobalConfiguration<AutomaticRetryAttribute> globalConfiguration = configuration
                                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                    .UseSimpleAssemblyNameTypeSerializer()
                                    //.UseRecommendedSerializerSettings(settings => settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                                    .UseFilter(new AutomaticRetryAttribute { Attempts = 2 });

                globalConfiguration.UseStorage(
                    new SqlServerStorage(
                        connectionString,
                        new SqlServerStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(10),
                            PrepareSchemaIfNecessary = true
                        }
                    )
                );

            });
            services.AddHangfireServer();
        }

        private static void RegisterSwagger(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CoworkingManager API",
                    Description = "API for CoworkingManager system"
                });

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public static void AddRegistration(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
            ); // allow credentials

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();


            app.UseHangfireDashboard(options: new DashboardOptions
            {
                //Authorization = new[] { new HangfireAuthorizationFilter() }
            });
        }

        public static IServiceCollection RegisterDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStringSection = configuration.GetSection("ConnectionStrings:CoworkingManager");
            services.AddDbContext<CoworkingManager>(options =>
            {
                options.UseSqlServer(connectionStringSection.Value);
            });
            services.AddScoped<DataSeeder>();

            
            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            //services.AddScoped<IHistoryDBContext, HistoryDbContext>();
            services.AddScoped<IUnitOfWork, coworking.UnitOfWork.Repositories.Base.UnitOfWork>();

            return services;
        }

        public static IServiceCollection RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtUtils, JwtUtils>();
            
            return services;
        }
    }
}
