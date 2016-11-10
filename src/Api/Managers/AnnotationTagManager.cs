using Api.Data;
using Api.Models;
using Api.Models.AnnotationTag;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Managers
{
    public class AnnotationTagManager : BaseManager
    {
        public AnnotationTagManager(CmsDbContext dbContext) : base(dbContext) { }

        #region GET

        public virtual IEnumerable<AnnotationTagResult> getAllTags(bool IncludeDeleted)
        {
            List<AnnotationTag> tags;

            if (IncludeDeleted)
            {
                tags = dbContext.AnnotationTags
                    .AsNoTracking().ToList<AnnotationTag>();
            }
            else
            {
                tags = dbContext.AnnotationTags.AsNoTracking()
                    .Where(t => !t.IsDeleted).ToList();
            }


            List<AnnotationTagResult> result = new List<AnnotationTagResult>();

            foreach(AnnotationTag tag in tags)
            {
                result.Add(new AnnotationTagResult(tag));
            }

            return result;
        }

        internal AnnotationTagResult getTag(int Id)
        {
            if (TagExists(Id))
            {
                var tag = dbContext.AnnotationTags.AsNoTracking().First(t => t.Id == Id);
                var result = new AnnotationTagResult(tag);
                return result;
            }
            return null;
        }

        public virtual List<AnnotationTagResult> getChildTagsOf(int Id)
        {
            if (TagExists(Id))
            { 
                var parent = dbContext.AnnotationTags.AsNoTracking()
                    .Include(t => t.ChildTags).First(t => t.Id == Id);
                List<AnnotationTagResult> result = new List<AnnotationTagResult>();
                foreach (AnnotationTag child in parent.ChildTags)
                {
                    result.Add(new AnnotationTagResult(child));
                }
                return result;
            }
            return null;
        }

        #endregion

        #region Adding

        public virtual AddEntityResult AddTag(AnnotationTagFormModel tagModel)
        {
            AnnotationTag tag = new AnnotationTag(tagModel);

            dbContext.AnnotationTags.Add(tag);
            dbContext.SaveChanges();

            return new AddEntityResult() { Success = true, Value = tag.Id };
        }

        public virtual bool AddChildTag(int parentId, int childId)
        {
            if (parentId == childId)
                return false;

            var parent = dbContext.AnnotationTags
                .Include(t => t.ChildTags)
                .First(t => t.Id == parentId);
            var child = dbContext.AnnotationTags
                .Include(t => t.ParentTag)
                    .ThenInclude(pt => pt.ChildTags)
                .First(t => t.Id == childId);

            if(parent != null && child != null)
            {

                parent.ChildTags.Add(child);

                //remove child from old parent
                child.ParentTag.ChildTags.Remove(child);
                
                //set the new parent
                child.ParentTag = parent;

                dbContext.Update(parent);
                dbContext.Update(child);

                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion

        #region edit

        public virtual bool EditTag(AnnotationTagFormModel model, int Id)
        {
            if (TagExists(Id))
            {
                var tag = dbContext.AnnotationTags.First(t => t.Id == Id);
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
            if (TagExists(id))
            {
                var tag = dbContext.AnnotationTags
                    .Include(t => t.ChildTags)
                    .First(t => t.Id == id);

                if (tag.UsageCounter == 0)
                {
                    foreach (AnnotationTag child in tag.ChildTags)
                    {
                        child.ParentTag = null;
                    }
                    dbContext.AnnotationTags.Remove(tag);
                    dbContext.SaveChanges();
                }
                else
                {
                    tag.IsDeleted = true;
                    dbContext.SaveChanges();
                }
                return true;
            }
            return false;
            
                
        }

        internal bool RemoveChildTag(int parentId, int childId)
        {
            bool success = false;

            if (TagExists(parentId))
            {
                var parent = dbContext.AnnotationTags
                    .Include(t => t.ChildTags)
                    .First(t => t.Id == parentId);

                if (parent.ChildTags.Any(t => t.Id == childId))
                {
                    var child = parent.ChildTags.First(t => t.Id == childId);

                    //reset parent
                    child.ParentTag = null;
                    //remove child
                    success = parent.ChildTags.Remove(child);
                    if (success)
                    {
                        dbContext.SaveChanges();
                    }
                }
            }
            return success;
        }

        #endregion
        private bool TagExists(int Id)
        {
            return dbContext.AnnotationTags.AsNoTracking().Any(t => t.Id == Id);
        }
    }

}
