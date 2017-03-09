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
        public RelationResult(TagRelation rel)
            : base(rel.FirstTagId, rel.SecondTagId, rel)
        {
            SourceName = rel.FirstTag.TagModel.Name;
            TargetName = rel.SecondTag.TagModel.Name;
        }

        public RelationResult(TagRelationRule rule)
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
