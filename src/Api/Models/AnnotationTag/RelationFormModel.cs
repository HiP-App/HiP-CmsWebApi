namespace Api.Models.AnnotationTag
{
    /// <summary>
    /// Generic FormModel for annotation tag relations and annotation tag relation rules
    /// </summary>
    public class RelationFormModel
    {
        public int FirstTagId { get; set; }        

        public int SecondTagId { get; set; }

        public string Name { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }
    }
}
