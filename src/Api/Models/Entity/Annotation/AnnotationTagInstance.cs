using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation
{
    public class AnnotationTagInstance
    {
        private List<AnnotationTagInstanceRelation> _incomingRelations;
        private List<AnnotationTagInstanceRelation> _tagRelations;

        [Key]
        public int Id { get; set; }

        [Required]
        public int TagModelId { get; set; }

        public AnnotationTag TagModel { get; set; }

        public List<AnnotationTagInstanceRelation> TagRelations
        {
            get { return _tagRelations; }
            set { _tagRelations = value; }
        }

        public List<AnnotationTagInstanceRelation> IncomingRelations
        {
            get { return _incomingRelations; }
            set { _incomingRelations = value; }
        }

        public string Value { get; set; }

        public virtual Document Document { get; set; }

        public int DocumentId { get; set; }

        public int IdInDocument { get; set; }

        public int PositionInDocument { get; set; }

        public AnnotationTagInstance(AnnotationTag model)
        {
            TagModel = model;
            TagModelId = model.Id;
        }

        public AnnotationTagInstance()
        {
        }

        public class AnnotationTagInstanceMap
        {
            public AnnotationTagInstanceMap(EntityTypeBuilder<AnnotationTagInstance> entityBuilder)
            {
                entityBuilder.HasOne(tag => tag.TagModel)
                    .WithMany(model => model.TagInstances)
                    .HasForeignKey(tag => tag.TagModelId)
                    .OnDelete(DeleteBehavior.Cascade);
                entityBuilder.HasOne(tag => tag.Document)
                    .WithMany(doc => doc.TagsInstances)
                    .HasForeignKey(tag => tag.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}