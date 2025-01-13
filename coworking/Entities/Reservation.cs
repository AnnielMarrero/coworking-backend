

namespace coworking.Entities
{
    public class Reservation : BasicEntity
    {
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public DateTime Date { get; set; }


        public DateTime? CanceledAt { get; set; }

    }
}
