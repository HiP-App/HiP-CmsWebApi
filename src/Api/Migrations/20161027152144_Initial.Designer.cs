using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    [Migration("20161027152144_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("Api.Models.Entity.AssociatedTopic", b =>
                {
                    b.Property<int>("ParentTopicId");

                    b.Property<int>("ChildTopicId");

                    b.HasKey("ParentTopicId", "ChildTopicId");

                    b.HasIndex("ChildTopicId");

                    b.HasIndex("ParentTopicId");

                    b.ToTable("AssociatedTopics");
                });

            modelBuilder.Entity("Api.Models.Entity.Topic", b =>
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

            modelBuilder.Entity("Api.Models.Entity.TopicAttatchment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Legal");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Path");

                    b.Property<int>("TopicId");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicAttatchments");
                });

            modelBuilder.Entity("Api.Models.Entity.TopicUser", b =>
                {
                    b.Property<int>("TopicId");

                    b.Property<int>("UserId");

                    b.Property<string>("Role");

                    b.HasKey("TopicId", "UserId", "Role");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicUsers");
                });

            modelBuilder.Entity("Api.Models.Entity.User", b =>
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
                });

            modelBuilder.Entity("Api.Models.Entity.AssociatedTopic", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "ChildTopic")
                        .WithMany("AssociatedTopics")
                        .HasForeignKey("ChildTopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.Topic", "ParentTopic")
                        .WithMany("ParentTopics")
                        .HasForeignKey("ParentTopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Topic", b =>
                {
                    b.HasOne("Api.Models.Entity.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.TopicAttatchment", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithMany("Attatchments")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User", "User")
                        .WithMany("Attatchments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Api.Models.Entity.TopicUser", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithMany("TopicUsers")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User", "User")
                        .WithMany("TopicUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
        }
    }
}
