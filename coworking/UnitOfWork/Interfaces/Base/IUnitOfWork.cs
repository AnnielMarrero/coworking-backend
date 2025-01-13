
namespace coworking.UnitOfWork.Interfaces.Base
{
    public interface IUnitOfWork : IDisposable
    {

        public IRoomRepository Rooms { get; }
        public IReservationRepository Reservations { get; }

        public ILogRepository Logs { get; }

        public IRolRepository Rols { get; }

        public IUserRepository Users { get; }

        public IRefreshTokenRepository RefreshTokens { get; }

        


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
