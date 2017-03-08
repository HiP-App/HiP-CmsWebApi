using Api.Models.Entity.Annotation;

namespace Api.Models.AnnotationTag
{
    public class TagRelationResult
    {
        public TagRelationResult(TagRelation relation)
        {
            Name = relation.Title;
            /*FirstTag = new TagResult(relation.FirstTag);
            SecondTag = new TagResult(relation.SecondTag);*/
        }

        public string Name { get; set; }

        public TagResult FirstTag { get; set; }

        public TagResult SecondTag { get; set; }
    
    }
}
