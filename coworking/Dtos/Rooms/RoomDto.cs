

using coworking.Entities;

namespace coworking.Dtos
{
    public class RoomDto : BasicEntity
    {
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }

        public required string Location { get; set; }

        public virtual ICollection<ReservationDto> Reservations { get; set; } = new HashSet<ReservationDto>();

    }
}
