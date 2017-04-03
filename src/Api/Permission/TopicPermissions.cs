using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Permission
{
    public class TopicPermissions : BaseManager
    {
        // TODO change User.Email to the Identity if changed!
        private readonly UserManager _userManager;


        public TopicPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }


        public bool IsAllowedToEdit(string identity, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdentity(identity);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (DbContext.Topics.Include(t => t.CreatedBy).Any(t => (t.Id == topicId && t.CreatedById == user.Id)))
                    return true;
                // Supervisor?
                if (DbContext.TopicUsers.Include(t => t.User).Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Supervisor)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool IsAssociatedTo(string identity, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdentity(identity);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (DbContext.Topics.Include(t => t.CreatedBy).Any(t => (t.Id == topicId && t.CreatedById == user.Id)))
                    return true;
                // Is associated
                if (DbContext.TopicUsers.Include(t => t.User).Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool IsAllowedToCreate(string identity)
        {
            try
            {
                var user = _userManager.GetUserByIdentity(identity);
                return user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsReviewer(string identity, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdentity(identity);
                if (DbContext.TopicUsers.Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Reviewer)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }
    }
}
