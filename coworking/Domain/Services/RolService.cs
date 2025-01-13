
using coworking.Domain.Interfaces;
using coworking.Domain.Services.Base;
using coworking.Dtos.Base;
using coworking.Entities;

using coworking.UnitOfWork.Interfaces.Base;
using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace coworking.Domain.Services
{

    public class RolService : BaseService<Rol>, IRolService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositories"></param>
        /// <param name="httpContext"></param>
        public RolService(IUnitOfWork repositories, IHttpContextAccessor httpContext) : base(repositories, repositories.Rols, httpContext)
        {

        }
        
    }
}
