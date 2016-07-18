using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using AutoMapper;

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

        public virtual Object AddTopicAsync(TopicFormModel topic)
        {
            Topic _topic = new Topic();
            _topic.Title = topic.Title;
            _topic.Description = topic.Description;
            _topic.Deadline = topic.Deadline;
            _topic.Students = topic.Students;
            _topic.Supervisors = topic.Supervisors;
            _topic.Requirements = topic.Requirements;
            _topic.ReviewerId = topic.ReviewerId;
            _topic.Status = topic.Status;
            try
            {
                dbContext.Topics.Add(_topic);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public virtual async Task<bool> UpdateTopicAsync(int id, TopicFormModel topic)
        {
            var _topic = await GetTopicByIdAsync(id);


            if (_topic != null)
            {
                _topic.Title = topic.Title;
                _topic.Description = topic.Description;
                _topic.Deadline = topic.Deadline;
                _topic.Students = topic.Students;
                _topic.Supervisors = topic.Supervisors;
                _topic.ReviewerId = topic.ReviewerId;
                _topic.Requirements = topic.Requirements;
                _topic.Status = topic.Status;
                try
                {
                    dbContext.Entry(_topic).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                catch(Exception ex)
                {
                    return false;
                }                
                return true;
            }
            return false;
        }

        public virtual bool DeleteTopicAsync(int id)
        {
            var topic = dbContext.Topics.FirstOrDefaultAsync(u => u.Id == id);

            if(topic != null)
            {
                try
                {
                    dbContext.Remove(dbContext.Topics.FirstOrDefaultAsync(u => u.Id == id));
                    dbContext.SaveChanges();
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }                
            }

            return false;            
        }

    }
}
