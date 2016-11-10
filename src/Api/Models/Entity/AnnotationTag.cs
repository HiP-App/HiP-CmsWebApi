using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using Api.Managers;
using System.ComponentModel.DataAnnotations.Schema;

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


        public AnnotationTag ParentTag { get; set; }

        [InverseProperty("ParentTag")]
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
            ChildTags = new List<AnnotationTag>();
            IsDeleted = false;
        }

        #region Utily Methods
           
        public string getAbsoluteName()
        {
            if(ParentTag == null)
            {
                return Layer + "_" + ShortName;
            }
            return ParentTag.ShortName + "-" + ShortName;
        }

        #endregion

    }
}
