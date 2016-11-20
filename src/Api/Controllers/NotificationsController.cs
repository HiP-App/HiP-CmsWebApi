using Api.Utility;
using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.Managers;
using System.Collections.Generic;
using Api.Models.Notifications;

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

        // GET api/Notifications/All
        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<NotificationResult>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAllNotifications()
        {
            return GetNotifications(false);
        }

        // GET api/Notifications/Unread
        [HttpGet("Unread")]
        [ProducesResponseType(typeof(IEnumerable<NotificationResult>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetUnreadNotifications()
        {
            return GetNotifications(true);
        }

        private IActionResult GetNotifications(bool onlyUread)
        {
            var notifications = notificationManager.GetNotificationsForTheUser(User.Identity.GetUserId(), onlyUread);

            if (notifications != null)
                return Ok(notifications);
            else
                return NotFound();
        }



        // GET api/Notifications/Count
        [HttpGet("Count")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetNotificationCount()
        {
            return Ok(notificationManager.GetNotificationCount(User.Identity.GetUserId()));
        }

        // POST api/Notifications/:id/markread
        [HttpPost("{notificationId}/markread")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Post(int notificationId)
        {
            if (notificationManager.MarkAsRead(notificationId))
                return Ok();
            else
                return NotFound();
        }

        // PUT api/notifications/subscribe/:notificationType
        [HttpPut("subscribe/{notificationType}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutSubscribe(string notificationType)
        {
            return setSubscription(notificationType, true);
        }

        // PUT api/notifications/unsubscribe/:notificationType
        [HttpPut("unsubscribe/{notificationType}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutUnsubscribe(string notificationType)
        {
            return setSubscription(notificationType, false);
        }

        private IActionResult setSubscription(string notificationType, bool subscribe)
        {
            NotificationType type;
            if (NotificationType.TryParse(notificationType, out type))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}