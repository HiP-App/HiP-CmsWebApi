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

        #region GET

        // GET api/Notifications

        /// <summary>
        /// Get all notifications for the user
        /// </summary>
        /// <response code="200">A List of All NotificationResults for the current user</response>
        /// <response code="404">There are no Notifications for the current user</response>                
        /// <response code="401">User is denied</response>
        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<NotificationResult>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAllNotifications()
        {
            return GetNotifications(false);
        }

        // GET api/Notifications/Unread

        /// <summary>
        /// Get unread notifications for the user
        /// </summary>
        /// <response code="200">A List of Unread NotificationResults for the current user</response>
        /// <response code="404">There are no Unread Notifications for the current user</response>                
        /// <response code="401">User is denied</response>
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

        /// <summary>
        /// Get notification count for the user
        /// </summary>
        /// <response code="200">Returns a Notification count for the current user</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("Count")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetNotificationCount()
        {
            return Ok(notificationManager.GetNotificationCount(User.Identity.GetUserId()));
        }

        #endregion

        #region POST

        // POST api/Notifications/:id/markread

        /// <summary>
        /// Makes the notification {notificationId} as read
        /// </summary>
        /// <param name="notificationId">The id of the Notification</param>
        /// <response code="200">Notification {notificationId} is marked as read</response>
        /// <response code="404">Notification {notificationId} is not found</response>                
        /// <response code="401">User is denied</response>
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
        
        #endregion
        
        #region PUT

        /// <summary>
        /// Subscribes the current user to notifications of the given type.
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns>nothing</returns>
        /// <response code="200">OK</response>
        /// <response code="403">User is not allowed to subscribe</response>
        /// <response code="404">Resource Not Found</response>
        [HttpPut("subscribe/{notificationType}")]
        public IActionResult PutSubscribe(string notificationType)
        {
            return setSubscription(notificationType, true);
        }

        /// <summary>
        /// Unsubscribes the current user from notifications of the given type.
        /// </summary>
        /// <param name="notificationType"></param>
        /// <returns>nothing</returns>
        /// <response code="200">OK</response>
        /// <response code="403">User is not allowed to unsubscribe</response>
        /// <response code="404">Resource Not Found</response>
        [HttpPut("unsubscribe/{notificationType}")]
        public IActionResult PutUnsubscribe(string notificationType)
        {
            return setSubscription(notificationType, false);
        }

        private IActionResult setSubscription(string notificationType, bool subscribe)
        {
            NotificationType type;
            if (NotificationType.TryParse(notificationType, out type))
            {
                if (notificationManager.SetSubscription(User.Identity.GetUserId(), type, subscribe))
                {
                    return Ok();
                } else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        #endregion
    }
}