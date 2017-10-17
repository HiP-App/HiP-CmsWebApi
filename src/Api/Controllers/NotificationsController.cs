using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public class NotificationsController : ApiController
    {
        private readonly NotificationManager _notificationManager;

        public NotificationsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            _notificationManager = new NotificationManager(dbContext);
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

        private IActionResult GetNotifications([FromQuery]bool onlyUnread)
        {
            var notifications = _notificationManager.GetNotificationsForTheUser(User.Identity.GetUserIdentity(), onlyUnread);

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
            return Ok(_notificationManager.GetNotificationCount(User.Identity.GetUserIdentity()));
        }

        // GET api/Notifications/Subscriptions

        /// <summary>
        /// Get all subscriptions for the current user
        /// </summary>
        /// <response code="200">Returns a list of subscriptions for the current user</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("Subscriptions")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public IActionResult GetSubscriptions()
        {
            return Ok(_notificationManager.GetSubscriptions(User.Identity.GetUserIdentity()));
        }

        // GET api/Notifications/Types

        /// <summary>
        /// Get all Notification Types
        /// </summary>
        /// <response code="200">Returns a list of subscriptions types</response>  
        [HttpGet("Types")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public IActionResult GetNotificationsTypes()
        {
            var result = (from object type in Enum.GetValues(typeof(NotificationType)) select Enum.GetName(typeof(NotificationType), type)).ToList();
            return Ok(result);
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
        public IActionResult Post([FromRoute]int notificationId)
        {
            if (_notificationManager.MarkAsRead(notificationId))
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
        public IActionResult PutSubscribe([FromRoute]string notificationType)
        {
            return SetSubscription(notificationType, true);
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
        public IActionResult PutUnsubscribe([FromRoute]string notificationType)
        {
            return SetSubscription(notificationType, false);
        }

        private IActionResult SetSubscription(string notificationType, bool subscribe)
        {
            if (Enum.TryParse(notificationType, out NotificationType type))
            {
                if (_notificationManager.SetSubscription(User.Identity.GetUserIdentity(), type, subscribe))
                {
                    return Ok();
                }
                else
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