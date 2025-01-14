//using coworking.Application.Dtos.Brands;
//using coworking.Application.Helpers;
//using coworking.Application.Test.Helpers;
//using coworking.Data.Entities;
//using coworking.Data.Helpers;
//using coworking.Data.IUnitOfWork.Interfaces;
//using FluentAssertions;
//using Moq;
//using System;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//namespace coworking.Application.Test.Validators
//{
//    public class EditBrandValidatorTest
//    {
//        readonly EditBrandValidator editBrandValidator;
//        readonly Mock<IBasicRepository<Brand>> basicRepository;

//        public EditBrandValidatorTest()
//        {
//            basicRepository = new Mock<IBasicRepository<Brand>>();
//            editBrandValidator = new EditBrandValidator(basicRepository.Object);
//        }

//        [Fact]
//        public void EditBrand_ThrowsValidationErrors_WhenNameIsEmpty()
//        {
//            //arreange
//            UpdateBrandInputDto b = new() { Name = "" };
//            //act
//            var result = editBrandValidator.Validate(b);
//            //assert
//            result.Errors[0]
//                .ToString()
//                .Should()
//                .Contain(coworkingMsg.NotEmpty[BaseTest.PropertyName.Length..]);
//        }

//        [Fact]
//        public async Task EditBrand_ThrowsValidationErrors_WhenNameExists()
//        {
//            //arreange
//            UpdateBrandInputDto b = new() { Id = Guid.NewGuid(), Name = "AA" };
//            basicRepository
//                .Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Brand, bool>>>()))
//                .ReturnsAsync(true);

//            //act
//            var result = await editBrandValidator.ValidateAsync(b);
//            //assert
//            result.Errors[0]
//                .ToString()
//                .Should()
//                .Contain(coworkingMsg.UniqueName);
//        }
//    }
//}
