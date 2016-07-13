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

        public DbSet<Topic> Topics { get; set; }

        public DbSet<UserTopic> UserTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasDiscriminator<string>("Role")
                .HasValue<Student>(Role.Student)
                .HasValue<Supervisor>(Role.Supervisor)
                .HasValue<Administrator>(Role.Administrator);

            modelBuilder.Entity<UserTopic>().HasKey(x => new { x.StudentId, x.StudentTopicId });

            modelBuilder.Entity<UserTopic>().HasKey(x => new { x.SupervisorId, x.SupervisorTopicId });

            modelBuilder.Entity<Topic>().HasDiscriminator<string>("Role")
                .HasValue<StudentTopic>(Role.Student)
                .HasValue<SupervisorTopic>(Role.Supervisor);

            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.Student)
                .WithMany(p => p.StudentTopics)
                .HasForeignKey(pt => pt.StudentId);

            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.Supervisor)
                .WithMany(p => p.SupervisorTopics)
                .HasForeignKey(pt => pt.SupervisorId);

            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.StudentTopic)
                .WithMany(p => p.Students)
                .HasForeignKey(pt => pt.StudentTopicId);

            modelBuilder.Entity<UserTopic>()
                .HasOne(pt => pt.SupervisorTopic)
                .WithMany(p => p.Supervisors)
                .HasForeignKey(pt => pt.SupervisorTopicId);
        }
    }
}
