﻿using BOL.Models;
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

        public DbSet<TopicUser> TopicUsers { get; set; }

        public DbSet<AssociatedTopic> AssociatedTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasDiscriminator<string>("Role")
                .HasValue<Student>(Role.Student)
                .HasValue<Supervisor>(Role.Supervisor)
                .HasValue<Administrator>(Role.Administrator);

            new TopicMap(modelBuilder.Entity<Topic>());
            new TopicUserMap(modelBuilder.Entity<TopicUser>());
            new AssociatedTopicMap(modelBuilder.Entity<AssociatedTopic>());
        }
    }
}
