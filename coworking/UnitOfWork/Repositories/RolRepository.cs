using coworking.Data;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Repositories.Base;
using System.Linq.Expressions;

namespace coworking.UnitOfWork.Repositories
{
    public class RolRepository : BaseRepository<Rol>, IRolRepository
    {
        public RolRepository(CoworkingManager context) : base(context)
        {
        }
        
    }
}
