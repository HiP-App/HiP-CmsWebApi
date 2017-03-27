using Api.Models.Entity.Annotation;

namespace Api.Models.AnnotationTag
{
    /// <summary>
    /// Generic result type for all relations, i.e. TagRelationRule, LayerRelationRule, and TagRelation
    /// Information about the type of the contained relations should be represented by the webservice method that is called,
    /// e.g. if GetRelations is called, the returned TagRelationResults represent TagRelations.
    /// </summary>
    public class RelationResult: RelationFormModel
    {
        public RelationResult(AnnotationTagInstanceRelation rel)
            : base(rel.SourceTagId, rel.TargetTagId, rel)
        {
            SourceName = rel.SourceTag.TagModel.Name;
            TargetName = rel.TargetTag.TagModel.Name;
        }

        public RelationResult(AnnotationTagRelationRule rule)
            : base(rule.SourceTagId, rule.TargetTagId, rule)
        {
            SourceName = rule.SourceTag.Name;
            TargetName = rule.TargetTag.Name;
        }

        public RelationResult(LayerRelationRule rule)
            : base(rule.SourceLayerId, rule.TargetLayerId, rule)
        {
            SourceName = rule.SourceLayer.Name;
            TargetName = rule.TargetLayer.Name;
        }

        public string SourceName { get; set; }

        public string TargetName { get; set; }
    }
}
