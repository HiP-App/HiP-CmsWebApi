namespace Api.Models.AnnotationTag
{
    public class AnnotationTagRelationFormModel
    {
        public int FirstTagId { get; set; }        

        public int SecondTagId { get; set; }

        public string Name { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }
    }
}
