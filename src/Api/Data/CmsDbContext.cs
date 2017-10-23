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

            AssociatedTopic.ConfigureModel(modelBuilder.Entity<AssociatedTopic>());
            Topic.ConfigureModel(modelBuilder.Entity<Topic>());
            TopicUser.ConfigureModel(modelBuilder.Entity<TopicUser>());
            TopicAttachment.ConfigureModel(modelBuilder.Entity<TopicAttachment>());
            Document.ConfigureModel(modelBuilder.Entity<Document>());
            Notification.ConfigureModel(modelBuilder.Entity<Notification>());
            AnnotationTagInstanceRelation.ConfigureModel(modelBuilder.Entity<AnnotationTagInstanceRelation>());
            AnnotationTag.ConfigureModel(modelBuilder.Entity<AnnotationTag>());
            Subscription.ConfigureModel(modelBuilder.Entity<Subscription>());
            AnnotationTagInstance.ConfigureModel(modelBuilder.Entity<AnnotationTagInstance>());
            Models.Entity.TopicAttachmentMetadata.ConfigureModel(modelBuilder.Entity<TopicAttachmentMetadata>());
            LayerRelationRule.ConfigureModel(modelBuilder.Entity<LayerRelationRule>());
            AnnotationTagRelationRule.ConfigureModel(modelBuilder.Entity<AnnotationTagRelationRule>());
            Models.Entity.StudentDetails.ConfigureModel(modelBuilder.Entity<StudentDetails>());
            TopicReview.ConfigureModel(modelBuilder.Entity<TopicReview>());
        }
    }
}
