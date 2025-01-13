using System.Security;

namespace coworking.Entities
{
    public class Rol

        : BasicEntity
    {
        public required string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

        

    }
}
