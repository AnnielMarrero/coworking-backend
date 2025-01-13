using coworking.Dtos.Base;
using coworking.Entities;

namespace coworking.Dtos
{
    public class EditReservationDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        
        public DateTime Date { get; set; }


        public DateTime? CanceledAt { get; set; }

    }
}
