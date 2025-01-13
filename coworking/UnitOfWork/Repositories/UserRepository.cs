using coworking.Data;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Repositories.Base;
using System.Linq.Expressions;

namespace coworking.UnitOfWork.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(CoworkingManager context) : base(context)
        {
        }
        //public override IQueryable<Dron> GetQuery(params Expression<Func<Dron, object>>[] includeProperties) => base.GetQuery(x => x.Medications);
    }
}
