using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models.Entity
{
    public class AnnotationTagInstance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TagModelId { get; set; }

        public AnnotationTag TagModel { get; set; }

        public List<TagRelation> Relations { get; set; }

        public List<TagRelation> IncomingRelations { get; set; }

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
                entityBuilder.HasOne(tag => tag.TagModel).WithMany(model => model.TagInstances)
                    .HasForeignKey(tag => tag.TagModelId).OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}