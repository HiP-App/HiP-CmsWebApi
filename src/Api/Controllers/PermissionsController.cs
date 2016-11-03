using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.Extensions.Logging;
using Api.Permission;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;

namespace Api.Controllers
{
    [ProducesResponseType(typeof(void), 200)]
    public class PermissionsController : ApiController
    {

        private TopicPermissions topicPermissions;
        private UserPermissions userPermissions;

        public PermissionsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicPermissions = new TopicPermissions(dbContext);
            userPermissions = new UserPermissions(dbContext);
        }

        #region Topics

        [HttpGet("Topics/All/Permission/IsAllowedToCreate")]
        public IActionResult IsAllowedToCreate()
        {
            if (topicPermissions.IsAllowedToCreate(User.Identity.GetUserId()))
                return Ok();
            return Unauthorized();
        }

        [HttpGet("Topics/{topicId}/Permission/IsAssociatedTo")]
        public IActionResult IsAssociatedTo(int topicId)
        {
            if (topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Ok();
            return Unauthorized();
        }

        [HttpGet("Topics/{topicId}/Permission/IsAllowedToEdit")]
        public IActionResult IsAllowedToEdit(int topicId)
        {
            if (topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Ok();
            return Unauthorized();
        }

        #endregion

        #region

        [HttpGet("Users/All/Permission/IsAllowedToAdminister")]
        public IActionResult IsAllowedToAdminister()
        {
            if (userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Ok();
            return Unauthorized();
        }

        #endregion
    }
}
