using Microsoft.EntityFrameworkCore;
using ParkingApp.Models;

namespace ParkingApp.DatabaseContext
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<FixedSlot> FixedSlots { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
