using coworking.Domain.Interfaces;
using coworking.Domain.Services.Base;
using coworking.Dtos.Base;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EntityFrameworkPaginateCore;
using coworking.Entities;
using coworking.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace coworking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        
        //private readonly ILogger<ReservationsController> _logger;
        private readonly IReservationService _reservationService;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _clientHangfire;
        private string username;

        //public ReservationsController(ILogger<ReservationsController> logger)
        //{
        //    _logger = logger;
        //}

        public ReservationsController(IReservationService ReservationService, IMapper mapper, IHttpContextAccessor httpContext, IBackgroundJobClient clientHangfire)
        {
            _reservationService = ReservationService;
            _mapper = mapper;
            //this.httpContext = httpContext;
            _clientHangfire = clientHangfire;
            username = "A"; //httpContext.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

        }

        /// <summary>
        /// Gets the paginated list
        /// </summary>
        /// <param name="inputDto">Settings</param>
        /// <response code="200">Get paginated list</response>
        /// <response code="400">Bad Request. Get error message</response>
        [HttpGet("[action]")]
        public virtual async Task<IActionResult> GetAllPaginated([FromQuery] PagedListInputDto inputDto)
        {
            try
            {
                Page<Reservation> result = await _reservationService.GetPagedListAsync(inputDto);
                PagedResultDto<ReservationDto> pagedResultDto = new()
                {
                    Results = _mapper.Map<List<ReservationDto>>(result.Results),
                    PageSize = result.PageSize,
                    CurrentPage = result.CurrentPage,
                    PageCount = result.PageCount,
                    RecordCount = result.RecordCount

                };
                return Ok(new ResponseDto { Status = StatusCodes.Status200OK, Result = pagedResultDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Gets the element by Id
        /// </summary>
        /// <param name="id">element Id</param>
        /// <response code="200">Get element</response>
        /// <response code="400">Bad Request. Get error message</response>
        /// <response code="404">Elemento não encontrado</response>
        [HttpGet("[action]/{id}")]
        public virtual async Task<IActionResult> FindById(int id)
        {
            try
            {
                var result = await _reservationService.FindByIdAsync(id);
                if (result == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entityDto = _mapper.Map<ReservationDto>(result);

                return Ok(new ResponseDto { Status = StatusCodes.Status200OK, Result = entityDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Add a new element
        /// </summary>
        /// <param name="createDto">element to add</param>
        /// <response code="200">Get created element</response>
        /// <response code="400">Bad Request. Get error message</response>

        [HttpPost("[action]")]
        [Authorize(Policy = "admin_policy")]
        public virtual async Task<IActionResult> Create([FromBody] CreateReservationDto createDto)
        {
            try
            {
                var entity = _mapper.Map<Reservation>(createDto);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
                entity.CreatedBy = username;
                entity.UpdatedBy = username;
                //validate entity before add
                await _reservationService.ValidateAdd(entity);

                EntityEntry<Reservation> result = await _reservationService.AddAsync(entity);
                await _reservationService.SaveChangesAsync();

                var entityDto = _mapper.Map<ReservationDto>(result.Entity);

                //set history in backgroung job
               _clientHangfire.Enqueue<IReservationService>(basicService =>
                    basicService.SetHistory(username, $"New element with id = {result.Entity.Id} was created at {typeof(Reservation).Name}", typeof(Reservation).Name, result.Entity, null));

                return Ok(new ResponseDto { Status = StatusCodes.Status201Created, Result = entityDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Update an element
        /// </summary>
        /// <param name="id">element id</param>
        /// <param name="editDto">element to update</param>
        /// <response code="200">Get updated element</response>
        /// <response code="400">Bad Request. Get error message</response>
        [HttpPut("[action]/{id}")]
        public virtual async Task<IActionResult> Update(int id, EditReservationDto editDto)
        {
            try
            {
                if (id != editDto.Id)
                    return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = "Failed to update." });

                var originalEntity = await _reservationService.FindByIdAsync(id);
                if (originalEntity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entity = _mapper.Map<Reservation>(editDto);
                entity.CreatedBy = originalEntity.CreatedBy;
                entity.CreatedAt = originalEntity.CreatedAt;

                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = username;

                //validate entity before edit
                await _reservationService.ValidateEdit(entity);

                EntityEntry<Reservation> result = _reservationService.Update(entity);
                await _reservationService.SaveChangesAsync();

                var entityDto = _mapper.Map<ReservationDto>(result.Entity);

                //agregando tarea a la cola para ejecutarla en segundo plano
               _clientHangfire.Enqueue<IReservationService>(basicService =>
                   basicService.SetHistory(username, $"Element with id = {id} was update at {typeof(Reservation).Name}", typeof(Reservation).Name, originalEntity, result.Entity));

                return Ok(new ResponseDto { Status = StatusCodes.Status200OK, Result = entityDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }
        /// <summary>
        /// Remove element
        /// </summary>
        /// <param name="id">element Id</param>
        /// <response code="200">Get removed element</response>
        /// <response code="400">Bad Request. Get error message</response>

        [HttpDelete("[action]/{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _reservationService.FindByIdAsync(id);
                if (entity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                //validate entity before remove
                await _reservationService.ValidateRemove(id);

                EntityEntry<Reservation>? result = _reservationService.Delete(entity);
                await _reservationService.SaveChangesAsync();

                var entityDto = _mapper.Map<ReservationDto>(result.Entity);

                //set history in backgroung job
                _clientHangfire.Enqueue<IReservationService>(basicService =>
                basicService.SetHistory(
                                username,
                                $"Element with id = {id} was removed at {typeof(Reservation).Name}",
                                typeof(Reservation).Name,
                                result.Entity,
                                null));
                return Ok(new ResponseDto { Status = StatusCodes.Status200OK, Result = entityDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }


    }
}
