using Microsoft.EntityFrameworkCore;

namespace ParkingApp.DatabaseContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
