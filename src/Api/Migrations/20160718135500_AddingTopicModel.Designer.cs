using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160718135500_AddingTopicModel")]
    partial class AddingTopicModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BOL.Models.StudentTopic", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("TopicId");

                    b.HasKey("StudentId", "TopicId");

                    b.HasIndex("StudentId");

                    b.HasIndex("TopicId");

                    b.ToTable("StudentTopics");
                });

            modelBuilder.Entity("BOL.Models.SupervisorTopic", b =>
                {
                    b.Property<int>("SupervisorId");

                    b.Property<int>("TopicId");

                    b.HasKey("SupervisorId", "TopicId");

                    b.HasIndex("SupervisorId");

                    b.HasIndex("TopicId");

                    b.ToTable("SupervisorTopics");
                });

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Requirements");

                    b.Property<int>("ReviewerId");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ReviewerId");

                    b.ToTable("Topics");
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

            modelBuilder.Entity("BOL.Models.StudentTopic", b =>
                {
                    b.HasOne("BOL.Models.Student", "Student")
                        .WithMany("StudentTopics")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.Topic", "Topic")
                        .WithMany("Students")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BOL.Models.SupervisorTopic", b =>
                {
                    b.HasOne("BOL.Models.Supervisor", "Supervisor")
                        .WithMany("SupervisorTopics")
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.Topic", "Topic")
                        .WithMany("Supervisors")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.HasOne("BOL.Models.Supervisor", "Reviewer")
                        .WithMany("ReviewTopic")
                        .HasForeignKey("ReviewerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
