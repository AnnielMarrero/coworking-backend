using coworking.Controllers;
using coworking.Dtos;
using coworking.Test.Helpers;
using coworking.Entities;
using coworking.Domain.Interfaces;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using coworking.Dtos.Base;
using EntityFrameworkPaginateCore;
using System.Linq.Expressions;

namespace coworking.Test.Controllers
{
    public class RoomsControllerTest : BaseTest
    {
        private readonly RoomsController _sut;
        private readonly Mock<IRoomService> _serviceMock;
        //private readonly Mock<IValidator<AddBrandInputDto>> _addValidator;
        //private readonly Mock<IValidator<UpdateBrandInputDto>> _editValidator;

        public RoomsControllerTest()
        {
            _serviceMock = _fixture.Freeze<Mock<IRoomService>>();
            //_addValidator = new Mock<IValidator<AddBrandInputDto>>();
            //_editValidator = new Mock<IValidator<UpdateBrandInputDto>>();

            _sut = new RoomsController(
                _serviceMock.Object,
                _mapper,
                _httpContextAccessor.Object,
                _backgroundJob.Object
            );
        }

        [Fact]
        public async Task GetPaginatedList_ShouldRetOK_WhenDataFound()
        {
            //Arreange
            RoomPagedInputDto roomPagedInputDto = _fixture.Create<RoomPagedInputDto>();
            roomPagedInputDto.Capacity = null;
            roomPagedInputDto.Locaction = null;
            roomPagedInputDto.IsAvailable = null;

            var pageRoom = _fixture.Create<Page<Room>>();
            
            _serviceMock
                .Setup(_ => _.GetPagedListAsync(roomPagedInputDto, new Expression<Func<Room, bool>>[0]))
                .ReturnsAsync(pageRoom);

            //Act
            var result = await _sut.GetAllPaginated(roomPagedInputDto);
            var okResult = result as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult>();
            okResult.Should().BeAssignableTo<OkObjectResult>();
            okResult.As<OkObjectResult>().Value.Should().NotBeNull();

            //PaginatedOutputDto<EventDto> paged = (PaginatedOutputDto<EventDto>)okResult!.Value!;
            //IEnumerable<EventDto> eventsDto = paged.Data;
            //eventsDto.Should().BeEquivalentTo(eventsMockDto);
            _serviceMock.Verify(
                _ => _.GetPagedListAsync(roomPagedInputDto),
                Times.Once
            );
        }

        [Fact]
        public async Task AddRoom_ShouldRetOK_WhenValidInput()
        {
            //Arreange
            var requestDto = _fixture.Create<CreateRoomDto>();
            var requestEntity = _mapper.Map<Room>(requestDto);
            await using var context = new CoworkingDbMock().CreateDbContext();

            EntityEntry<Room> response = await context.Room.AddAsync(requestEntity);
            _serviceMock
                .Setup(
                    s =>
                        s.AddAsync(
                            It.Is<Room>(
                                _ => _.Id >= 0
                            )
                        )
                )
                .ReturnsAsync(response);

            //ValidationResult vr = new();
            //_addValidator.Setup(s => s.ValidateAsync(requestDto, default)).ReturnsAsync(vr);

            //Act
            var result = await _sut.Create(requestDto);
            //Assert(resu)
            var createdResult = result as OkObjectResult;

            //Assert
            createdResult.StatusCode.Should().Be(200);
            createdResult.As<OkObjectResult>().Value
                .Should().NotBeNull()
                .And.BeOfType(typeof(ResponseDto));
        }

        [Fact]
        public async Task GetRoom_ShouldRetOk_WhenRoomFound()
        {
            //Arreange
            int roomId = 1;
            var room = _fixture.Create<Room>();
            _serviceMock.Setup(_ => _.FindByIdAsync(roomId)).ReturnsAsync(room);

            //Act
            var result = await _sut.FindById(roomId);
            var okResult = result as OkObjectResult;
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult>();
            okResult.Should().BeAssignableTo<OkObjectResult>();

            var res = (ResponseDto)okResult!.Value!;
            res.Result.Should().BeEquivalentTo(_mapper.Map<RoomDto>(room));
        }
    }
}
