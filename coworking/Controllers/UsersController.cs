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
using coworking.Dtos.Users;
using coworking.Authorization;

namespace coworking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        
        //private readonly ILogger<UsersController> _logger;
        private readonly IUserService _UserService;
       
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _clientHangfire;
        private string username; 
        private readonly IJwtUtils _jwtUtils;

        //public UsersController(ILogger<UsersController> logger)
        //{
        //    _logger = logger;
        //}

        public UsersController(IUserService UserService, IMapper mapper, IHttpContextAccessor httpContext, IBackgroundJobClient clientHangfire, IJwtUtils jwtUtils)
        {
            _UserService = UserService;
            _mapper = mapper;
            //this.httpContext = httpContext;
            //this.clientHangfire = clientHangfire;
            username = "A"; // httpContext.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            _clientHangfire = clientHangfire;
            _jwtUtils = jwtUtils;
           
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
                Page<User> result = await _UserService.GetPagedListAsync(inputDto);
                PagedResultDto<UserDto> pagedResultDto = new()
                {
                    Results = _mapper.Map<List<UserDto>>(result.Results),
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
                var result = await _UserService.FindByIdAsync(id);
                if (result == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entityDto = _mapper.Map<UserDto>(result);

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
        public virtual async Task<IActionResult> Create([FromBody] CreateUserDto createDto)
        {
            try
            {
                createDto.Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
                var entity = _mapper.Map<User>(createDto);
                entity.CreatedBy = username;
                entity.UpdatedBy = username;
                //validate entity before add
                await _UserService.ValidateAdd(entity);

                
                
                EntityEntry<User> result = await _UserService.AddAsync(entity);
                await _UserService.SaveChangesAsync();
                var entityDto = _mapper.Map<UserDto>(result.Entity);

                //set history in backgroung job
                _clientHangfire.Enqueue<IUserService>(fire =>
                    fire.SetHistory(username, $"New element with id = {result.Entity.Id} was created at {typeof(User).Name}", typeof(User).Name, result.Entity, null));

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
        public virtual async Task<IActionResult> Update(int id, EditUserDto editDto)
        {
            try
            {
                if (id != editDto.Id)
                    return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = "Failed to update." });

                var originalEntity = await _UserService.FindByIdAsync(id);
                if (originalEntity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                var entity = _mapper.Map<User>(editDto);
                //entity.CreatedBy = originalEntity.CreatedBy;
                entity.CreatedAt = originalEntity.CreatedAt;
                entity.CreatedBy = username;
                entity.UpdatedBy = username;
                entity.Password = BCrypt.Net.BCrypt.HashPassword(editDto.Password);

                //validate entity before edit
                await _UserService.ValidateEdit(entity);

                EntityEntry<User> result = _UserService.Update(entity);
                await _UserService.SaveChangesAsync();

                var entityDto = _mapper.Map<UserDto>(result.Entity);

                //agregando tarea a la cola para ejecutarla en segundo plano
                _clientHangfire.Enqueue<IUserService>(basicService =>
                    basicService.SetHistory(username, $"Element with id = {id} was update at {typeof(User).Name}", typeof(User).Name, originalEntity, result.Entity));

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
                var entity = await _UserService.FindByIdAsync(id);
                if (entity == null)
                    return NotFound(new ResponseDto { Status = StatusCodes.Status404NotFound, ErrorMessage = "Elemento no encontrado." });

                //validate entity before remove
                await _UserService.ValidateRemove(id);

                EntityEntry<User>? result = _UserService.Delete(entity);
                await _UserService.SaveChangesAsync();

                var entityDto = _mapper.Map<UserDto>(result.Entity);

                //set history in backgroung job
                _clientHangfire.Enqueue<IUserService>(basicService =>
                basicService.SetHistory(
                                username,
                                $"Element with id = {id} was removed at {typeof(User).Name}",
                                typeof(User).Name,
                                result.Entity,
                                null));
                return Ok(new ResponseDto { Status = StatusCodes.Status200OK, Result = entityDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { Status = StatusCodes.Status400BadRequest, ErrorMessage = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Handle login for the api
        /// </summary>
        /// <param name="service"></param>
        /// <param name="jwtUtils"></param>
        /// <param name="mapper"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> HandleLogin( UserLoginDto userDto)
        {
            var user = await _UserService
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                
                return BadRequest(new ResponseDto
                {
                    Status = StatusCodes.Status400BadRequest,
                    ErrorMessage = "The provided credentials are incorrect"
                });
            }
            string accessToken = _jwtUtils.GenerateToken(user);
            string refreshToken = await _jwtUtils.GenerateRefreshToken(user);

            var userDtoResp = _mapper.Map<UserDto>(user);
            userDtoResp.AccessToken = accessToken;
            userDtoResp.RefreshToken = refreshToken;
            return Ok(userDtoResp);
        }

    }
}
