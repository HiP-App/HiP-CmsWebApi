using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationTag
{
    public class AnnotationTagRelationResult
    {
        public AnnotationTagRelationResult(AnnotationTagRelationRule relation)
        {
            Title = relation.Title;
            FirstTag = new TagResult(relation.SourceTag);
            SecondTag = new TagResult(relation.TargetTag);
        }

        public string Title { get; set; }

        public TagResult FirstTag { get; set; }

        public TagResult SecondTag { get; set; }
    
    }
}
