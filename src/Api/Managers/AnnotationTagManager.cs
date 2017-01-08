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
            // TODO: Catch circular dependency here & abort if necessary
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
                if (dbContext.AnnotationTags.Any(t => t.Id == parentId))
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

        #endregion
    }

}
