using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public partial class TopicManager : BaseManager
    {
        private readonly UserManager _userManager;

        public TopicManager(CmsDbContext dbContext, UserManager userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public PagedResult<TopicResult> GetAllTopics(string queryString, string status, DateTime? deadline, bool onlyParents, int page, int pageSize)
        {
            IQueryable<Topic> query = DbContext.Topics;

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

        public PagedResult<TopicResult> GetTopicsForUser(string userId, int page, int pageSize, string queryString)
        {
            var relatedTopicIds = DbContext.TopicUsers.Where(ut => ut.UserId == userId).Select(ut => ut.TopicId).ToList();

            var query = DbContext.Topics.Where(t => t.CreatedById == userId || relatedTopicIds.Contains(t.Id));

            if (!string.IsNullOrEmpty(queryString))
                query = query.Where(t => t.Title.Contains(queryString) || t.Description.Contains(queryString));

            var totalCount = query.Count();

            return (page != 0)
                ? new PagedResult<TopicResult>(query.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(t => new TopicResult(t)), page, pageSize, totalCount)
                : new PagedResult<TopicResult>(query.ToList().Select(t => new TopicResult(t)), totalCount);
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public Topic GetTopicById(int topicId)
        {
            return DbContext.Topics.Single(t => t.Id == topicId);
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

        public IEnumerable<string> GetAssociatedUsersByRole(int topicId, string role)
        {
            return DbContext.TopicUsers
                .Where(tu => tu.Role == role && tu.TopicId == topicId)
                .Select(u => u.UserId);
        }

        public async Task<bool> ChangeAssociatedUsersByRoleAsync(string updaterIdentity, int topicId, string role, UsersFormModel users)
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
            if (users.Users != null)
            {
                // new user?
                foreach (var email in users.Users)
                {
                    if (!existingUsers.Any(tu => tu.User.Email == email && tu.Role == role))
                    {
                        var user = await _userManager.GetUserByEmailAsync(email);
                        newUsers.Add(new TopicUser { UserId = user.Id, Role = role });
                    }
                }
                // removed user?
                removedUsers.AddRange(existingUsers.Where(existingUser => !users.Users.Contains(existingUser.User.Email)));
            }
            topic.TopicUsers.AddRange(newUsers);
            topic.TopicUsers.RemoveAll(tu => removedUsers.Contains(tu));
            // Updated // TODO add user
            topic.UpdatedAt = DateTime.Now;

            DbContext.Update(topic);
            DbContext.SaveChanges();

            try
            {
                // Notifications
                await new NotificationProcessor(DbContext, topic, updaterIdentity, _userManager).OnUsersChangedAsync(newUsers, removedUsers, role);
            }
            catch (NullReferenceException)
            {
                return false;
            }

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

        public async Task<EntityResult> AddTopicAsync(string updaterUserId, TopicFormModel model)
        {
            try
            {
                var topic = new Topic(model) { CreatedById = updaterUserId };
                DbContext.Topics.Add(topic);
                DbContext.SaveChanges();
                await new NotificationProcessor(DbContext, topic, updaterUserId, _userManager).OnNewTopicAsync();

                return EntityResult.Successful(topic.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }

        public async Task<bool> UpdateTopicAsync(string identity, int topicId, TopicFormModel model)
        {
            // Using Transactions to roobback Notifications on error.
            using (var transaction = DbContext.Database.BeginTransaction())
                try
                {
                    var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
                    // REM: do before updating to estimate the changes
                    await new NotificationProcessor(DbContext, topic, identity, _userManager).OnUpdateAsync(model);

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

        public async Task<bool> ChangeTopicStatusAsync(string identity, int topicId, string status)
        {
            try
            {
                var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
                topic.Status = status;
                DbContext.Update(topic);
                DbContext.SaveChanges();

                try
                {
                    // Notifications
                    await new NotificationProcessor(DbContext, topic, identity, _userManager).OnStateChangedAsync(status);
                }
                catch (NullReferenceException)
                {
                    return false;
                }

                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public virtual bool DeleteTopic(int topicId, string identity)
        {
            try
            {
                var topic = DbContext.Topics.Include(t => t.TopicUsers).Single(u => u.Id == topicId);
                DbContext.Remove(topic);
                DbContext.SaveChanges();

                try
                {
                    // Notifications
                    await new NotificationProcessor(DbContext, topic, identity, _userManager).OnDeleteTopicAsync();
                }
                catch (NullReferenceException)
                {
                    return false;
                }

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
                return EntityResult.Error("Child not found");

            if (!DbContext.Topics.Any(t => t.Id == parentId))
                return EntityResult.Error("Parent not found");

            if (DbContext.AssociatedTopics.Any(at => at.ChildTopicId == childId && at.ParentTopicId == parentId))
                return EntityResult.Error("Allready exists");

            var relation = new AssociatedTopic { ChildTopicId = childId, ParentTopicId = parentId };

            DbContext.Add(relation);
            DbContext.SaveChanges();
            return EntityResult.Successful(null);
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