using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationTag
{
    public class RelationUpdateModel
    {
        [Required]
        public int SourceId { get; set; }

        [Required]
        public int TargetId { get; set; }

        [Required]
        public string Title { get; set; }

        public string NewTitle { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }

        public string Description { get; set; }
    }
}
