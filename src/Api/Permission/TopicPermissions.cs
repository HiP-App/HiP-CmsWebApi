using Api.Data;
using Api.Managers;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Permission
{
    public class TopicPermissions : BaseManager
    {
        private UserManager userManager;


        public TopicPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            this.userManager = new UserManager(dbContext);
        }


        public bool IsAllowedToEdit(int userId, int topicId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (dbContext.Topics.Any(t => (t.Id == topicId && t.CreatedById == userId)))
                    return true;
                // Supervisor?
                if (dbContext.TopicUsers.Any(tu => (tu.TopicId == topicId && tu.UserId == userId && tu.Role == Role.Supervisor)))
                    return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAssociatedTo(int userId, int topicId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (dbContext.Topics.Any(t => (t.Id == topicId && t.CreatedById == userId)))
                    return true;
                // Is associated
                if (dbContext.TopicUsers.Any(tu => (tu.TopicId == topicId && tu.UserId == userId)))
                    return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAllowedToCreate(int userId)
        {
            try
            {
                var user = userManager.GetUserById(userId);
                return user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

    }
}
