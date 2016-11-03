using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Api.Models.Entity;
using Api.Models.User;
using Api.Models.Topic;

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

        public virtual IQueryable<TopicResult> GetTopicsForUser(int userId)
        {
            return dbContext.TopicUsers.Where(tu => tu.UserId == userId).Include(tu => tu.Topic).ThenInclude(t => t.CreatedBy).Select(tu => new TopicResult(tu.Topic));
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
            return dbContext.TopicUsers.Where(tu => (tu.Role.Equals(role) && tu.TopicId == topicId)).ToList().Select(u => new UserResult(u.User));
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

                // Add Topic Associations
                topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

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

                    // Add Topic associations
                    topic.AssociatedTopics = AssociateTopicsToTopic(topic.Id, model.AssociatedTopics);

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
    }
}
