using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace BLL.Managers
{
    public class TopicManager : BaseManager
    {
        public TopicManager(CmsDbContext dbContext) : base(dbContext) {}

        public virtual async Task<IEnumerable<Topic>> GetAllTopicsAsync(string query, int page, int pageSize)
        {
            var topics = from u in dbContext.Topics
                         select u;

            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(u =>
                    u.Title.Contains(query) ||
                    u.Description.Contains(query));

            return await topics.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<Topic> GetTopicByIdAsync(int id)
        {
            return await dbContext.Topics.FirstOrDefaultAsync(u => u.Id == id);
        }

        public virtual async Task<IEnumerable<Topic>> GetTopicByDeadlineAsync(DateTime deadline)
        {
            var topics = from u in dbContext.Topics
                         select u;

            if (deadline != null)
            {
                topics = topics.Where(u => DateTime.Compare(u.Deadline, deadline) >= 0);
                return await topics.ToListAsync();
            }
            return Enumerable.Empty<Topic>();
        }

        public virtual async Task<int> GetTopicsCountAsync()
        {
            return await dbContext.Topics.CountAsync();
        }

        public virtual Object AddTopicAsync(TopicFormModel model)
        {
            Topic topic = new Topic(model);            
            try
            {
                dbContext.Topics.Add(topic);
                dbContext.SaveChanges();
                
                return topic;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public virtual async Task<object> UpdateTopicAsync(int id, TopicFormModel model)
        {
            //Finding whether the record exists as NoTracting, since we are tracking while updating and multiple tracking on same instance throws an error.
            if (dbContext.Topics.AsNoTracking().FirstOrDefault(u => u.Id == id) != null)
            {
                Topic topic = new Topic(model);
                topic.Id = id;
                dbContext.Topics.Attach(topic);
                dbContext.Entry(topic).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                            
                return topic;
            }
            return false;
        }

        public virtual bool DeleteTopicAsync(int id)
        {
            var topic = dbContext.Topics.FirstOrDefault(u => u.Id == id);

            if (topic != null)
            {
                try
                {
                    dbContext.Remove(topic);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

    }
}
