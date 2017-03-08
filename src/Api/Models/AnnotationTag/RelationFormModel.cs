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
    }
}
