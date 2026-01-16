using BEapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BEapplication.DBContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationDate)
                .HasColumnType("date");

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ReservationDate, r.ReservationHour });
        }
    }
}
