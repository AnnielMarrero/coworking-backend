using System.Security;

namespace coworking.Entities
{
    public class User

        : BasicEntity
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }

        public required string Email { get; set; }

        public string? ProfileName { get; set; }

        public string? Description { get; set; }

        public int RolId { get; set; }
        public virtual Rol Rol { get; set; } 

        

    }
}
