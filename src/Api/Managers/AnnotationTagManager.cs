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
                .FirstOrDefault(t => t.Id == parentId);
            var child = dbContext.AnnotationTags
                .Include(t => t.ParentTag)
                    .ThenInclude(pt => pt.ChildTags)
                .FirstOrDefault(t => t.Id == childId);

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

        public virtual List<AnnotationTagResult> getChildTagsOf(int id)
        {
            var parent = dbContext.AnnotationTags.Include(t => t.ChildTags).FirstOrDefault(t => t.Id == id); 
            if (parent != null)
            {
                List<AnnotationTagResult> result = new List<AnnotationTagResult>();
                foreach (AnnotationTag child in parent.ChildTags) {
                    result.Add(new AnnotationTagResult(child));
                }
                return result;
            }
            return null;
        }

        internal bool DeleteTag(int id)
        {
            var tag = dbContext.AnnotationTags
                .Include(t => t.ChildTags)
                .FirstOrDefault(t => t.Id == id);


            if (tag != null)
            {
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

            var parent = dbContext.AnnotationTags
                .Include(t => t.ChildTags)
                .FirstOrDefault(t => t.Id == parentId);

            var child = parent.ChildTags.FirstOrDefault(t => t.Id == childId);
            
            if (child != null)
            {
                //reset parent
                child.ParentTag = null;
                //remove child
                success = parent.ChildTags.Remove(child);
                if(success)
                {
                    dbContext.SaveChanges();
                }
            }

            return success;
        }
    }
}
