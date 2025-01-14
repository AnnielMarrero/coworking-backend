using coworking.Test.Helpers;
using coworking.Entities;

using coworking.Helpers;
using coworking.Domain.Interfaces;
using coworking.Domain.Services;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using coworking.Dtos.Base;
using EntityFrameworkPaginateCore;

namespace coworking.Application.Test.Services
{
    /// <summary>
    /// autofixture only allows two mocks.
    /// Avoid include in messages special characteres like ã in validations text because fail in pipeline
    /// </summary>
    public class RoomServiceTest : BaseTest
    {
        readonly IRoomService _sut;
        

        public RoomServiceTest()
        {
            

            _sut = new RoomService(
                new UnitOfWorkMock().CreateUnitOfWork(),
                _httpContextAccessor.Object
            );
        }


        [Fact]
        public async Task GetPagedListAsync_ReturnItems_WhenIsCalled()
        {
            //Arreange
            var data = _fixture.Create<List<Room>>();
            //var mock = data.BuildMock().BuildMockDbSet();
            //_basicRepoMock.Setup(s => s.GetQuery()).Returns(mock.Object);

            //Act
            var result = await _sut.GetPagedListAsync(_fixture.Create<PagedListInputDto>());
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Page<Room>>();
        }
    }
}
