
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
    public class ReservationService : BaseService<Reservation>, IReservationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositories"></param>
        /// <param name="httpContext"></param>
        public ReservationService(IUnitOfWork repositories, IHttpContextAccessor httpContext) : base(repositories, repositories.Reservations, httpContext)
        {

        }
        public override async Task<Page<Reservation>> GetPagedListAsync(PagedListInputDto inputDto, params Expression<Func<Reservation, bool>>[] filters)
        {
            IQueryable<Reservation> query = CreateQuery().Include(_ => _.Room);

            //apply custom filters, si son especificados en cada clase Servicio q sobrescribe este metodo
            query = filters.Aggregate(query, (current, filters) => current.Where(filters));


            //Paginating
            var result = await query.PaginateAsync(inputDto.Page ?? 1, inputDto.PageSize ?? inputDto.DefaultPageSize);
            return result;
        }
    }
}
