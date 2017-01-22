using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using Api.Managers;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models.Entity
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

        public int UsageCounter { get; set; }

        public bool IsDeleted { get; set; }

        public AnnotationTag() { }

        public AnnotationTag(AnnotationTagFormModel model)
        {
            Name = model.Name;
            ShortName = model.ShortName;
            Layer = model.Layer;
            Description = model.Description;
            Style = model.Style;

            UsageCounter = 0;
            IsDeleted = false;
        }

        #region Utily Methods

        public string getAbsoluteName()
        {
            if (ParentTag == null)
            {
                return Layer + "_" + ShortName;
            }
            return ParentTag.ShortName + "-" + ShortName;
        }

        #endregion

        public class AnnotationTagMap
        {
            public AnnotationTagMap(EntityTypeBuilder<AnnotationTag> entityBuilder)
            {
                entityBuilder.HasOne(at => at.ParentTag).WithMany(pt => pt.ChildTags)
                    .HasForeignKey(at => at.ParentTagId).OnDelete(DeleteBehavior.SetNull);
            }
        }
    }
}
