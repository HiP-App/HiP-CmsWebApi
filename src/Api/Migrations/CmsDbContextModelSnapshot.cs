using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;
using Api.Models.Notifications;

namespace Api.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    partial class CmsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Api.Models.Entity.AnnotationTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Icon");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Layer")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("ParentTagId");

                    b.Property<string>("ShortName")
                        .IsRequired();

                    b.Property<string>("Style");

                    b.Property<int>("UsageCounter");

                    b.HasKey("Id");

                    b.HasIndex("ParentTagId");

                    b.ToTable("AnnotationTags");
                });

            modelBuilder.Entity("Api.Models.Entity.AssociatedTopic", b =>
                {
                    b.Property<int>("ParentTopicId");

                    b.Property<int>("ChildTopicId");

                    b.HasKey("ParentTopicId", "ChildTopicId");

                    b.HasIndex("ChildTopicId");

                    b.ToTable("AssociatedTopics");
                });

            modelBuilder.Entity("Api.Models.Entity.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.Property<bool>("IsRead")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("false");

                    b.Property<DateTime>("TimeStamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("TopicId");

                    b.Property<string>("TypeName");

                    b.Property<int>("UpdaterId");

                    b.Property<int>("UserId");

                    b.HasKey("NotificationId");

                    b.HasIndex("TopicId");

                    b.HasIndex("UpdaterId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Api.Models.Entity.Subscription", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SubscriberId");

                    b.Property<int>("Type");

                    b.Property<int?>("UserId");

                    b.HasKey("SubscriptionId");

                    b.HasIndex("SubscriberId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscription");
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
                        .ValueGeneratedOnAdd()
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

            modelBuilder.Entity("Api.Models.User.UserResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.ToTable("UserResult");
                });

            modelBuilder.Entity("Api.Models.Entity.AnnotationTag", b =>
                {
                    b.HasOne("Api.Models.Entity.AnnotationTag", "ParentTag")
                        .WithMany("ChildTags")
                        .HasForeignKey("ParentTagId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Api.Models.Entity.AssociatedTopic", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "ChildTopic")
                        .WithMany("ParentTopics")
                        .HasForeignKey("ChildTopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.Topic", "ParentTopic")
                        .WithMany("AssociatedTopics")
                        .HasForeignKey("ParentTopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Notification", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithMany()
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User", "Updater")
                        .WithMany("ProducedNotifications")
                        .HasForeignKey("UpdaterId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Api.Models.Entity.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Subscription", b =>
                {
                    b.HasOne("Api.Models.User.UserResult", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId");
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
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
