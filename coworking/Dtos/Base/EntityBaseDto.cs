namespace coworking.Dtos.Base
{
    public class EntityBaseDto
    {
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
