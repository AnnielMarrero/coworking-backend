

namespace coworking.Entities
{
    public class Room : BasicEntity
    {
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }

        public required string Location { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
