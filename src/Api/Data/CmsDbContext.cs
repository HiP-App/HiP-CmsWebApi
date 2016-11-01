using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options) { }

        // Add all Tables here
        public DbSet<User> Users { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<TopicUser> TopicUsers { get; set; }

        public DbSet<AssociatedTopic> AssociatedTopics { get; set; }

        public DbSet<TopicAttatchment> TopicAttatchments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(b => b.Email).IsUnique();

            new AssociatedTopicMap(modelBuilder.Entity<AssociatedTopic>());
            new TopicMap(modelBuilder.Entity<Topic>());
            new TopicUserMap(modelBuilder.Entity<TopicUser>());
            new TopicAttatchmentMap(modelBuilder.Entity<TopicAttatchment>());
            new NotificationMap(modelBuilder.Entity<Notification>());
        }
    }
}
