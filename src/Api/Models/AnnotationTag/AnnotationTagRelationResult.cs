using Api.Models.Entity.Annotation;

namespace Api.Models.AnnotationTag
{
    public class AnnotationTagRelationResult
    {
        public AnnotationTagRelationResult(AnnotationTagRelation relation)
        {
            Name = relation.Title;
            /*FirstTag = new AnnotationTagResult(relation.FirstTag);
            SecondTag = new AnnotationTagResult(relation.SecondTag);*/
        }

        public string Name { get; set; }

        public AnnotationTagResult FirstTag { get; set; }

        public AnnotationTagResult SecondTag { get; set; }
    
    }
}
