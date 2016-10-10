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

        public virtual IQueryable<Topic> GetAllTopics(string query, string status, DateTime? deadline, bool onlyParents)
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

            if (onlyParents)
                topics = topics.Except(from at in dbContext.AssociatedTopics
                                       join t in topics
                                       on at.ChildTopicId equals t.Id
                                       select t);

            return topics;
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

                    // Add User associations
                    topic.TopicUsers = AssociateUsersToTopicByRole(Role.Student, model.Students);
                    topic.TopicUsers.AddRange(AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors));
                    topic.TopicUsers.AddRange(AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers));

                    // Add Topic Associations
                    topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                    dbContext.Topics.Add(topic);

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

        public virtual async Task<bool> UpdateTopicAsync(int userId, int topicId, TopicFormModel model)
        {            
            if (await dbContext.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id == topicId) != null)
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {       
                        var topic = new Topic(model);
                        topic.Id = topicId;
                        topic.CreatedById = userId;

                        dbContext.Topics.Attach(topic);
                        dbContext.Entry(topic).State = EntityState.Modified;

                        DeassociateAllLinksFromTopic(topicId);

                        // Add User associations
                        topic.TopicUsers = AssociateUsersToTopicByRole(Role.Student, model.Students);
                        topic.TopicUsers.AddRange(AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors));
                        topic.TopicUsers.AddRange(AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers));

                        // Add Topic associations
                        topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

                        await dbContext.SaveChangesAsync();

                        transaction.Commit();
                        return true;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        transaction.Rollback();
                    }
                }
            }

            return false;
        }

        public virtual async Task<bool> DeleteTopicAsync(int topicId)
        {
            var topic = await dbContext.Topics.FirstOrDefaultAsync(u => u.Id == topicId);
            
            if (topic != null)
            {
                dbContext.Remove(topic);
                await dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

    
        // Private Region

        private List<TopicUser> AssociateUsersToTopicByRole(string role, int[] userIds)
        {
            var topicUsers = new List<TopicUser>();

            if (userIds != null)
            {
                foreach (int userId in userIds)
                {
                    topicUsers.Add(new TopicUser() { UserId = userId, Role = role });
                }
            }

            return topicUsers;
        }

        private List<AssociatedTopic> AssociateTopicsToTopic(int topicId, int[] associatedTopicIds)
        {
            var associatedTopics = new List<AssociatedTopic>();

            if (associatedTopicIds != null)
            {
                foreach (int associatedTopicId in associatedTopicIds)
                {
                    associatedTopics.Add(new AssociatedTopic() { ParentTopicId = associatedTopicId });
                }
            }

            return associatedTopics;
        }

        private void DeassociateAllLinksFromTopic(int topicId)
        {
            dbContext.Database.ExecuteSqlCommand($"delete from \"TopicUsers\" where  \"TopicId\" = '{topicId}'");
            dbContext.Database.ExecuteSqlCommand($"delete from \"AssociatedTopics\" where  \"ChildTopicId\" = '{topicId}'");
        }
    }
}