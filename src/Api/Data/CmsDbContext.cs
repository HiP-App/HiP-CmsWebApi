using BOL.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();
                
            modelBuilder.Entity<User>().HasDiscriminator<string>("Role")
                .HasValue<Student>("Student")
                .HasValue<Supervisor>("Supervisor")
                .HasValue<Administrator>("Administrator");
        }
    }
}
