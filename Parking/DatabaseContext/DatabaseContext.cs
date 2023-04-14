using Microsoft.EntityFrameworkCore;
using ParkingApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ParkingApp.DatabaseContext
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ParkingSlot> ParkingSlots { get; set; } = null!;
        public DbSet<FixedSlot> FixedSlots { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DatabaseContext() : base() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
