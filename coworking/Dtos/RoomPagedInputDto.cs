using coworking.Dtos.Base;

namespace coworking.Dtos
{
    public class RoomPagedInputDto : PagedListInputDto
    {
        public int? Capacity { get; set; }

        public string? Locaction { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
