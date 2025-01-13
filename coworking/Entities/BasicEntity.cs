using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace coworking.Entities
{
    public class BasicEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public required string CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public required string UpdatedBy { get; set; }

        public BasicEntity() { 
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
