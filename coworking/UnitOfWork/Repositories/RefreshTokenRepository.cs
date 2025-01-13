
using coworking.Data;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Repositories.Base;

namespace coworking.UnitOfWork.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CoworkingManager context) : base(context)
        {
        }
    }
}
