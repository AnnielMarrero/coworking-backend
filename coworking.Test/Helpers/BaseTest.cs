using coworking.Mapper;
using AutoFixture;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq;
using coworking.Mapper.Profiles;

namespace coworking.Test.Helpers
{
    /// <summary>
    /// TestMethodName_WhatShouldHappens_WhenScenario
    /// autofixture allows only two mocks
    /// </summary>
    public class BaseTest
    {
        public const string PropertyName = "{PropertyName} ";
        protected readonly IFixture _fixture;
        protected readonly IMapper _mapper;
        protected readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        protected readonly Mock<IBackgroundJobClient> _backgroundJob;

        public BaseTest()
        {
            _fixture = new Fixture();
            //omit circular reference, eg: recursion
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReservationMap>();
                cfg.AddProfile<RolMap>();
                cfg.AddProfile<RoomMap>();
                cfg.AddProfile<UserMap>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _httpContextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext());
            _backgroundJob = new Mock<IBackgroundJobClient>();
        }
    }
}
