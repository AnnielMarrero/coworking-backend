
using coworking.Entities;
using Microsoft.EntityFrameworkCore;

namespace coworking.Data
{
    public class CoworkingManager : DbContext
    {
        public CoworkingManager(DbContextOptions<CoworkingManager> options)
            : base(options) { }

        public DbSet<Reservation> Reservation { get; set; } = default!;

        public DbSet<Room> Room { get; set; } = default!;

        public DbSet<Log> Log { get; set; } = default!;

       
        public DbSet<User> User { get; set; } = default!;
        public DbSet<Rol> Rol { get; set; } = default!;

        public DbSet<RefreshToken> RefreshToken { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}