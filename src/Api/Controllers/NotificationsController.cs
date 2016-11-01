using Api.Utility;
using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.Managers;

namespace Api.Controllers
{
    public class NotificationsController : ApiController
    {
        private NotificationManager notificationManager;
        private UserManager userManager;

        public NotificationsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            notificationManager = new NotificationManager(dbContext);
            userManager = new UserManager(dbContext);
        }

        // GET api/Notifications/
        [HttpGet]
        [Route("")]
        public IActionResult GetNotifications()
        {
            var notifications = notificationManager.GetNotificationsForTheUser(User.Identity.GetUserId());

            if (notifications != null)
                return Ok(notifications);
            else
                return NotFound();
        }
        // GET api/Notifications/Count
        [HttpGet]
        [Route("Count")]
        public IActionResult GetNotificationCount()
        {
            return Ok(notificationManager.GetNotificationCount(User.Identity.GetUserId()));
        }

        // POST api/Notifications/:id/markread
        [HttpPost("{notificationId}/markread")]
        public IActionResult Post(int notificationId)
        {
            if (notificationManager.MarkAsRead(notificationId))
                return Ok();
            else
                return BadRequest();
        }
    }
}