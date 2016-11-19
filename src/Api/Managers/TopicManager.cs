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
            IQueryable<Topic> topics = dbContext.Topics.Include(t => t.CreatedBy);
            if (!string.IsNullOrEmpty(query))
                topics = topics.Where(t => t.Title.Contains(query) || t.Description.Contains(query));

            if (!string.IsNullOrEmpty(status))
                topics = topics.Where(t => t.Status.Equals(status));

            if (deadline != null && deadline.HasValue)
                topics = topics.Where(t => DateTime.Compare(t.Deadline, deadline.Value) == 0);

            // only parents without parent.
            if (onlyParents)
            {
                var topicsWithParent = dbContext.AssociatedTopics.ToList().Select(at => at.ChildTopicId);
                topics.Where(t => !topicsWithParent.Contains(t.Id));
            }

            return topics;
        }

        public virtual IEnumerable<TopicResult> GetTopicsForUser(int userId, int page)
        {
            var relatedTopicIds = dbContext.TopicUsers.Where(ut => ut.UserId == userId).ToList().Select(ut => ut.TopicId);

           var topics = dbContext.Topics.Include(t => t.CreatedBy)
                .Where(t => t.CreatedById == userId || relatedTopicIds.Contains(t.Id))
                .Skip((page - 1) * Constants.PageSize).Take(Constants.PageSize).ToList();
            return topics.Select(t => new TopicResult(t));
        }

        public virtual int GetTopicsCount()
        {
            return dbContext.Topics.Count();
        }

        public virtual Topic GetTopicById(int topicId)
        {
            return dbContext.Topics.Include(t => t.CreatedBy).Single(t => t.Id == topicId);
        }

        public virtual IEnumerable<UserResult> GetAssociatedUsersByRole(int topicId, string role)
        {
            return dbContext.TopicUsers.Where(tu => (tu.Role.Equals(role) && tu.TopicId == topicId)).Include(tu => tu.User).ToList().Select(u => new UserResult(u.User));
        }

        public virtual bool ChangeAssociatedUsersByRole(int updaterId, int topicId, string role, int[] userIds)
        {
            var topic = dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            if (topic == null)
                return false;

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
            // Updated // TODO add user
            topic.UpdatedAt = DateTime.Now;
            // Notifications
            new NotificationProcessor(dbContext, topic, updaterId).OnUsersChanged(newUsers, removedUsers, role);

            dbContext.Update(topic);
            dbContext.SaveChanges();
            return true;
        }

        public virtual IEnumerable<Topic> GetSubTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Include(at => at.ChildTopic).Where(at => at.ParentTopicId == topicId).Select(at => at.ChildTopic).ToList();
        }

        public virtual IEnumerable<Topic> GetParentTopics(int topicId)
        {
            return dbContext.AssociatedTopics.Include(at => at.ParentTopic).Where(at => at.ChildTopicId == topicId).Select(at => at.ParentTopic).ToList();
        }

        public virtual EntityResult AddTopic(int userId, TopicFormModel model)
        {
            try
            {
                var topic = new Topic(model);
                topic.CreatedById = userId;
                dbContext.Topics.Add(topic);
                dbContext.SaveChanges();
                new NotificationProcessor(dbContext, topic, userId).OnNewTopic();

                return EntityResult.Successfull(topic.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
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
            var topic = dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
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
            var topic = dbContext.Topics.Include(t => t.TopicUsers).Single(u => u.Id == topicId);
            if (topic != null)
            {
                new NotificationProcessor(dbContext, topic, userId).OnDeleteTopic();
                dbContext.Remove(topic);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual EntityResult AssociateTopic(int parentId, int childId)
        {
            // TODO throw errors
            if (!dbContext.Topics.Any(t => t.Id == childId))
                return EntityResult.Error("Child not Found");
            if (!dbContext.Topics.Any(t => t.Id == parentId))
                return EntityResult.Error("Parent not Found");

            if (dbContext.AssociatedTopics.Any(at => at.ChildTopicId == childId && at.ParentTopicId == parentId))
                return EntityResult.Error("Allready exists");

            var relation = new AssociatedTopic() { ChildTopicId = childId, ParentTopicId = parentId };

            dbContext.Add(relation);
            dbContext.SaveChanges();
            return EntityResult.Successfull(null);
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
    }
}