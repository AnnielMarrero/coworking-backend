//using coworking.Application.Controllers.v1;
//using coworking.Application.Dtos;
//using coworking.Application.Dtos.EventFiles;
//using coworking.Application.Dtos.Events;
//using coworking.Application.Test.Helpers;
//using coworking.Data.Entities;
//using coworking.Data.Entities.Enum;
//using coworking.Data.Helpers;
//using coworking.Domain.Interfaces;
//using AutoFixture;
//using FluentAssertions;
//using FluentValidation;
//using FluentValidation.Results;
//using Hangfire;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;

//namespace coworking.Application.Test.Controllers
//{
//    public class EventsControllerTest : BaseTest
//    {
//        private readonly EventsController _sut;
//        private readonly Mock<IEventBrandService> _eventBrandServiceMock;
//        private readonly Mock<IEventFileService> _eventFileServiceMock;
//        private readonly Mock<IEventService> _eventServiceMock;
//        private readonly Mock<IValidator<AddEventInputDto>> _addValidator;
//        private readonly Mock<IValidator<UpdateEventInputDto>> _editValidator;
//        private readonly Mock<IValidator<UploadEventFileDto>> _uploadEventFileValidator;

//        public EventsControllerTest()
//        {
//            _eventBrandServiceMock = new Mock<IEventBrandService>();
//            _eventServiceMock = _fixture.Freeze<Mock<IEventService>>();
//            _eventFileServiceMock = _fixture.Freeze<Mock<IEventFileService>>();
//            _addValidator = new Mock<IValidator<AddEventInputDto>>();
//            _editValidator = new Mock<IValidator<UpdateEventInputDto>>();
//            _uploadEventFileValidator = new Mock<IValidator<UploadEventFileDto>>();

//            _sut = new EventsController(
//                _eventBrandServiceMock.Object,
//                _mapper,
//                _backgroundJob.Object,
//                _eventServiceMock.Object,
//                _eventFileServiceMock.Object,
//                _httpContextAccessor.Object,
//                _addValidator.Object,
//                _editValidator.Object,
//                _uploadEventFileValidator.Object
//            );
//        }



//        [Fact]
//        public async Task RejectEvent_ShouldRetOK_WhenEventFound()
//        {
//            //Arreange
//            bool approval = false;
//            var response = _fixture.Create<Event>();
//            _eventServiceMock.Setup(_ => _.GetById(response.Id)).ReturnsAsync(response);
//            _eventServiceMock
//                .Setup(_ => _.Approval(response, approval))
//                .Callback(() => response.Status = Status.Rejected);

//            //Act
//            var result = await _sut.Approval(response.Id, new ApprovalDto { Status = approval });
//            var okResult = result as OkResult;

//            //Assert
//            //empty body
//            okResult.Should().NotBeNull();
//            okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);

//            //EventDto eventDto = (EventDto)okResult!.Value!;
//            //eventDto.Status.Should().Be(Status.Rejected);
//            _eventServiceMock.Verify(_ => _.GetById(response.Id), Times.Once);
//        }

//        [Fact]
//        public async Task ApproveEvent_ShouldRetNotFound_WhenEventNotFound()
//        {
//            //Arreange
//            var eventId = _fixture.Create<Guid>();
//            Event? response = null;
//            _eventServiceMock.Setup(_ => _.GetById(eventId)).ReturnsAsync(response);

//            //Act
//            var result = await _sut.Approval(eventId, new ApprovalDto { Status = false });
//            var notfResult = result as NotFoundObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            notfResult.Should().BeAssignableTo<NotFoundObjectResult>();
//            notfResult.Should().NotBeNull();

//            string? msg = ((ResponseErrorDto)notfResult!.Value!).Details;
//            //msg.Should().Contain("encontrado.");
//            msg.Should().Be(coworkingMsg.NotFound);

//            _eventServiceMock.Verify(_ => _.GetById(eventId), Times.Once);
//        }

//        [Fact]
//        public async Task GetById_ShouldRetOK_WhenEventFound()
//        {
//            //Arreange
//            var response = _fixture.Create<Event>();
//            var responseDto = _mapper.Map<EventDto>(response);
//            _eventServiceMock.Setup(_ => _.GetById(response.Id)).ReturnsAsync(response);

//            //Act
//            var result = await _sut.GetById(response.Id);
//            var okResult = result as OkObjectResult;

//            //Assert
//            okResult.Should().BeAssignableTo<OkObjectResult>();
//            EventDto eventDto = (EventDto)okResult!.Value!;
//            eventDto.Should().BeEquivalentTo(responseDto);
//        }

//        [Fact]
//        public async Task GetById_ShouldRetNotFound_WhenEventNotFound()
//        {
//            //Arreange
//            Event? response = null;
//            Guid id = Guid.NewGuid();
//            _eventServiceMock.Setup(_ => _.GetById(id)).ReturnsAsync(response);

//            //Act
//            var result = await _sut.GetById(id);
//            var notfResult = result as NotFoundObjectResult;

//            //Assert
//            notfResult.Should().BeAssignableTo<NotFoundObjectResult>();
//            string? msg = ((ResponseErrorDto)notfResult!.Value!).Details;
//            msg.Should().Be(coworkingMsg.NotFound);

//            _eventServiceMock.Verify(_ => _.GetById(id), Times.Once);
//        }

//        [Fact]
//        public async Task Headlight_ShouldRetOk()
//        {
//            //Arreange
//            var response = _fixture.Create<ICollection<RegionalReport>>();
//            _eventServiceMock.Setup(_ => _.Report(null, null, null)).ReturnsAsync(response);

//            //Act
//            var result = await _sut.Reports(null, null, null);
//            var okResult = result as OkObjectResult;
//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            okResult.Should().BeAssignableTo<OkObjectResult>();
//            okResult.Should().NotBeNull();

//            var regionalReportDto = (RegionalReport)okResult!.Value!;
//            //regionalReportDto.Should().BeEquivalentTo(response);
//            regionalReportDto.Should().NotBeNull();

//            _eventServiceMock.Verify(_ => _.Report(null, null, null), Times.Once);
//        }

//        [Fact]
//        public async Task GetEventFiles_ShouldRetOk_WhenEventFileFound()
//        {
//            //Arreange
//            Guid eventId = Guid.NewGuid();
//            var evt = _fixture.Create<Event>();
//            _eventServiceMock.Setup(_ => _.GetById(eventId)).ReturnsAsync(evt);

//            //Act
//            var result = await _sut.Uploads(eventId);
//            var okResult = result as OkObjectResult;
//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            okResult.Should().BeAssignableTo<OkObjectResult>();

//            var eventFilesDto = (EventWithFilesDto)okResult!.Value!;
//            eventFilesDto.EventFiles
//                .Should()
//                .BeEquivalentTo(_mapper.Map<ICollection<EventFileDto>>(evt.EventFiles));
//        }

//        [Fact]
//        public async Task EditEvent_ShouldRetBadReq_WhenIdsNotMatch()
//        {
//            //Arreange
//            var id = Guid.NewGuid();
//            var requestDto = _fixture.Create<UpdateEventInputDto>();
//            requestDto.Id = Guid.NewGuid();
//            //Act
//            _editValidator
//                .Setup(_ => _.ValidateAsync(requestDto, default))
//                .ReturnsAsync(new ValidationResult());
//            var result = await _sut.Update(id, requestDto);
//            var badReqResult = result as BadRequestObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            badReqResult.Should().BeAssignableTo<BadRequestObjectResult>();
//            badReqResult.As<BadRequestObjectResult>().Value.Should().NotBeNull();

//            ResponseErrorDto reponseDto = (ResponseErrorDto)(badReqResult!.Value!);
//            var msg = reponseDto.Details!;
//            msg.Should().Be(coworkingMsg.IdsNotMatch);
//        }

//        [Fact]
//        public async Task EditEvent_ShouldRetNotFound_WhenEventNotFound()
//        {
//            //Arreange
//            var id = Guid.NewGuid();
//            var requestDto = _fixture.Create<UpdateEventInputDto>();
//            requestDto.Id = id;
//            _editValidator
//                .Setup(_ => _.ValidateAsync(requestDto, default))
//                .ReturnsAsync(new ValidationResult());
//            _eventServiceMock.Setup(_ => _.GetById(id)).ReturnsAsync((Event?)null);

//            //Act
//            var result = await _sut.Update(id, requestDto);
//            var notFoundResult = result as NotFoundObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            notFoundResult.Should().BeAssignableTo<NotFoundObjectResult>();
//            notFoundResult.As<NotFoundObjectResult>().Value.Should().NotBeNull();

//            ResponseErrorDto reponseDto = (ResponseErrorDto)(notFoundResult!.Value!);
//            var msg = reponseDto.Details;
//            msg.Should().Be(coworkingMsg.NotFound);
//        }

//        [Fact]
//        public async Task EditEvent_ShouldRetOk_WhenEventFound()
//        {
//            //Arreange
//            var id = Guid.NewGuid();
//            var evt = _fixture.Create<Event>();
//            evt.Id = id;
//            var requestDto = _fixture.Create<UpdateEventInputDto>();
//            requestDto.Id = id;
//            _editValidator
//                .Setup(_ => _.ValidateAsync(requestDto, default))
//                .ReturnsAsync(new ValidationResult());
//            _eventServiceMock.Setup(_ => _.GetById(id)).ReturnsAsync(evt);

//            await using var context = new coworkingDbMock().CreateDbContext();
//            EntityEntry<Event> response = await context.Events.AddAsync(evt);
//            _eventServiceMock
//                .Setup(_ => _.Update(It.Is<Event>(t => t.Id == evt.Id)))
//                .Returns(response);

//            //Act
//            var result = await _sut.Update(id, requestDto);
//            var okResult = result as OkObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            okResult.Should().BeAssignableTo<OkObjectResult>();
//            okResult.As<OkObjectResult>().Value.Should().NotBeNull();

//            var eventDtoRes = (EventDto)okResult!.Value!;
//            eventDtoRes.Should().BeEquivalentTo(_mapper.Map<EventDto>(response.Entity));
//        }

//        [Fact]
//        public async Task DeleteEvent_ShouldRetOk_WhenEventFound()
//        {
//            //Arreange
//            var id = Guid.NewGuid();
//            var evt = _fixture.Create<Event>();
//            evt.Id = id;
//            await using var context = new coworkingDbMock().CreateDbContext();
//            EntityEntry<Event> response = context.Events.Remove(evt);
//            _eventServiceMock.Setup(_ => _.Remove(id)).ReturnsAsync(response);

//            //Act
//            var result = await _sut.Remove(id);
//            var okResult = result as OkObjectResult;

//            //Assert
//            result.Should().NotBeNull();
//            result.Should().BeAssignableTo<ActionResult>();
//            okResult.Should().BeAssignableTo<OkObjectResult>();
//            okResult.As<OkObjectResult>().Value.Should().NotBeNull();

//            var eventDtoRes = (EventDto)okResult!.Value!;
//            eventDtoRes.Id.Should().Be(id);
//        }

//        [Fact]
//        public async Task PostEventFile_ShouldRetOk_WhenEventFound()
//        {
//            //Arreange

//            UploadEventFileDto uploadDto = new UploadEventFileDto { Type = "", File = null!, };
//            var evt = _fixture.Create<Event>();
//            _uploadEventFileValidator
//                .Setup(_ => _.ValidateAsync(uploadDto, default))
//                .ReturnsAsync(new ValidationResult());
//            _eventServiceMock.Setup(_ => _.GetById(evt.Id)).ReturnsAsync(evt);
//            var evtFile = _fixture.Create<EventFile>();
//            _eventFileServiceMock
//                .Setup(s => s.Upload(evt, uploadDto.Type, uploadDto.File))
//                .ReturnsAsync(evtFile);

//            //Act
//            var result = await _sut.Uploads(evt.Id, uploadDto);
//            var okCreatedResult = result as CreatedResult;
//            //Assert
//            okCreatedResult!.Value.Should().BeNull();
//            okCreatedResult.Location.Should().Be($"/v1/event-files/{evtFile.Id}");
//        }

//        //////[Fact]
//        //////public async Task EditEvent_ShouldRetOk_WhenValidInput()
//        //////{
//        //////    //Arreange
//        //////    var requestDto = _fixture.Create<UpdateEventInputDto>();
//        //////    requestDto.StartDate = DateTime.Now.ToString("dd/MM/yyyy");
//        //////    requestDto.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
//        //////    requestDto.State = "CE";
//        //////    requestDto.ValidationEmailArchiveId = null;
//        //////    requestDto.MobileContractId = null;
//        //////    requestDto.CivilFireServiceContractId = null;
//        //////    requestDto.ComplianceAreaAuthorizationId = null;
//        //////    requestDto.EventCharterId = null;
//        //////    requestDto.FireDepartmentInspectionReportId = null;
//        //////    requestDto.PrivateSecurityContractId = null;
//        //////    requestDto.SponsorshipAgreementId = null;
//        //////    var id = requestDto.Id;
//        //////    var response = _fixture.Create<Event>();
//        //////    var entity = _mapper.Map<Event>(requestDto);
//        //////    _baseServiceMock.Setup(s => s.GetById(requestDto.Id)).ReturnsAsync(response);
//        //////    _baseServiceMock.Setup(s => s.ValidateEdit(entity)).ReturnsAsync(true);
//        //////    var archiveRes = _fixture.Create<Archive>();
//        //////    //_archiveServiceMock.Setup(s => s.GetById(_fixture.Create<int>()))
//        //////    //    .ReturnsAsync(archiveRes);
//        //////    archiveRes.Event = entity;
//        //////    var dbSetMockArchive = MockDBSetArchive();
//        //////    EntityEntry<Archive> responseArchive = dbSetMockArchive.Object.Update(archiveRes);
//        //////    //_archiveServiceMock.Setup(s => s.Update(archiveRes))
//        //////    //   .Returns(responseArchive);
//        //////    var dbSetMock = MockDBSet();
//        //////    EntityEntry<Event> responseEntry = await dbSetMock.Object.Update(entity);
//        //////    _baseServiceMock.Setup(s => s.Update(entity)).Returns(responseEntry);

//        //////    //Act
//        //////    var result = await _sut.Update(id, requestDto);
//        //////    var okResult = result as OkObjectResult;

//        //////    //Assert
//        //////    result.Should().NotBeNull();
//        //////    result.Should().BeAssignableTo<ActionResult>();
//        //////    okResult.Should().BeAssignableTo<OkObjectResult>();
//        //////    okResult.As<OkObjectResult>().Value
//        //////        .Should().NotBeNull()
//        //////        .And.BeOfType(typeof(ResponseDto));

//        //////    _serviceMock.Verify(s => s.Update(entity), Times.Once);
//        //////}
//    }
//}
