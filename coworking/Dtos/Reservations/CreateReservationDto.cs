using coworking.Entities;

namespace coworking.Dtos
{
    public class CreateReservationDto
    {
        public int RoomId { get; set; }
        

        public DateTime Date { get; set; }

    }
}
