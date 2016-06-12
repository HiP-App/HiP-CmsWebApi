using BOL.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .HasIndex(b => b.Email)
                .IsUnique();
                
            modelBuilder.Entity<Member>().HasDiscriminator<string>("Role")
                .HasValue<Student>("Student")
                .HasValue<Supervisor>("Supervisor")
                .HasValue<Administrator>("Administrator");
        }
    }
}
