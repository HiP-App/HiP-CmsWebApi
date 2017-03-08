using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Models.Entity.Annotation
{
    public class TagInstance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TagModelId { get; set; }

        public Tag TagModel { get; set; }

        public List<TagRelation> TagRelations { get; set; }

        public List<TagRelation> IncomingRelations { get; set; }

        public TagInstance(Tag model)
        {
            TagModel = model;
            TagModelId = model.Id;
        }

        public TagInstance()
        {
        }

        public class AnnotationTagInstanceMap
        {
            public AnnotationTagInstanceMap(EntityTypeBuilder<TagInstance> entityBuilder)
            {
                entityBuilder.HasOne(tag => tag.TagModel).WithMany(model => model.TagInstances)
                    .HasForeignKey(tag => tag.TagModelId).OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}