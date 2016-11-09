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

        public virtual IEnumerable<AnnotationTagResult> getAllTags()
        {
            List<AnnotationTag> tags = dbContext.AnnotationTags
                .AsNoTracking().ToList<AnnotationTag>();
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
            var parent = dbContext.AnnotationTags.FirstOrDefault(t => t.Id == parentId);
            var child = dbContext.AnnotationTags.FirstOrDefault(t => t.Id == childId);

            if(parent != null && child != null)
            {
               
                    var list = new List<AnnotationTag>();
                    list.Add(child);
                    parent.ChildTags = list;
                
                child.ParentTag = parent;

                dbContext.Update(parent);
                dbContext.Update(child);

                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual List<AnnotationTag> getChildTagsOf(int id)
        {
            var parent = dbContext.AnnotationTags.AsNoTracking().FirstOrDefault(t => t.Id == id);
            if (parent != null)
            {
                return parent.ChildTags;
            }
            return new List<AnnotationTag>();
        }
    }
}
