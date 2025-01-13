

using coworking.Entities;

namespace coworking.Dtos
{
    public class UserDto : BasicEntity
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }

        public required string Email { get; set; }

        public string? ProfileName { get; set; }

        public string? Description { get; set; }


        public required string Rol { get; set; } 

        public string? AccessToken { get; set; } //access_token

        public string? RefreshToken { get; set; }

    }
}
