using coworking.Entities;


namespace coworking.Entities
{
    public class RefreshToken : BasicEntity
    {
        public required string RefreshTokenValue { get; set; }
        public bool Active { get; set; }
        public DateTime Expiration { get; set; }
        public bool Used { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
