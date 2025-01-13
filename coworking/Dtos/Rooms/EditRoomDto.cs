using coworking.Dtos.Base;

namespace coworking.Dtos.Drones
{
    public class EditRoomDto 
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }

        public required string Location { get; set; }
    }
}
