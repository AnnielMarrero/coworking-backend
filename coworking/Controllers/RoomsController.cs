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
using coworking.Dtos.Drones;
using System.Linq.Expressions;

namespace coworking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        
        //private readonly ILogger<RoomsController> _logger;
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _clientHangfire;
        private string username; 

       

        public RoomsController(IRoomService roomService, IMapper mapper, IHttpContextAccessor httpContext, IBackgroundJobClient clientHangfire)
        {
            _roomService = roomService;
            _mapper = mapper;
            //this.httpContext = httpContext;
            
            username = httpContext.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value ?? "";
            _clientHangfire = clientHangfire;

        }

        /// <summary>
        /// Gets the paginated list
        /// </summary>
        /// <param name="inputDto">Settings</param>
        /// <response code="200">Get paginated list</response>
        /// <response code="400">Bad Request. Get error message</response>
        [HttpGet("[action]")]
        public virtual async Task<IActionResult> GetAllPaginated([FromQuery] RoomPagedInputDto inputDto)
        {
            try
            {
                List<Expression<Func<Room, bool>>> filters = new List<Expression<Func<Room, bool>>>();
                if (inputDto.Capacity != null)
                    filters.Add( _ => _.Capacity == inputDto.Capacity );
                if (inputDto.Locaction != null)
                    filters.Add(_ => _.Location == inputDto.Locaction);
                if (inputDto.IsAvailable != null && inputDto.IsAvailable.Value)
                    filters.Add(_ => _.IsAvailable == inputDto.IsAvailable.Value);

                
                Page<Room> result = await _roomService.GetPagedListAsync(inputDto, filters.ToArray());
                PagedResultDto<RoomDto> pagedResultDto = new()
                {
                    Results = _mapper.Map<List<RoomDto>>(result.Results),
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
                var result = await _roomService.FindByIdAsync(id);
                if (result == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entityDto = _mapper.Map<RoomDto>(result);

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
        public virtual async Task<IActionResult> Create([FromBody] CreateRoomDto createDto)
        {
            try
            {
                var entity = _mapper.Map<Room>(createDto);
                //validate entity before add
                await _roomService.ValidateAdd(entity);

                EntityEntry<Room> result = await _roomService.AddAsync(entity);
                await _roomService.SaveChangesAsync();

                var entityDto = _mapper.Map<RoomDto>(result.Entity);

                //set history in backgroung job
               _clientHangfire.Enqueue<IRoomService>(fire =>
                    fire.SetHistory(username, $"New element with id = {result.Entity.Id} was created at {typeof(Room).Name}", typeof(Room).Name, result.Entity, null));

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
        public virtual async Task<IActionResult> Update(int id, EditRoomDto editDto)
        {
            try
            {
                if (id != editDto.Id)
                    return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = "Failed to update." });

                var originalEntity = await _roomService.FindByIdAsync(id);
                if (originalEntity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entity = _mapper.Map<Room>(editDto);
                //entity.CreatedBy = originalEntity.CreatedBy;
                entity.CreatedAt = originalEntity.CreatedAt;

                //validate entity before edit
                await _roomService.ValidateEdit(entity);

                EntityEntry<Room> result = _roomService.Update(entity);
                await _roomService.SaveChangesAsync();

                var entityDto = _mapper.Map<RoomDto>(result.Entity);

                //agregando tarea a la cola para ejecutarla en segundo plano
                _clientHangfire.Enqueue<IRoomService>(basicService =>
                    basicService.SetHistory(username, $"Element with id = {id} was update at {typeof(Room).Name}", typeof(Room).Name, originalEntity, result.Entity));

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
                var entity = await _roomService.FindByIdAsync(id);
                if (entity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                //validate entity before remove
                await _roomService.ValidateRemove(id);

                EntityEntry<Room>? result = _roomService.Delete(entity);
                await _roomService.SaveChangesAsync();

                var entityDto = _mapper.Map<RoomDto>(result.Entity);

                //set history in backgroung job
                _clientHangfire.Enqueue<IRoomService>(basicService =>
                basicService.SetHistory(
                                username,
                                $"Element with id = {id} was removed at {typeof(Room).Name}",
                                typeof(Room).Name,
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
