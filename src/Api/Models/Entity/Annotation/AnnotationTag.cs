using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable CollectionNeverUpdated.Global

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation
{
    public class AnnotationTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Layer { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ShortName { get; set; }

        public int? ParentTagId { get; set; }

        public AnnotationTag ParentTag { get; set; }

        public List<AnnotationTag> ChildTags { get; set; }

        public string Style { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public bool IsDeleted { get; set; }

        public List<AnnotationTagInstance> TagInstances { get; set; }

        public List<AnnotationTagRelationRule> TagRelationRules { get; set; }

        public List<AnnotationTagRelationRule> IncomingTagRelationRules { get; set; }

        public AnnotationTag()
        {
        }

        public AnnotationTag(TagFormModel model)
        {
            Name = model.Name;
            ShortName = model.ShortName;
            Layer = model.Layer;
            Description = model.Description;
            Style = model.Style;

            TagInstances = new List<AnnotationTagInstance>();
            IsDeleted = false;
        }

        public string GetAbsoluteName()
        {
            return (ParentTag == null)
                ? Layer + "_" + ShortName
                : ParentTag.ShortName + "-" + ShortName;
        }

        public int UsageCounter => TagInstances?.Count ?? 0;

        public static void ConfigureModel(EntityTypeBuilder<AnnotationTag> entityBuilder)
        {
            entityBuilder.HasOne(at => at.ParentTag)
                .WithMany(pt => pt.ChildTags)
                .HasForeignKey(at => at.ParentTagId)
                .OnDelete(DeleteBehavior.SetNull);
            entityBuilder.HasMany(at => at.TagInstances).WithOne(ati => ati.TagModel);
        }
    }
}