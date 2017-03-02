using Api.Data;
using Api.Models;
using Api.Models.AnnotationTag;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Api.Models.Entity.Annotation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Managers
{
    public class AnnotationTagManager : BaseManager
    {
        public AnnotationTagManager(CmsDbContext dbContext) : base(dbContext) { }

        private bool TagRelationExists(AnnotationTag tag1, AnnotationTag tag2)
        {
            return tag1 != null &&
                tag2 != null &&
                DbContext.AnnotationTagRelations.Any(rel => rel.FirstTagId == tag1.Id && rel.SecondTagId == tag2.Id);
        }

        #region GET

        public IEnumerable<AnnotationTagResult> GetAllTags(bool includeDeleted, bool includeOnlyRoot)
        {
            return DbContext
                .AnnotationTags
                .Where(t => !includeOnlyRoot || t.ParentTag == null)
                .Where(t => includeDeleted || !t.IsDeleted)
                .ToList()
                .Select(at => new AnnotationTagResult(at));
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        internal AnnotationTagResult GetTag(int id)
        {
            return new AnnotationTagResult(DbContext.AnnotationTags.Single(t => t.Id == id));
        }

        public IEnumerable<AnnotationTagResult> GetChildTagsOf(int id)
        {
            return DbContext.AnnotationTags.Where(at => at.ParentTagId == id).ToList().Select(at => new AnnotationTagResult(at));
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

        #endregion

        #region Adding

        public EntityResult AddTag(AnnotationTagFormModel tagModel)
        {
            var tag = new AnnotationTag(tagModel);

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

        private bool HasDuplicateParent(AnnotationTag original, AnnotationTag check)
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

        internal bool AddTagRelation(AnnotationTagRelationFormModel model)
        {
            var tag1 = DbContext.AnnotationTags.Single(tag => tag.Id == model.FirstTagId);
            var tag2 = DbContext.AnnotationTags.Single(tag => tag.Id == model.SecondTagId);
            if (TagRelationExists(tag1, tag2))
            {
                return false;
            } else if (tag1 != null && tag2 != null)
            {
                var forwardRelation = new AnnotationTagRelation(tag1, tag2, model.Name, model.ArrowStyle, model.Color);
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
            AnnotationTag model = DbContext.AnnotationTags.Single(m => m.Id == tagModelId);
            AnnotationTagInstance instance = new AnnotationTagInstance(model);
            DbContext.AnnotationTagInstances.Add(instance);
            DbContext.SaveChanges();
            return true;
        }

        internal bool AddLayerRelationRule(RelationRuleFormModel model)
        {
            if (!(DbContext.Layers.Any(l => l.Id == model.SourceLayerId) || DbContext.Layers.Any(l => l.Id == model.TargetLayerId)))
                return false;
            var rule = new LayerRelationRule()
            {
                SourceLayerId = model.SourceLayerId,
                TargetLayerId = model.TargetLayerId,
                Color = model.Color,
                ArrowStyle = model.ArrowStyle
            };
            DbContext.LayerRelationRules.Add(rule);
            DbContext.SaveChanges();
            Console.Write("added relation:" + model.SourceLayerId + model.TargetLayerId);
            return true;
        }

        public bool AddTagRelationRule(RelationRuleFormModel model)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region edit

        public bool EditTag(AnnotationTagFormModel model, int id)
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


        internal bool ChangeLayerRelationRule(int sourceId, int targetId, RelationRuleFormModel model)
        {
            if (!(DbContext.Layers.Any(l => l.Id == sourceId) || DbContext.Layers.Any(l => l.Id == targetId)))
                return false;

            var rule = DbContext.LayerRelationRules.Single(r => r.SourceLayerId == sourceId && r.TargetLayerId == targetId);
            rule.ArrowStyle = model.ArrowStyle;
            rule.Color = model.Color;
            DbContext.SaveChanges();
            Console.Write("changed relation:" + model.SourceLayerId + model.TargetLayerId);
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

        internal bool RemoveTagRelation(AnnotationTagRelationFormModel model)
        {
            var tag1 = DbContext.AnnotationTags.Single(tag => tag.Id == model.FirstTagId);
            var tag2 = DbContext.AnnotationTags.Single(tag => tag.Id == model.SecondTagId);
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

        private void RemoveRelationFor(AnnotationTag source, AnnotationTag target)
        {
            var relation = DbContext.AnnotationTagRelations.Single(rel => rel.FirstTagId == source.Id && rel.SecondTagId == target.Id);
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

        public bool RemoveTagRelationRule(RelationRuleFormModel model)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
