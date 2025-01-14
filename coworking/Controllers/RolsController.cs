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
    public class RolsController : ControllerBase
    {
        
        //private readonly ILogger<RoomsController> _logger;
        private readonly IRolService _rolService;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _clientHangfire;
        private string username; 

        //public RoomsController(ILogger<RoomsController> logger)
        //{
        //    _logger = logger;
        //}

        public RolsController(IRolService rolService, IMapper mapper, IHttpContextAccessor httpContext, IBackgroundJobClient clientHangfire)
        {
            _rolService = rolService;
            _mapper = mapper;
            //this.httpContext = httpContext;
            //this.clientHangfire = clientHangfire;
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
        public virtual async Task<IActionResult> GetAllPaginated([FromQuery] PagedListInputDto inputDto)
        {
            try
            {
                Page<Rol> result = await _rolService.GetPagedListAsync(inputDto);
                PagedResultDto<RolDto> pagedResultDto = new()
                {
                    Results = _mapper.Map<List<RolDto>>(result.Results),
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



    }
}
