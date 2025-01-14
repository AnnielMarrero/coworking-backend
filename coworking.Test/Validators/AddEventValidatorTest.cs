//using coworking.Application.Dtos.Events;
//using coworking.Application.Helpers.Events;
//using coworking.Data.Entities;
//using coworking.Data.IUnitOfWork.Interfaces;
//using FluentAssertions;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;

//namespace coworking.Application.Test.Validators
//{
//    public class AddEventValidatorTest
//    {
//        readonly CreateEventValidator addEventValidator;

//        readonly Mock<IBasicRepository<Brand>> brandRepo;
//        readonly Mock<IBasicRepository<City>> cityRepo;
//        readonly Mock<IBasicRepository<Unit>> unitRepo;

//        public AddEventValidatorTest()
//        {
//            brandRepo = new Mock<IBasicRepository<Brand>>();
//            cityRepo = new Mock<IBasicRepository<City>>();
//            unitRepo = new Mock<IBasicRepository<Unit>>();
//            addEventValidator = new CreateEventValidator(
//                cityRepo.Object,
//                brandRepo.Object,
//                unitRepo.Object
//            );
//        }

//        [Fact]
//        public void AddEvent_ThrowsValidationErrors_WhenNotMeetConditions()
//        {
//            //arreange
//            AddEventInputDto addDto =
//                new()
//                {
//                    Name = "",
//                    EstimatedPublic = -1,
//                    Address = "",
//                    StartDate = DateTime.Now,
//                    EndDate = DateTime.Now
//                };
//            //act
//            var result = addEventValidator.Validate(addDto);
//            //assert
//            result.Errors.Count.Should().BeGreaterThan(0);
//        }

//        [Fact]
//        public void AddEvent_IsOk_WhenMeetConditions()
//        {
//            //arreange
//            AddEventInputDto addDto =
//                new()
//                {
//                    Name = "any",
//                    EstimatedPublic = 10,
//                    Address = "any",
//                    StartDate = DateTime.Now,
//                    EndDate = DateTime.Now,
//                    BrandIds = new HashSet<Guid>() { Guid.NewGuid() }
//                };
//            brandRepo
//                .Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Brand, bool>>>()))
//                .ReturnsAsync(true);
//            cityRepo
//                .Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<City, bool>>>()))
//                .ReturnsAsync(true);
//            unitRepo
//                .Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Unit, bool>>>()))
//                .ReturnsAsync(true);

//            //act
//            var result = addEventValidator.Validate(addDto);
//            //assert
//            result.Errors.Count.Should().Be(0);
//        }
//    }
//}
