using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ObjectCreationAsStatement

namespace PaderbornUniversity.SILab.Hip.CmsApi.Data
{
    public class CmsDbContext : DbContext
    {
        public CmsDbContext(DbContextOptions options) : base(options) { }

        // Add all Tables here
        public DbSet<Topic> Topics { get; set; }

        public DbSet<TopicUser> TopicUsers { get; set; }

        public DbSet<AssociatedTopic> AssociatedTopics { get; set; }

        public DbSet<TopicAttachment> TopicAttachments { get; set; }

        public DbSet<TopicAttachmentMetadata> TopicAttachmentMetadata { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<AnnotationTag> AnnotationTags { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<AnnotationTagInstanceRelation> AnnotationTagRelations { get; set; }

        public DbSet<AnnotationTagInstance> AnnotationTagInstances { get; set; }

        public DbSet<Layer> Layers { get; set; }

        public DbSet<LayerRelationRule> LayerRelationRules { get; set; }

        public DbSet<AnnotationTagRelationRule> TagRelationRules { get; set; }

        public DbSet<StudentDetails> StudentDetails { get; set; }

        public DbSet<TopicReview> TopicReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(b => b.Email).IsUnique();

            new AssociatedTopicMap(modelBuilder.Entity<AssociatedTopic>());
            new TopicMap(modelBuilder.Entity<Topic>());
            new TopicUserMap(modelBuilder.Entity<TopicUser>());
            new TopicAttachmentMap(modelBuilder.Entity<TopicAttachment>());
            new DocumentMap(modelBuilder.Entity<Document>());
            new NotificationMap(modelBuilder.Entity<Notification>());
            new AnnotationTagRelationMap(modelBuilder.Entity<AnnotationTagInstanceRelation>());
            new AnnotationTag.AnnotationTagMap(modelBuilder.Entity<AnnotationTag>());
            new SubscriptionMap(modelBuilder.Entity<Subscription>());
            new AnnotationTagInstance.AnnotationTagInstanceMap(modelBuilder.Entity<AnnotationTagInstance>());
            new TopicAttachmentMetadataMap(modelBuilder.Entity<TopicAttachmentMetadata>());
            new LayerRelationRule.LayerRelationRuleMap(modelBuilder.Entity<LayerRelationRule>());
            new LayerRelationRule.LayerRelationRuleMap(modelBuilder.Entity<LayerRelationRule>());
            new AnnotationTagRelationRule.TagRelationRuleMap(modelBuilder.Entity<AnnotationTagRelationRule>());
            new StudentDetailsMap(modelBuilder.Entity<StudentDetails>());
            new TopicReviewMap(modelBuilder.Entity<TopicReview>());
        }
    }
}
