using System.ComponentModel.DataAnnotations;

namespace Api.Models.AnnotationTag
{
    public class LayerRelationRuleFormModel
    {
        public LayerRelationRuleFormModel() {}

        [Required]
        public int SourceLayerId { get; set; }

        [Required]
        public int TargetLayerId { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }
    }
}
