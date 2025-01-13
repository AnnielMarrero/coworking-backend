
using coworking.Dtos.Base;
using coworking.Entities;

namespace coworking.Dtos
{
    public class ReservationDto : EntityBaseDto
    {
        public int RoomId { get; set; }
        public required string RoomLocation { get; set; }

        public DateTime Date { get; set; }


        public DateTime? CanceledAt { get; set; }

    }
}
