
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

    public class RoomService : BaseService<Room>, IRoomService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositories"></param>
        /// <param name="httpContext"></param>
        public RoomService(IUnitOfWork repositories, IHttpContextAccessor httpContext) : base(repositories, repositories.Rooms, httpContext)
        {

        }
        public override async Task<Page<Room>> GetPagedListAsync(PagedListInputDto inputDto, params Expression<Func<Room, bool>>[] filters)
        {
            IQueryable<Room> query = CreateQuery().Include(_ => _.Reservations);

            //apply custom filters, si son especificados en cada clase Servicio q sobrescribe este metodo
            query = filters.Aggregate(query, (current, filters) => current.Where(filters));


            //Paginating
            var result = await query.PaginateAsync(inputDto.Page ?? 1, inputDto.PageSize ?? inputDto.DefaultPageSize);
            return result;
        }
        /*
        public override async Task<bool> ValidateAdd(Room entity)
        {
            if ((await _baseRepository.CountAsync()) >= 10)
            {
                throw new Exception("The fleet is already filled");
            }
            return true;
        }

        public override async Task<bool> ValidateEdit(Dron entity)
        {
            if (entity.DronState != DronState.IDLE)
            {
                throw new Exception("Only drones with Idle state can be loaded with medications");
            }
            if (entity.Battery < 25)
            {
                throw new Exception("Dron can't be loaded because baterry is below 25%");
            }
            //validate the sum weigth
            if (entity.Medications.Sum(m => m.Weight) > entity.Weight)
                throw new Exception("The sum of the drugs cannot exceed the weight of the drone");
            return await Task.Run(() => true);
        }
        */
    }
}
