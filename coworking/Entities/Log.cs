

namespace coworking.Entities
{
    public class Log : BasicEntity
    {
       public required string Description { get; set; }
        public required string Table { get; set; }

        public required string Element { get; set; }

        public required string UpdateElement { get; set; }
    }
}
