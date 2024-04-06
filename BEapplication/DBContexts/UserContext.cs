using BEapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BEapplication.DBContexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null;
    }
}
