using System.ComponentModel.DataAnnotations;

namespace Api.Models.AnnotationTag
{
    public class RelationRuleFormModel
    {
        [Required]
        public int SourceLayerId { get; set; }

        [Required]
        public int TargetLayerId { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
