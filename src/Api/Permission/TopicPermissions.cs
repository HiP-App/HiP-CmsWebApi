using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Permission
{
    public class TopicPermissions : BaseManager
    {
        private readonly UserManager _userManager;

        public TopicPermissions(CmsDbContext dbContext, UserManager userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsAllowedToEditAsync(string userId, int topicId)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(userId);

                if (user.Roles.Contains(Role.Administrator))
                    return true;

                // Created?
                if (DbContext.Topics.Any(t => t.Id == topicId && t.CreatedById == user.Id))
                    return true;

                // Supervisor?
                if (DbContext.TopicUsers.Any(tu => tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Supervisor))
                    return true;
            }
            catch (InvalidOperationException) { }
            catch (SwaggerException) { }

            return false;
        }

        public async Task<bool> IsAssociatedToAsync(string identity, int topicId)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(identity);

                if (user.Roles.Contains(Role.Administrator))
                    return true;

                // Created?
                if (DbContext.Topics.Any(t => t.Id == topicId && t.CreatedById == user.Id))
                    return true;

                // Is associated
                if (DbContext.TopicUsers.Any(tu => tu.TopicId == topicId && tu.UserId == user.Id))
                    return true;
            }
            catch (InvalidOperationException) { }
            catch (SwaggerException) { }


            return false;
        }

        public async Task<bool> IsAllowedToCreateAsync(string identity)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(identity);
                return user.Roles.Contains(Role.Administrator) || user.Roles.Contains(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (SwaggerException)
            {
                return false;
            }
        }

        public async Task<bool> IsReviewerAsync(string identity, int topicId)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(identity);
                if (DbContext.TopicUsers.Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Reviewer)))
                    return true;
            }
            catch (InvalidOperationException) { }
            catch (SwaggerException) { }

            return false;
        }
    }
}
