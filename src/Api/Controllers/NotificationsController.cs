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

        // GET api/Notifications/current
        [HttpGet]
        [Route("Current")]
        public IActionResult GetNotifications()
        {            
            var user = userManager.GetUserById(User.Identity.GetUserId());
            var notifications = notificationManager.GetNotificationsForTheUser(user);
            
            if (notifications != null)
                return Ok(notifications);
            else
                return NotFound();
        }

        // POST api/Notifications/:id/markread
        [HttpPost()]        
        public IActionResult Post(int notificationId)
        {
            var result = notificationManager.MarkAsRead(notificationId);
            if (result == true)
                return Ok(true);
            else
                return BadRequest();
        }
    }
}
