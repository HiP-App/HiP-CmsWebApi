using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationTag
{
    public class LayerRelationRuleFormModel
    {
        [Required]
        public int SourceLayerId { get; set; }

        [Required]
        public int TargetLayerId { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }
    }
}
