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
    public class ReservationsControllerTest : BaseTest
    {
        private readonly ReservationsController _sut;
        private readonly Mock<IReservationService> _serviceMock;
        //private readonly Mock<IValidator<AddBrandInputDto>> _addValidator;
        //private readonly Mock<IValidator<UpdateBrandInputDto>> _editValidator;

        public ReservationsControllerTest()
        {
            _serviceMock = _fixture.Freeze<Mock<IReservationService>>();
            //_addValidator = new Mock<IValidator<AddBrandInputDto>>();
            //_editValidator = new Mock<IValidator<UpdateBrandInputDto>>();

            _sut = new ReservationsController(
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
            var ReservationPagedInputDto = _fixture.Create<PagedListInputDto>();
            

            var pageReservation = _fixture.Create<Page<Reservation>>();
            
            _serviceMock
                .Setup(_ => _.GetPagedListAsync(ReservationPagedInputDto))
                .ReturnsAsync(pageReservation);

            //Act
            var result = await _sut.GetAllPaginated(ReservationPagedInputDto);
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
                _ => _.GetPagedListAsync(ReservationPagedInputDto),
                Times.Once
            );
        }

        [Fact]
        public async Task AddReservation_ShouldRetOK_WhenValidInput()
        {
            //Arreange
            var requestDto = _fixture.Create<CreateReservationDto>();
            var requestEntity = _mapper.Map<Reservation>(requestDto);
            await using var context = new CoworkingDbMock().CreateDbContext();

            EntityEntry<Reservation> response = await context.Reservation.AddAsync(requestEntity);
            _serviceMock
                .Setup(
                    s =>
                        s.AddAsync(
                            It.Is<Reservation>(
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
        public async Task GetReservation_ShouldRetOk_WhenReservationFound()
        {
            //Arreange
            int ReservationId = 1;
            var Reservation = _fixture.Create<Reservation>();
            _serviceMock.Setup(_ => _.FindByIdAsync(ReservationId)).ReturnsAsync(Reservation);

            //Act
            var result = await _sut.FindById(ReservationId);
            var okResult = result as OkObjectResult;
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult>();
            okResult.Should().BeAssignableTo<OkObjectResult>();

            var res = (ResponseDto)okResult!.Value!;
            res.Result.Should().BeEquivalentTo(_mapper.Map<ReservationDto>(Reservation));
        }
    }
}
