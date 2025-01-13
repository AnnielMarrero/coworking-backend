

using coworking.Dtos.Base;



namespace coworking.Dtos
{
    public class CreateRoomDto 
    {
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }

        public required string Location { get; set; }
    }
}
