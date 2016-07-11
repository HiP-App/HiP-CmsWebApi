using BOL.Models;
using Microsoft.EntityFrameworkCore;

namespace BOL.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext() { }

        public CmsDbContext(DbContextOptions options) : base(options) { }

        // Add all Tables here
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasDiscriminator<string>("Role")
                .HasValue<Student>(Role.Student)
                .HasValue<Supervisor>(Role.Supervisor)
                .HasValue<Administrator>(Role.Administrator);

            modelBuilder.Entity<UserTopic>().HasKey(x => new { x.UserId, x.TopicId });

            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.Topic)
                .WithMany(p => p.Students)
                .HasForeignKey(pt => pt.TopicId);
            
            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.Topics)
                .HasForeignKey(pt => pt.UserId);


        }
    }
}
