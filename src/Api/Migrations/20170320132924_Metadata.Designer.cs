using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Api.Data;

namespace Api.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    [Migration("20170320132924_Metadata")]
    partial class Metadata
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTag", b =>
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

                    b.HasKey("Id");

                    b.HasIndex("ParentTagId");

                    b.ToTable("AnnotationTags");
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTagInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DocumentId");

                    b.Property<int>("IdInDocument");

                    b.Property<int>("PositionInDocument");

                    b.Property<int>("TagModelId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.HasIndex("TagModelId");

                    b.ToTable("AnnotationTagInstances");
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTagRelation", b =>
                {
                    b.Property<int>("FirstTagId");

                    b.Property<int>("SecondTagId");

                    b.Property<string>("ArrowStyle");

                    b.Property<string>("Color");

                    b.Property<string>("Description");

                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.Property<string>("Title");

                    b.HasKey("FirstTagId", "SecondTagId");

                    b.HasAlternateKey("Id");

                    b.HasIndex("SecondTagId");

                    b.ToTable("AnnotationTagRelations");
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.Layer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Layers");
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.LayerRelationRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ArrowStyle");

                    b.Property<string>("Color");

                    b.Property<string>("Description");

                    b.Property<int>("SourceLayerId");

                    b.Property<int>("TargetLayerId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("SourceLayerId");

                    b.HasIndex("TargetLayerId");

                    b.ToTable("LayerRelationRules");
                });

            modelBuilder.Entity("Api.Models.Entity.AssociatedTopic", b =>
                {
                    b.Property<int>("ParentTopicId");

                    b.Property<int>("ChildTopicId");

                    b.HasKey("ParentTopicId", "ChildTopicId");

                    b.HasIndex("ChildTopicId");

                    b.ToTable("AssociatedTopics");
                });

            modelBuilder.Entity("Api.Models.Entity.Document", b =>
                {
                    b.Property<int>("TopicId");

                    b.Property<string>("Content")
                        .HasMaxLength(65536);

                    b.Property<DateTime>("TimeStamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("UpdaterId");

                    b.HasKey("TopicId");

                    b.HasIndex("UpdaterId");

                    b.ToTable("Documents");
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

            modelBuilder.Entity("Api.Models.Entity.StudentDetails", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("CurrentDegree");

                    b.Property<short>("CurrentSemester");

                    b.Property<string>("Discipline");

                    b.HasKey("UserId");

                    b.ToTable("StudentDetails");
                });

            modelBuilder.Entity("Api.Models.Entity.Subscription", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SubscriberId");

                    b.Property<string>("TypeName");

                    b.HasKey("SubscriptionId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("Subscriptions");
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

            modelBuilder.Entity("Api.Models.Entity.TopicAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Path");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("TopicId");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TopicAttachments");
                });

            modelBuilder.Entity("Api.Models.Entity.TopicAttachmentMetadata", b =>
                {
                    b.Property<int>("TopicAttachmentId");

                    b.Property<string>("Copyright");

                    b.Property<string>("Creator");

                    b.Property<string>("Date");

                    b.Property<string>("Date2");

                    b.Property<int>("Depth");

                    b.Property<string>("DetailedPosition");

                    b.Property<string>("Details");

                    b.Property<int>("Height");

                    b.Property<string>("Location");

                    b.Property<string>("Material");

                    b.Property<int>("Page");

                    b.Property<string>("Photographer");

                    b.Property<string>("PlaceOfManufacture");

                    b.Property<string>("PointOfOrigin");

                    b.Property<string>("Signature");

                    b.Property<string>("Source");

                    b.Property<string>("SubType");

                    b.Property<DateTime>("TimeStamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Type");

                    b.Property<string>("Unit");

                    b.Property<int>("Width");

                    b.HasKey("TopicAttachmentId");

                    b.ToTable("TopicAttachmentMetadata");
                });

            modelBuilder.Entity("Api.Models.Entity.TopicReview", b =>
                {
                    b.Property<int>("TopicId");

                    b.Property<int>("ReviewerId");

                    b.Property<string>("Status");

                    b.Property<DateTime>("TimeStamp")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("TopicId", "ReviewerId");

                    b.HasIndex("ReviewerId");

                    b.ToTable("TopicReviews");
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

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTag", b =>
                {
                    b.HasOne("Api.Models.Entity.Annotation.AnnotationTag", "ParentTag")
                        .WithMany("ChildTags")
                        .HasForeignKey("ParentTagId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTagInstance", b =>
                {
                    b.HasOne("Api.Models.Entity.Document", "Document")
                        .WithMany("TagsInstances")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.Annotation.AnnotationTag", "TagModel")
                        .WithMany("TagInstances")
                        .HasForeignKey("TagModelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.AnnotationTagRelation", b =>
                {
                    b.HasOne("Api.Models.Entity.Annotation.AnnotationTag", "FirstTag")
                        .WithMany("TagRelations")
                        .HasForeignKey("FirstTagId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.Annotation.AnnotationTag", "SecondTag")
                        .WithMany("IncomingRelations")
                        .HasForeignKey("SecondTagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Annotation.LayerRelationRule", b =>
                {
                    b.HasOne("Api.Models.Entity.Annotation.Layer", "SourceLayer")
                        .WithMany("Relations")
                        .HasForeignKey("SourceLayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.Annotation.Layer", "TargetLayer")
                        .WithMany("IncomingRelations")
                        .HasForeignKey("TargetLayerId")
                        .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity("Api.Models.Entity.Document", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithOne("Document")
                        .HasForeignKey("Api.Models.Entity.Document", "TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User", "Updater")
                        .WithMany("Documents")
                        .HasForeignKey("UpdaterId")
                        .OnDelete(DeleteBehavior.SetNull);
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

            modelBuilder.Entity("Api.Models.Entity.StudentDetails", b =>
                {
                    b.HasOne("Api.Models.Entity.User", "User")
                        .WithOne("StudentDetails")
                        .HasForeignKey("Api.Models.Entity.StudentDetails", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Subscription", b =>
                {
                    b.HasOne("Api.Models.Entity.User", "Subscriber")
                        .WithMany("Subscriptions")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.Topic", b =>
                {
                    b.HasOne("Api.Models.Entity.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.TopicAttachment", b =>
                {
                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithMany("Attachments")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Api.Models.Entity.User", "User")
                        .WithMany("Attachments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Api.Models.Entity.TopicAttachmentMetadata", b =>
                {
                    b.HasOne("Api.Models.Entity.TopicAttachment", "TopicAttachment")
                        .WithOne("Metadata")
                        .HasForeignKey("Api.Models.Entity.TopicAttachmentMetadata", "TopicAttachmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Api.Models.Entity.TopicReview", b =>
                {
                    b.HasOne("Api.Models.Entity.User", "Reviewer")
                        .WithMany("Reviews")
                        .HasForeignKey("ReviewerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Api.Models.Entity.Topic", "Topic")
                        .WithMany("Reviews")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
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
