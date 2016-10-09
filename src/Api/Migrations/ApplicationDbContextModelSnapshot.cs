using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BOL.Models.AssociatedTopic", b =>
                {
                    b.Property<int>("ParentTopicId");

                    b.Property<int>("ChildTopicId");

                    b.HasKey("ParentTopicId", "ChildTopicId");

                    b.HasIndex("ChildTopicId");

                    b.ToTable("AssociatedTopics");
                });

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("CreatedById");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Requirements");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("BOL.Models.TopicUser", b =>
                {
                    b.Property<int>("TopicId");

                    b.Property<int>("UserId");

                    b.Property<string>("Role");

                    b.HasKey("TopicId", "UserId", "Role");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicUsers");
                });

            modelBuilder.Entity("BOL.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("ProfilePicture");

                    b.Property<string>("Role")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Role").HasValue("User");
                });

            modelBuilder.Entity("BOL.Models.Administrator", b =>
                {
                    b.HasBaseType("BOL.Models.User");


                    b.ToTable("Administrator");

                    b.HasDiscriminator().HasValue("Administrator");
                });

            modelBuilder.Entity("BOL.Models.Student", b =>
                {
                    b.HasBaseType("BOL.Models.User");

                    b.Property<string>("MatriculationNumber");

                    b.ToTable("Student");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("BOL.Models.Supervisor", b =>
                {
                    b.HasBaseType("BOL.Models.User");


                    b.ToTable("Supervisor");

                    b.HasDiscriminator().HasValue("Supervisor");
                });

            modelBuilder.Entity("BOL.Models.AssociatedTopic", b =>
                {
                    b.HasOne("BOL.Models.Topic")
                        .WithMany("AssociatedTopics")
                        .HasForeignKey("ChildTopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.HasOne("BOL.Models.Supervisor", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BOL.Models.TopicUser", b =>
                {
                    b.HasOne("BOL.Models.Topic")
                        .WithMany("TopicUsers")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.User")
                        .WithMany("TopicUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
