using coworking.Data;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Interfaces.Base;

namespace coworking.UnitOfWork.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoworkingManager _context;


        public IRoomRepository Rooms { get; }
        public IReservationRepository Reservations { get; }
        public ILogRepository Logs { get; }

        public IRolRepository Rols { get; }

        public IUserRepository Users { get; }

        public IRefreshTokenRepository RefreshTokens { get; }

        

        public UnitOfWork(CoworkingManager context)
        {
            _context = context;
            Rooms = new RoomRepository(context);
            Reservations = new ReservationRepository(context);
            Logs = new LogRepository(context);
            Rols = new RolRepository(context);
            Users = new UserRepository(context);
            RefreshTokens = new RefreshTokenRepository(context);
            
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
