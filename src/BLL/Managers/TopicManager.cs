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

        public virtual async Task<IEnumerable<Topic>> GetAllTopicsAsync(string query, string status, DateTime? deadline, int page, int pageSize)
        {
            var topics = from t in dbContext.Topics
                         select t;

            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(t =>
                    t.Title.Contains(query) ||
                    t.Description.Contains(query));
            
            if (!string.IsNullOrEmpty(status))
                topics = topics.Where(t => t.Status.CompareTo(status) == 0);

            if (deadline != null)
                topics = topics.Where(t => DateTime.Compare(t.Deadline, (DateTime)deadline) == 0);

            return await topics.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<int> GetTopicsCountAsync()
        {
            return await dbContext.Topics.CountAsync();
        }

        public virtual async Task<Topic> GetTopicByIdAsync(int topicId)
        {
            return await dbContext.Topics.Include(t => t.CreatedBy).FirstOrDefaultAsync(t => t.Id == topicId);
        }

        public virtual async Task<IEnumerable<User>> GetAssociatedUsersByRole(int topicId, string role)
        {
            return await (from u in dbContext.Users
                          join tu in dbContext.TopicUsers
                          on u.Id equals tu.UserId
                          where tu.TopicId == topicId && tu.Role.CompareTo(role) == 0
                          select u).ToListAsync();
        }

        public virtual async Task<IEnumerable<Topic>> GetSubTopics(int topicId)
        {
            return await (from t in dbContext.Topics
                          join at in dbContext.AssociatedTopics
                          on t.Id equals at.ChildTopicId
                          where at.ParentTopicId == topicId
                          select t).ToListAsync();
        }

        public virtual async Task<IEnumerable<Topic>> GetParentTopics(int topicId)
        {
            return await (from t in dbContext.Topics
                          join at in dbContext.AssociatedTopics
                          on t.Id equals at.ParentTopicId
                          where at.ChildTopicId == topicId
                          select t).ToListAsync();
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
                    AssociateUsersToTopicByRole(topic.Id, Role.Student, model.Students);
                    AssociateUsersToTopicByRole(topic.Id, Role.Supervisor, model.Supervisors);
                    AssociateUsersToTopicByRole(topic.Id, Role.Reviewer, model.Reviewers);
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
            if (await dbContext.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id == topicId) != null)
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
                        AssociateUsersToTopicByRole(topic.Id, Role.Student, model.Students);
                        AssociateUsersToTopicByRole(topic.Id, Role.Supervisor, model.Supervisors);
                        AssociateUsersToTopicByRole(topic.Id, Role.Reviewer, model.Reviewers);
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

    
        // Private Region

        private void AssociateUsersToTopicByRole(int topicId, string role, int[] userIds)
        {
            if (userIds != null)
            {
                foreach (int userId in userIds)
                {
                    dbContext.TopicUsers.Add(new TopicUser() 
                    { 
                        UserId = userId, 
                        TopicId = topicId, 
                        Role = role 
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