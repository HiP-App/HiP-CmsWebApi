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

        public DbSet<StudentTopic> StudentTopics { get; set; }

        public DbSet<SupervisorTopic> SupervisorTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasDiscriminator<string>("Role")
                .HasValue<Student>(Role.Student)
                .HasValue<Supervisor>(Role.Supervisor)
                .HasValue<Administrator>(Role.Administrator);

            // Relationship for a Every Topic could have a reviewer and a reviewer can review many topics.
            modelBuilder.Entity<Topic>()
                .HasOne(p => p.Reviewer)
                .WithMany(b => b.ReviewTopic)
                .HasForeignKey(p => p.ReviewerId);

            // TODO: Should replace the below code once EF core 1.0 updates for no intermediate table implementation & TPH.
            // Many to many relationship between Topic and Students.
            modelBuilder.Entity<StudentTopic>().HasKey(x => new { x.StudentId, x.TopicId });

            modelBuilder.Entity<StudentTopic>()
                .HasOne(pt => pt.Topic)
                .WithMany(p => p.Students)
                .HasForeignKey(pt => pt.TopicId);
            
            modelBuilder.Entity<StudentTopic>()
                .HasOne(pt => pt.Student)
                .WithMany(t => t.StudentTopics)
                .HasForeignKey(pt => pt.StudentId);

            // Many to many relationship between Topic and Supervisor.
            modelBuilder.Entity<SupervisorTopic>().HasKey(x => new { x.SupervisorId, x.TopicId });

            modelBuilder.Entity<SupervisorTopic>()
                .HasOne(pt => pt.Topic)
                .WithMany(p => p.Supervisors)
                .HasForeignKey(pt => pt.TopicId);

            modelBuilder.Entity<SupervisorTopic>()
                .HasOne(pt => pt.Supervisor)
                .WithMany(t => t.SupervisorTopics)
                .HasForeignKey(pt => pt.SupervisorId);

        }
    }
}
