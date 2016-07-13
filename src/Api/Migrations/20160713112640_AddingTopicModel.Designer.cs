using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160713112640_AddingTopicModel")]
    partial class AddingTopicModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.Property<int>("TopicId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Role")
                        .IsRequired();

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("TopicId");

                    b.ToTable("Topics");

                    b.HasDiscriminator<string>("Role").HasValue("Topic");
                });

            modelBuilder.Entity("BOL.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Role")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Role").HasValue("User");
                });

            modelBuilder.Entity("BOL.Models.UserTopic", b =>
                {
                    b.Property<int>("SupervisorId");

                    b.Property<int>("SupervisorTopicId");

                    b.Property<int>("StudentId");

                    b.Property<int>("StudentTopicId");

                    b.HasKey("SupervisorId", "SupervisorTopicId");

                    b.HasAlternateKey("StudentId", "StudentTopicId");

                    b.HasIndex("StudentId");

                    b.HasIndex("StudentTopicId");

                    b.HasIndex("SupervisorId");

                    b.HasIndex("SupervisorTopicId");

                    b.ToTable("UserTopics");
                });

            modelBuilder.Entity("BOL.Models.StudentTopic", b =>
                {
                    b.HasBaseType("BOL.Models.Topic");


                    b.ToTable("StudentTopic");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("BOL.Models.SupervisorTopic", b =>
                {
                    b.HasBaseType("BOL.Models.Topic");


                    b.ToTable("SupervisorTopic");

                    b.HasDiscriminator().HasValue("Supervisor");
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

            modelBuilder.Entity("BOL.Models.UserTopic", b =>
                {
                    b.HasOne("BOL.Models.Student", "Student")
                        .WithMany("StudentTopics")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.StudentTopic", "StudentTopic")
                        .WithMany("Students")
                        .HasForeignKey("StudentTopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.Supervisor", "Supervisor")
                        .WithMany("SupervisorTopics")
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.SupervisorTopic", "SupervisorTopic")
                        .WithMany("Supervisors")
                        .HasForeignKey("SupervisorTopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
