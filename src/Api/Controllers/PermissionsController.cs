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
    [ProducesResponseType(typeof(void), 403)]
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

        /// <summary>
        /// Is the current user allowed to create new Topics.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/All/Permission/IsAllowedToCreate")]
        public IActionResult IsAllowedToCreate()
        {
            if (topicPermissions.IsAllowedToCreate(User.Identity.GetUserId()))
                return Ok();
            return Forbidden();
        }

        /// <summary>
        /// Is the current user allowed to see and edit the content of the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsAssociatedTo")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public IActionResult IsAssociatedTo(int topicId)
        {
            if (topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Ok();
            return Forbidden();
        }

        /// <summary>
        /// Is the current user allowed to edit the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsAllowedToEdit")]
        public IActionResult IsAllowedToEdit(int topicId)
        {
            if (topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Ok();
            return Forbidden();
        }

        #endregion

        #region User

        /// <summary>
        /// Is the current user allowed to administer users.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Users/All/Permission/IsAllowedToAdminister")]
        public IActionResult IsAllowedToAdminister()
        {
            if (userPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Ok();
            return Forbidden();
        }

        /// <summary>
        /// Is the current user is allowed to invite users.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Users/All/Permission/IsAllowedToInvite")]
        public IActionResult IsAllowedToInvite()
        {
            if (userPermissions.IsAllowedToInvite(User.Identity.GetUserId()))
                return Ok();
            return Forbidden();
        }

        #endregion
    }
}
