using coworking.Dtos.Base;

namespace coworking.Dtos
{
    public class EditUserDto 
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }

        public required string Email { get; set; }

        public string? ProfileName { get; set; }

        public string? Description { get; set; }

        public int RolId { get; set; }
    }
}
