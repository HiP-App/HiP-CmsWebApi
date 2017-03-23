using Api.Data;
using Api.Models;
using Api.Models.AnnotationTag;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
(??)
using Api.Models.Entity.Annotation;
(??)

namespace Api.Managers
{
    public class AnnotationTagManager : BaseManager
    {
        public AnnotationTagManager(CmsDbContext dbContext) : base(dbContext) { }

        private bool TagRelationExists(Tag tag1, Tag tag2)
        {
            return tag1 != null &&
                tag2 != null &&
                DbContext.AnnotationTagRelations.Any(rel => rel.SourceTagId == tag1.Id && rel.TargetTagId == tag2.Id);
        }

        #region GET

        public IEnumerable<TagResult> GetAllTags(bool includeDeleted, bool includeOnlyRoot)
        {
            return DbContext
                .AnnotationTags
                .Include(t => t.TagInstances)
                .Where(t => !includeOnlyRoot || t.ParentTag == null)
                .Where(t => includeDeleted || !t.IsDeleted)
                .ToList()
                .Select(at => new TagResult(at));
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        internal TagResult GetTag(int id)
        {
            return new TagResult(DbContext.AnnotationTags.Single(t => t.Id == id));
        }

        public IEnumerable<TagResult> GetChildTagsOf(int id)
        {
            return DbContext.AnnotationTags.Where(at => at.ParentTagId == id).ToList().Select(at => new TagResult(at));
        }
        
        public IEnumerable<Layer> GetAllLayers()
        {
            return DbContext.Layers.ToList();
        }

        public IEnumerable<LayerRelationRule> GetAllLayerRelationRules()
        {
            return DbContext.LayerRelationRules.ToList();
        }

        public IEnumerable<AnnotationTag> GetAllowedRelationRulesForTag(int tagId)
        {
            var tag = DbContext.AnnotationTags.Single(t => t.Id == tagId);
            var rules = DbContext.LayerRelationRules.Where(r => r.SourceLayer.Name == tag.Layer);
            var tags = new List<AnnotationTag>();
            // add all tags that rules are allowed to
            foreach (var relationRule in rules)
            {
                tags.AddRange(DbContext.AnnotationTags.Where(
                    t => relationRule.TargetLayer.Name == t.Layer
                ));
            }
            return tags;
        }

        public IEnumerable<RelationResult> GetAllowedRelationsForTagInstance(int tagInstanceId)
        {
            var tagInstance = DbContext.AnnotationTagInstances.Single(t => t.Id == tagInstanceId);
            var relations = DbContext.AnnotationTagRelations.Where(rel => rel.SourceTagId == tagInstance.Id);
            // TODO: Also include relations that tagInstance is the *target* of?
            // TODO: Filter by document?
            var list = relations.ToList().Select(rel => new RelationResult(rel)).ToList();
            return list;
        }

        public IEnumerable<RelationResult> GetAllTagInstanceRelations()
        {
            return DbContext.AnnotationTagRelations.ToList().Select(rel => new RelationResult(rel)).ToList();
        }

        public IEnumerable<RelationResult> GetAllTagInstanceRelations(int tagId)
        {
            return DbContext.AnnotationTagRelations.Where(tag => tag.Id == tagId).ToList()
                .Select(rel => new RelationResult(rel)).ToList();
        }

        #endregion

        #region Adding

        public EntityResult AddTag(TagFormModel tagModel)
        {
            var tag = new Tag(tagModel);

            DbContext.AnnotationTags.Add(tag);
            DbContext.SaveChanges();

            return EntityResult.Successfull(tag.Id);
        }

        public bool AddChildTag(int parentId, int childId)
        {
            if (parentId == childId)
                return false;
            try
            {
                var child = DbContext.AnnotationTags.Single(t => t.Id == childId);
                if (HasDuplicateParent(child, DbContext.AnnotationTags.Single(t => t.Id == parentId)))
                {
                    return false;
                } else if (DbContext.AnnotationTags.Any(t => t.Id == parentId))
                {
                    child.ParentTagId = parentId;
                    DbContext.Update(child);
                    DbContext.SaveChanges();
                    return true;
                }
            }
            catch (InvalidOperationException) { return false; }
            return false;
        }

        private bool HasDuplicateParent(Tag original, Tag check)
        {
            var duplicate = original.Id == check.Id;
            if (duplicate)
            {
                return true;
            }
            else
            {
                return check.ParentTagId != null && HasDuplicateParent(original, DbContext.AnnotationTags.Single(t => t.Id == check.ParentTagId));
            }
        }

        internal bool AddTagRelation(RelationFormModel model)
        {
            var tag1 = DbContext.AnnotationTagInstances.Single(tag => tag.Id == model.SourceId);
            var tag2 = DbContext.AnnotationTagInstances.Single(tag => tag.Id == model.TargetId);
            if (tag1 != null && tag2 != null)
            {
                var forwardRelation = new TagRelation(tag1, tag2, model.Title, model.ArrowStyle, model.Color);
                DbContext.AnnotationTagRelations.Add(forwardRelation);
                DbContext.SaveChanges();
                return true;
            } else
            {
                return false;
            }
        }

        internal bool AddTagInstance(int tagModelId)
        {
            Tag model = DbContext.AnnotationTags.Single(m => m.Id == tagModelId);
            TagInstance instance = new TagInstance(model);
            DbContext.AnnotationTagInstances.Add(instance);
            DbContext.SaveChanges();
            return true;
        }

        internal bool AddLayerRelationRule(RelationFormModel model)
        {
            if (!(DbContext.Layers.Any(l => l.Id == model.SourceId) || DbContext.Layers.Any(l => l.Id == model.TargetId)))
                return false;
            var rule = new LayerRelationRule()
            {
                SourceLayerId = model.SourceId,
                TargetLayerId= model.TargetId,
				Title = model.Title,
				Description = model.Description,
                Color = model.Color,
                ArrowStyle = model.ArrowStyle
            };
            DbContext.LayerRelationRules.Add(rule);
            DbContext.SaveChanges();
            Console.Write("added relation:" + model.SourceId + model.TargetId);
            return true;
        }

        public bool AddTagRelationRule(RelationFormModel model)
        {
            if (!(DbContext.AnnotationTags.Any(t => t.Id == model.SourceId) &&
                    DbContext.AnnotationTags.Any(t => t.Id == model.TargetId)))
            {
                return false;
            }
            var rule = new TagRelationRule()
            {
                SourceTagId = model.SourceId,
                TargetTagId = model.TargetId,
                Title = model.Title,
                Description = model.Description,
                Color = model.Color,
                ArrowStyle = model.ArrowStyle
            };
            DbContext.TagRelationRules.Add(rule);
            DbContext.SaveChanges();
            return true;
        }

        #endregion

        #region edit

        public bool EditTag(TagFormModel model, int id)
        {
            var tag = DbContext.AnnotationTags.First(t => t.Id == id);
            if (tag == null)
                return false;

            if (model.Description != null)
            {
                tag.Description = model.Description;
            }
            if (model.Layer != null)
            {
                tag.Layer = model.Layer;
            }
            if (model.Name != null)
            {
                tag.Name = model.Name;
            }
            if (model.ShortName != null)
            {
                tag.ShortName = model.ShortName;
            }
            if (model.Style != null)
            {
                tag.Style = model.Style;
            }
            DbContext.SaveChanges();
            return true;
        }


        internal bool ChangeLayerRelationRule(RelationFormModel original, RelationFormModel changed)
        {
            if (!(DbContext.Layers.Any(l => l.Id == original.SourceId) &&
                    DbContext.Layers.Any(l => l.Id == original.TargetId)))
                return false;

            var rule = DbContext.LayerRelationRules.Single(
                r => r.SourceLayerId == original.SourceId &&
                r.TargetLayerId == original.TargetId &&
                r.Title == original.Title);
            rule.Title = changed.Title;
            rule.Description = changed.Description;
            rule.ArrowStyle = changed.ArrowStyle;
            rule.Color = changed.Color;
            DbContext.SaveChanges();
            return true;
        }
        public bool ChangeTagRelationRule(RelationFormModel original, RelationFormModel changed)
        {
            if (!(DbContext.AnnotationTags.Any(l => l.Id == original.SourceId) &&
                    DbContext.AnnotationTags.Any(l => l.Id == original.TargetId)))
                return false;

            var rule = DbContext.TagRelationRules.Single(EqualsTagRelationRule(original));
            rule.Title = changed.Title;
            rule.Description = changed.Description;
            rule.ArrowStyle = changed.ArrowStyle;
            rule.Color = changed.Color;
            DbContext.SaveChanges();
            return true;
        }

        public bool ChangeTagRelation(RelationFormModel original, RelationFormModel changed)
        {
            if (!(DbContext.AnnotationTagInstances.Any(l => l.Id == original.SourceId) &&
                    DbContext.AnnotationTagInstances.Any(l => l.Id == original.TargetId)))
                return false;

            var rule = DbContext.AnnotationTagRelations.Single(rel => true);
            rule.Title = changed.Title;
            rule.Description = changed.Description;
            rule.ArrowStyle = changed.ArrowStyle;
            rule.Color = changed.Color;
            DbContext.SaveChanges();
            return true;
        }

        #endregion

        #region delete / remove

        internal bool DeleteTag(int id)
        {
            try
            {
                var tag = DbContext.AnnotationTags.Include(t => t.ChildTags).Single(t => t.Id == id);
                if (tag.UsageCounter == 0)
                {
                    DbContext.AnnotationTags.Remove(tag);
                }
                else
                {
                    tag.IsDeleted = true;
                    DbContext.AnnotationTags.Update(tag);
                }
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal bool RemoveChildTag(int parentId, int childId)
        {
            var child = DbContext.AnnotationTags.First(t => t.Id == childId);
            if (child?.ParentTagId != parentId)
                return false;

            child.ParentTagId = null;
            DbContext.SaveChanges();
            return true;
        }

        internal bool RemoveTagRelation(RelationFormModel model)
        {
            var tag1 = DbContext.AnnotationTags.Single(tag => tag.Id == model.SourceId);
            var tag2 = DbContext.AnnotationTags.Single(tag => tag.Id == model.TargetId);
            if (TagRelationExists(tag1, tag2))
            {
                RemoveRelationFor(tag1, tag2);
                DbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RemoveRelationFor(Tag source, Tag target)
        {
            var relation = DbContext.AnnotationTagRelations.Single(rel => rel.SourceTagId == source.Id && rel.TargetTagId == target.Id);
            DbContext.AnnotationTagRelations.Remove(relation);
        }

        internal bool RemoveTagInstance(int tagInstanceid)
        {
            var instance = DbContext.AnnotationTagInstances.Single(i => i.Id == tagInstanceid);
            DbContext.AnnotationTagInstances.Remove(instance);
            DbContext.SaveChanges();
            return true;
        }

        internal bool RemoveLayerRelationRule(int sourceId, int targetId)
        {
            var entity = DbContext.LayerRelationRules.Single(r => r.SourceLayerId == sourceId && r.TargetLayerId == targetId);
            DbContext.Remove(entity);
            DbContext.SaveChanges();
            return true;
        }

        public bool RemoveTagRelationRule(RelationFormModel original)
        {
            var entity = DbContext.TagRelationRules.Single(EqualsTagRelationRule(original));
            DbContext.Remove(entity);
            DbContext.SaveChanges();
            return true;
        }

        #endregion

        private static Expression<Func<TagRelationRule, bool>> EqualsTagRelationRule(RelationFormModel original)
        {
            return r => r.SourceTagId == original.SourceId &&
                        r.TargetTagId == original.TargetId &&
                        r.Title == original.Title;
        }
    }

}
