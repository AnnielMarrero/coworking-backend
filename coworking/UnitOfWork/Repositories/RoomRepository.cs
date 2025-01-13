using coworking.Data;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Repositories.Base;
using System.Linq.Expressions;

namespace coworking.UnitOfWork.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CoworkingManager context) : base(context)
        {
        }
        
    }
}
