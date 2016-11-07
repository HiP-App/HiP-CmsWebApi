using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Api.Models.Entity;
using Api.Models.User;
using Api.Models.Topic;
using Api.Utility;

namespace Api.Managers
{
    public class TopicManager : BaseManager
    {
        public TopicManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IQueryable<Topic> GetAllTopics(string query, string status, DateTime? deadline, bool onlyParents)
        {
            IQueryable<Topic> topics = from t in dbContext.Topics select t;
            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(t => t.Title.Contains(query) || t.Description.Contains(query));

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

        public virtual IEnumerable<TopicResult> GetTopicsForUser(int userId, int page)
        {
            var topics = dbContext.TopicUsers.Where(tu => tu.UserId == userId).Include(tu => tu.Topic).ThenInclude(t => t.CreatedBy).Skip((page - 1) * Constants.PageSize).Take(Constants.PageSize).ToList();
            return topics.Select(tu => new TopicResult(tu.Topic));
        }

        public virtual int GetTopicsCount()
        {
            return dbContext.Topics.Count();
        }

        public virtual Topic GetTopicById(int topicId)
        {
            return dbContext.Topics.Include(t => t.CreatedBy).FirstOrDefault(t => t.Id == topicId);
        }

        public virtual IEnumerable<UserResult> GetAssociatedUsersByRole(int topicId, string role)
        {
            return dbContext.TopicUsers.Where(tu => (tu.Role.Equals(role) && tu.TopicId == topicId)).Include(tu => tu.User).ToList().Select(u => new UserResult(u.User));
        }

        public virtual IEnumerable<Topic> GetSubTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Include(at => at.ChildTopic).Where(at => at.ParentTopicId == topicId).Select(at => at.ChildTopic).ToList();
        }

        public virtual IEnumerable<Topic> GetParentTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Include(at => at.ParentTopic).Where(at => at.ChildTopicId == topicId).Select(at => at.ParentTopic).ToList();
        }

        public virtual AddEntityResult AddTopic(int userId, TopicFormModel model)
        {
            try
            {
                var topic = new Topic(model);
                topic.CreatedById = userId;

                // Add User associations
                AssociateUsersToTopicByRole(Role.Student, model.Students, topic);
                AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors, topic);
                AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers, topic);

                dbContext.Topics.Add(topic);

                dbContext.SaveChanges();
                new NotificationProcessor(dbContext, topic, userId).OnNewTopic();

                return new AddEntityResult() { Success = true, Value = topic.Id };
            }
            catch (Exception e)
            {
                return new AddEntityResult() { Success = false, ErrorMessage = e.Message };
            }
        }

        public virtual bool UpdateTopic(int userId, int topicId, TopicFormModel model)
        {
            var topic = dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            if (topic == null)
                return false;

            // Using Transactions to roobback Notifications on error.
            using (var transaction = dbContext.Database.BeginTransaction())
                try
                {
                    // REM: do before updating to estimate the changes
                    new NotificationProcessor(dbContext, topic, userId).OnUpdate(model);

                    // TODO  topic.UpdatedById = userId;
                    topic.Title = model.Title;
                    topic.Status = model.Status;
                    topic.Deadline = (DateTime)model.Deadline;
                    topic.Description = model.Description;
                    topic.Requirements = model.Requirements;


                    // Add User associations
                    AssociateUsersToTopicByRole(Role.Student, model.Students, topic);
                    AssociateUsersToTopicByRole(Role.Supervisor, model.Supervisors, topic);
                    AssociateUsersToTopicByRole(Role.Reviewer, model.Reviewers, topic);

                    dbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                }

            return false;
        }

        public bool ChangeTopicStatus(int userId, int topicId, string status)
        {
            var topic = dbContext.Topics.Include(t => t.TopicUsers).FirstOrDefault(t => t.Id == topicId);
            if (topic != null)
            {
                topic.Status = status;
                dbContext.Update(topic);
                dbContext.SaveChanges();

                new NotificationProcessor(dbContext, topic, userId).OnStateChanged(status);
                return true;
            }
            return false;
        }

        public virtual bool DeleteTopic(int topicId, int userId)
        {
            var topic = dbContext.Topics.Include(t => t.TopicUsers).FirstOrDefault(u => u.Id == topicId);
            if (topic != null)
            {
                new NotificationProcessor(dbContext, topic, userId).OnDeleteTopic();
                dbContext.Remove(topic);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual bool AssociateTopic(int parentId, int childId)
        {
            // TODO throw errors
            if (!dbContext.Topics.Any(t => t.Id == childId))
                return false;
            if (!dbContext.Topics.Any(t => t.Id == parentId))
                return false;

            var relation = new AssociatedTopic() { ChildTopicId = childId, ParentTopicId = parentId };
            dbContext.Remove(relation);
            dbContext.SaveChanges();
            return true;
        }

        public virtual bool DeleteAssociated(int parentId, int childId)
        {
            var relation = dbContext.AssociatedTopics.Single(ta => (ta.ParentTopicId == parentId && ta.ChildTopicId == childId));
            if (relation != null)
            {
                dbContext.Remove(relation);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        // Private Region
        private void AssociateUsersToTopicByRole(string role, int[] userIds, Topic topic)
        {
            var existingUsers = topic.TopicUsers.Where(tu => tu.Role == role).ToList();

            var newUsers = new List<TopicUser>();
            var removedUsers = new List<TopicUser>();

            if (userIds != null)
            {
                // new user?
                foreach (int userId in userIds)
                {
                    if (!existingUsers.Any(tu => (tu.UserId == userId && tu.Role == role)))
                        newUsers.Add(new TopicUser() { UserId = userId, Role = role });
                }
                // removed user?
                foreach (TopicUser existingUser in existingUsers)
                {
                    if (!userIds.Contains(existingUser.UserId))
                        removedUsers.Add(existingUser);
                }
            }

            topic.TopicUsers.AddRange(newUsers);
            topic.TopicUsers.RemoveAll(tu => removedUsers.Contains(tu));
        }
    }
}
