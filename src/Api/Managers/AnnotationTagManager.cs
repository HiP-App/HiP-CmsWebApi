using Api.Data;
using Api.Models;
using Api.Models.AnnotationTag;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Managers
{
    public class AnnotationTagManager : BaseManager
    {
        public AnnotationTagManager(CmsDbContext dbContext) : base(dbContext) { }

        private bool TagRelationExists(AnnotationTagInstance tag1, AnnotationTagInstance tag2)
        {
            return tag1 != null &&
                tag2 != null &&
                dbContext.TagRelations.Any(rel => rel.FirstTagId == tag1.Id && rel.SecondTagId == tag2.Id);
        }

        #region GET

        public virtual IEnumerable<AnnotationTagResult> getAllTags(bool IncludeDeleted, bool IncludeOnlyRoot)
        {
            return dbContext
                .AnnotationTags
                .Where(t => !IncludeOnlyRoot || t.ParentTag == null)
                .Where(t => IncludeDeleted || !t.IsDeleted)
                .ToList()
                .Select(at => new AnnotationTagResult(at));
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        internal AnnotationTagResult getTag(int Id)
        {
            return new AnnotationTagResult(dbContext.AnnotationTags.Single(t => t.Id == Id));
        }

        public virtual IEnumerable<AnnotationTagResult> getChildTagsOf(int Id)
        {
            return dbContext.AnnotationTags.Where(at => at.ParentTagId == Id).ToList().Select(at => new AnnotationTagResult(at));
        }

        #endregion

        #region Adding

        public virtual EntityResult AddTag(AnnotationTagFormModel tagModel)
        {
            AnnotationTag tag = new AnnotationTag(tagModel);

            dbContext.AnnotationTags.Add(tag);
            dbContext.SaveChanges();

            return EntityResult.Successfull(tag.Id);
        }

        public virtual bool AddChildTag(int parentId, int childId)
        {
            if (parentId == childId)
                return false;
            try
            {
                var child = dbContext.AnnotationTags.Single(t => t.Id == childId);
                if (HasDuplicateParent(child, dbContext.AnnotationTags.Single(t => t.Id == parentId)))
                {
                    return false;
                } else if (dbContext.AnnotationTags.Any(t => t.Id == parentId))
                {
                    child.ParentTagId = parentId;
                    dbContext.Update(child);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (InvalidOperationException) { return false; }
            return false;
        }

        private bool HasDuplicateParent(AnnotationTag original, AnnotationTag check)
        {
            bool duplicate = original.Id == check.Id;
            if (duplicate)
            {
                return true;
            }
            else
            {
                return check.ParentTagId != null && HasDuplicateParent(original, dbContext.AnnotationTags.Single(t => t.Id == check.ParentTagId));
            }
        }

        internal bool AddTagRelation(int firstId, int secondId, string name)
        {
            AnnotationTagInstance tag1 = dbContext.AnnotationTagInstances.Single(tag => tag.Id == firstId);
            AnnotationTagInstance tag2 = dbContext.AnnotationTagInstances.Single(tag => tag.Id == secondId);
            if (TagRelationExists(tag1, tag2))
            {
                return false;
            } else if (tag1 != null && tag2 != null)
            {
                TagRelation forwardRelation = new TagRelation(tag1, tag2, name);
                dbContext.TagRelations.Add(forwardRelation);
                TagRelation backwardRelation = new TagRelation(tag2, tag1, name);
                dbContext.TagRelations.Add(backwardRelation);
                dbContext.SaveChanges();
                return true;
            } else
            {
                return false;
            }
        }

        #endregion

        #region edit

        public virtual bool EditTag(AnnotationTagFormModel model, int Id)
        {
            var tag = dbContext.AnnotationTags.First(t => t.Id == Id);
            if (tag != null)
            {
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
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion

        #region delete / remove

        internal bool DeleteTag(int id)
        {
            try
            {
                var tag = dbContext.AnnotationTags.Include(t => t.ChildTags).Single(t => t.Id == id);
                if (tag.UsageCounter == 0)
                {
                    dbContext.AnnotationTags.Remove(tag);
                }
                else
                {
                    tag.IsDeleted = true;
                    dbContext.AnnotationTags.Update(tag);
                }
                dbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        internal bool RemoveChildTag(int parentId, int childId)
        {
            var child = dbContext.AnnotationTags.First(t => t.Id == childId);
            if (child != null)
            {
                if (child.ParentTagId == parentId)
                {
                    child.ParentTagId = null;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        internal bool RemoveTagRelation(int firstId, int secondId)
        {
            AnnotationTagInstance tag1 = dbContext.AnnotationTagInstances.Single(tag => tag.Id == firstId);
            AnnotationTagInstance tag2 = dbContext.AnnotationTagInstances.Single(tag => tag.Id == secondId);
            if (TagRelationExists(tag1, tag2))
            {
                RemoveRelationFor(tag1, tag2);
                RemoveRelationFor(tag2, tag1);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RemoveRelationFor(AnnotationTagInstance tag1, AnnotationTagInstance tag2)
        {
            TagRelation relation = dbContext.TagRelations.Single(rel => rel.FirstTagId == tag1.Id && rel.SecondTagId == tag2.Id);
            dbContext.TagRelations.Remove(relation);
        }

        #endregion
    }

}
