//using coworking.Application.Controllers.v1;
//using coworking.Application.Dtos.Units;
//using coworking.Application.Test.Helpers;
//using coworking.Data.Entities;
//using coworking.Domain.Interfaces;
//using AutoFixture;
//using FluentAssertions;
//using Hangfire;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace coworking.Application.Test.Controllers
//{
//    public class UnitsControllerTest : BaseTest
//    {
//        private readonly UnitsController _sut;
//        private readonly Mock<IUnitService> _serviceMock;

//        public UnitsControllerTest()
//        {
//            _serviceMock = _fixture.Freeze<Mock<IUnitService>>();

//            _sut = new UnitsController(
//                _mapper,
//                _serviceMock.Object,
//                _backgroundJob.Object,
//                _httpContextAccessor.Object
//            );
//        }

//        [Fact]
//        public async Task GetUnits_ShouldRetOK_WhenDataFound()
//        {
//            //Arreange
//            var UnitsMock = _fixture.Create<IEnumerable<Unit>>();
//            var UnitsMockDto = _mapper.Map<IEnumerable<UnitDto>>(UnitsMock);
//            _serviceMock.Setup(_ => _.GetAll()).ReturnsAsync(UnitsMock);

//            //Act
//            var result = await _sut.GetAll();
//            var okResult = result as OkObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            okResult.Should().BeAssignableTo<OkObjectResult>();
//            okResult.As<OkObjectResult>().Value.Should().NotBeNull();

//            IEnumerable<UnitDto> Units = (IEnumerable<UnitDto>)okResult!.Value!;
//            Units.Should().HaveCount(UnitsMockDto.Count());
//            Units.Should().BeEquivalentTo(UnitsMockDto);

//            _serviceMock.Verify(_ => _.GetAll(), Times.Once);
//        }
//    }
//}
