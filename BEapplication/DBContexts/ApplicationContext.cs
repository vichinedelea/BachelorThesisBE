using BEapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BEapplication.DBContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
         
        }

        public DbSet<User> Users { get; set; } = null;

        public DbSet<Reservation> Reservations { get; set; } = null;
    }
}
