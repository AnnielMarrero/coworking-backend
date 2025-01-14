using coworking.Test.Helpers;
using coworking.Entities;
using coworking.Domain.Interfaces;
using coworking.Domain.Services;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using coworking.Dtos.Base;
using EntityFrameworkPaginateCore;

namespace coworking.Test.Services
{
    public class ReservationServiceTest : BaseTest
    {
        private readonly IReservationService _sut;

        public ReservationServiceTest()
        {
            _sut = new ReservationService(
                new UnitOfWorkMock().CreateUnitOfWork(),
                _httpContextAccessor.Object
            );
        }


        [Fact]
        public async Task GetPagedListAsync_ReturnItems_WhenIsCalled()
        {
            //Arreange
            var data = _fixture.Create<List<Reservation>>();
            //var mock = data.BuildMock().BuildMockDbSet();
            //_basicRepoMock.Setup(s => s.GetQuery()).Returns(mock.Object);

            //Act
            var result = await _sut.GetPagedListAsync(_fixture.Create<PagedListInputDto>());
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Page<Reservation>>();
        }
    }
}
