using Api.Models.Entity.Annotation;

namespace Api.Models.AnnotationTag
{
    /// <summary>
    /// Generic FormModel for annotation tag relations, annotation tag relation rules, and layer relation rules.
    /// </summary>
    public class RelationFormModel
    {
        public int SourceId { get; set; }        

        public int TargetId { get; set; }

        public string Title { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }

        public string Description { get; set; }

        public RelationFormModel() { }

        public RelationFormModel(int sourceId, int targetId, RelationRule relation)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Title = relation.Title;
            Description = relation.Description;
            Color = relation.Color;
            ArrowStyle = relation.ArrowStyle;
        }
    }
}
