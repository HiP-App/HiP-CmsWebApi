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
    public partial class TopicManager : BaseManager
    {
        public TopicManager(CmsDbContext dbContext) : base(dbContext) { }

        public PagedResult<TopicResult> GetAllTopics(string queryString, string status, DateTime? deadline, bool onlyParents, int page, int pageSize)
        {
            IQueryable<Topic> query = DbContext.Topics.Include(t => t.CreatedBy);
            if (!string.IsNullOrEmpty(queryString))
                query = query.Where(t => t.Title.Contains(queryString) || t.Description.Contains(queryString));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status.Equals(status));

            if (deadline != null)
                query = query.Where(t => DateTime.Compare(t.Deadline, deadline.Value) == 0);

            // only parents without parent.
            if (onlyParents)
            {
                var topicsWithParent = DbContext.AssociatedTopics.Select(at => at.ChildTopicId).ToList();
                query = query.Where(t => !topicsWithParent.Contains(t.Id));
            }

            var totalCount = query.Count();
            if (page != 0)
                return new PagedResult<TopicResult>(query.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(t => new TopicResult(t)), page, pageSize, totalCount);
            return new PagedResult<TopicResult>(query.ToList().Select(t => new TopicResult(t)), totalCount);

        }

        public PagedResult<TopicResult> GetTopicsForUser(int userId, int page, int pageSize)
        {
            var relatedTopicIds = DbContext.TopicUsers.Where(ut => ut.UserId == userId).ToList().Select(ut => ut.TopicId);

            var query = DbContext.Topics.Include(t => t.CreatedBy)
                 .Where(t => t.CreatedById == userId || relatedTopicIds.Contains(t.Id));

            var totalCount = query.Count();
            if (page != 0)
                return new PagedResult<TopicResult>(query.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(t => new TopicResult(t)), page, pageSize, totalCount);
            return new PagedResult<TopicResult>(query.ToList().Select(t => new TopicResult(t)), totalCount);
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public Topic GetTopicById(int topicId)
        {
            return DbContext.Topics.Include(t => t.CreatedBy).Single(t => t.Id == topicId);
        }

        public bool IsValidTopicId(int topicId)
        {
            try
            {
                GetTopicById(topicId);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<UserResult> GetAssociatedUsersByRole(int topicId, string role)
        {
            return DbContext.TopicUsers.Where(tu => (tu.Role.Equals(role) && tu.TopicId == topicId)).Include(tu => tu.User).ToList().Select(u => new UserResult(u.User));
        }

        public bool ChangeAssociatedUsersByRole(string updaterIdenty, int topicId, string role, int[] userIds)
        {

            Topic topic;
            try
            {
                topic = DbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            var existingUsers = topic.TopicUsers.Where(tu => tu.Role == role).ToList();

            var newUsers = new List<TopicUser>();
            var removedUsers = new List<TopicUser>();

            if (userIds != null)
            {
                // new user?
                foreach (var userId in userIds)
                {
                    if (!existingUsers.Any(tu => (tu.UserId == userId && tu.Role == role)))
                        newUsers.Add(new TopicUser() { UserId = userId, Role = role });
                }
                // removed user?
                removedUsers.AddRange(existingUsers.Where(existingUser => !userIds.Contains(existingUser.UserId)));
            }

            topic.TopicUsers.AddRange(newUsers);
            topic.TopicUsers.RemoveAll(tu => removedUsers.Contains(tu));
            // Updated // TODO add user
            topic.UpdatedAt = DateTime.Now;
            // Notifications
            new NotificationProcessor(DbContext, topic, updaterIdenty).OnUsersChanged(newUsers, removedUsers, role);

            DbContext.Update(topic);
            DbContext.SaveChanges();
            return true;
        }

        public IEnumerable<Topic> GetSubTopics(int topicId)
        {
            return DbContext.AssociatedTopics.Include(at => at.ChildTopic).Where(at => at.ParentTopicId == topicId).Select(at => at.ChildTopic).ToList();
        }

        public IEnumerable<Topic> GetParentTopics(int topicId)
        {
            return DbContext.AssociatedTopics.Include(at => at.ParentTopic).Where(at => at.ChildTopicId == topicId).Select(at => at.ParentTopic).ToList();
        }

        public EntityResult AddTopic(string updaterIdenty, TopicFormModel model)
        {
            try
            {
                var topic = new Topic(model) { CreatedById = DbContext.Users.Single(u => u.Email == updaterIdenty).Id };
                DbContext.Topics.Add(topic);
                DbContext.SaveChanges();
                new NotificationProcessor(DbContext, topic, updaterIdenty).OnNewTopic();

                return EntityResult.Successfull(topic.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }

        public bool UpdateTopic(string userIdenty, int topicId, TopicFormModel model)
        {
            // Using Transactions to roobback Notifications on error.
            using (var transaction = DbContext.Database.BeginTransaction())
                try
                {
                    var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
                    // REM: do before updating to estimate the changes
                    new NotificationProcessor(DbContext, topic, userIdenty).OnUpdate(model);

                    // TODO  topic.UpdatedById = userId;
                    topic.Title = model.Title;
                    topic.Status = model.Status;
                    if (model.Deadline != null)
                        topic.Deadline = (DateTime)model.Deadline;
                    topic.Description = model.Description;
                    topic.Requirements = model.Requirements;

                    DbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    //Not found
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    return false;
                }
        }

        public bool ChangeTopicStatus(string userIdenty, int topicId, string status)
        {
            try
            {
                var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
                topic.Status = status;
                DbContext.Update(topic);
                DbContext.SaveChanges();
                new NotificationProcessor(DbContext, topic, userIdenty).OnStateChanged(status);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public virtual bool DeleteTopic(int topicId, string userIdenty)
        {
            try
            {
                var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(u => u.Id == topicId);
                new NotificationProcessor(DbContext, topic, userIdenty).OnDeleteTopic();
                DbContext.Remove(topic);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public virtual EntityResult AssociateTopic(int parentId, int childId)
        {
            // TODO throw errors
            if (!DbContext.Topics.Any(t => t.Id == childId))
                return EntityResult.Error("Child not Found");
            if (!DbContext.Topics.Any(t => t.Id == parentId))
                return EntityResult.Error("Parent not Found");

            if (DbContext.AssociatedTopics.Any(at => at.ChildTopicId == childId && at.ParentTopicId == parentId))
                return EntityResult.Error("Allready exists");

            var relation = new AssociatedTopic() { ChildTopicId = childId, ParentTopicId = parentId };

            DbContext.Add(relation);
            DbContext.SaveChanges();
            return EntityResult.Successfull(null);
        }

        public virtual bool DeleteAssociated(int parentId, int childId)
        {
            try
            {
                var relation = DbContext.AssociatedTopics.Single(ta => (ta.ParentTopicId == parentId && ta.ChildTopicId == childId));
                DbContext.Remove(relation);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}