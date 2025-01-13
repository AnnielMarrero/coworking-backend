using coworking.Data;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces;
using coworking.UnitOfWork.Repositories.Base;

namespace coworking.UnitOfWork.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(CoworkingManager context) : base(context)
        {
        }
    }

}
