using Microsoft.EntityFrameworkCore;
// using APISantuy.Models;



    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
    }

