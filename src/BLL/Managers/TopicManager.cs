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

        public virtual async Task<IEnumerable<Topic>> GetAllTopicsAsync(string query, string status, DateTime deadline, int page, int pageSize)
        {
            var topics = from u in dbContext.Topics
                         select u;

            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(t =>
                    t.Title.Contains(query) ||
                    t.Description.Contains(query));
            
            if (!string.IsNullOrEmpty(status))
                topics = topics.Where(t => t.Status.CompareTo(status) == 0);

            if (deadline != null)
                topics = topics.Where(t => DateTime.Compare(t.Deadline, deadline) == 0);

            return await topics.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<Topic> GetTopicByIdAsync(int id)
        {
            return await dbContext.Topics.FirstOrDefaultAsync(u => u.Id == id);
        }

        public virtual async Task<int> GetTopicsCountAsync()
        {
            return await dbContext.Topics.CountAsync();
        }

        public virtual async Task<AddEntityResult> AddTopicAsync(int userId, TopicFormModel model)
        {     
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {       
                    var topic = new Topic(model);
                    topic.CreatedById = userId;
                    dbContext.Topics.Add(topic);
                    await dbContext.SaveChangesAsync();

                    // Add all associations
                    AssociateStudentsToTopic(topic.Id, model.Students);
                    AssociateSupervisorsToTopic(topic.Id, model.Supervisors);
                    AssociateReviewersToTopic(topic.Id, model.Reviewers);
                    AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                    await dbContext.SaveChangesAsync();

                    transaction.Commit();
                    return new AddEntityResult() { Success = true, Value = topic.Id };
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    return new AddEntityResult() { Success = false, ErrorMessage = e.Message };
                }
            }
        }

        public virtual async Task<bool> UpdateTopicAsync(int topicId, TopicFormModel model)
        {            
            if (await dbContext.Topics.AsNoTracking().FirstOrDefaultAsync(u => u.Id == topicId) != null)
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {       
                        var topic = new Topic(model);
                        topic.Id = topicId;
                        dbContext.Topics.Attach(topic);
                        dbContext.Entry(topic).State = EntityState.Modified;
                        
                        // Deassociated previous Links
                        DeassociateAllLinksFromTopic(topicId);
                        await dbContext.SaveChangesAsync();

                        // Add all associations
                        AssociateStudentsToTopic(topic.Id, model.Students);
                        AssociateSupervisorsToTopic(topic.Id, model.Supervisors);
                        AssociateReviewersToTopic(topic.Id, model.Reviewers);
                        AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                        await dbContext.SaveChangesAsync();

                        transaction.Commit();
                        return true;
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return false;
        }

        public virtual async Task<bool> DeleteTopicAsync(int topicId)
        {
            var topic = await dbContext.Topics.AsNoTracking().FirstOrDefaultAsync(u => u.Id == topicId);
            
            if (topic != null)
            {
                DeassociateAllLinksFromTopic(topicId);
                dbContext.Remove(topic);
                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        
        private void AssociateStudentsToTopic(int topicId, int[] studentIds)
        {
            if (studentIds != null)
            {
                foreach (int studentId in studentIds)
                {
                    dbContext.TopicUsers.Add(new TopicUser() 
                    { 
                        UserId = studentId, 
                        TopicId = topicId, 
                        Role = Role.Student 
                    });
                }
            }
        }

        private void AssociateSupervisorsToTopic(int topicId, int[] supervisorIds)
        {
            if (supervisorIds != null)
            {
                foreach (int supervisorId in supervisorIds)
                {
                    dbContext.TopicUsers.Add(new TopicUser() 
                    { 
                        UserId = supervisorId, 
                        TopicId = topicId, 
                        Role = Role.Supervisor 
                    });
                }
            }
        }

        private void AssociateReviewersToTopic(int topicId, int[] reviewerIds)
        {
            if (reviewerIds != null)
            {
                foreach (int reviewerId in reviewerIds)
                {
                    dbContext.TopicUsers.Add(new TopicUser() 
                    { 
                        UserId = reviewerId, 
                        TopicId = topicId, 
                        Role = Role.Reviewer 
                    });
                }
            }
        }

        private void AssociateTopicsToTopic(int topicId, int[] associatedTopicIds)
        {
            if (associatedTopicIds != null)
            {
                foreach (int associatedTopicId in associatedTopicIds)
                {
                    dbContext.AssociatedTopics.Add(new AssociatedTopic() 
                    { 
                        ChildTopicId = topicId, 
                        ParentTopicId = associatedTopicId
                    });
                }
            }
        }

        private void DeassociateAllLinksFromTopic(int topicId)
        {
            dbContext.TopicUsers.RemoveRange(dbContext.TopicUsers.Where(tu => tu.TopicId == topicId));
            dbContext.AssociatedTopics.RemoveRange(dbContext.AssociatedTopics.Where(tu => tu.ParentTopicId == topicId));
        }
    }
}