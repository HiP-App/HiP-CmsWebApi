﻿using Api.Utility;
using BLL.Managers;
using BOL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var notifications = notificationManager.CurrentUser(user);
            
            if (notifications != null)
                return Ok(notifications);
            else
                return NotFound();
        }
    }
}
