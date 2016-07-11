using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160711155600_AddingTopicModel")]
    partial class AddingTopicModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BOL.Models.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Topic");
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
                    b.Property<int>("UserId");

                    b.Property<int>("TopicId");

                    b.Property<int?>("TopicId2");

                    b.HasKey("UserId", "TopicId");

                    b.HasIndex("TopicId");

                    b.HasIndex("TopicId2");

                    b.HasIndex("UserId");

                    b.ToTable("UserTopic");
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
                    b.HasOne("BOL.Models.Topic", "Topic")
                        .WithMany("Students")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BOL.Models.Topic")
                        .WithMany("Supervisors")
                        .HasForeignKey("TopicId2");

                    b.HasOne("BOL.Models.User", "User")
                        .WithMany("Topics")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
