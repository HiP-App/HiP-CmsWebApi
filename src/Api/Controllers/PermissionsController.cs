using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using Microsoft.Extensions.Logging;
using PaderbornUniversity.SILab.Hip.CmsApi.Permission;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    /// <summary>
    /// Decides the permission for the user
    /// </summary>
    /// <response code="200">User is allowed</response>
    /// <response code="403">User is denied</response>
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 403)]
    public class PermissionsController : ApiController
    {

        private readonly AnnotationPermissions _annotationPermissions;
        private readonly TopicPermissions _topicPermissions;
        private readonly UserPermissions _userPermissions;

        public PermissionsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            _annotationPermissions = new AnnotationPermissions(dbContext);
            _topicPermissions = new TopicPermissions(dbContext);
            _userPermissions = new UserPermissions(dbContext);
        }

        #region Annotations

        /// <summary>
        /// Is the current user allowed to create new Annotation Tags.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="401">User is denied</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Annotation/Tags/All/Permission/IsAllowedToCreate")]
        public IActionResult IsAllowedToCreateTags()
        {
            if (_annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserIdentity()))
                return Ok();
            return Unauthorized();
        }

        /// <summary>
        /// Is the current user allowed to edit Annotation Tags.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="401">User is denied</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Annotation/Tags/All/Permission/IsAllowedToEdit")]
        public IActionResult IsAllowedToEditTags()
        {
            if (_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserIdentity()))
                return Ok();
            return Unauthorized();
        }

        #endregion

        #region Topics

        /// <summary>
        /// Is the current user allowed to create new Topics.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/All/Permission/IsAllowedToCreate")]
        public IActionResult IsAllowedToCreate()
        {
            if (_topicPermissions.IsAllowedToCreate(User.Identity.GetUserIdentity()))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user allowed to see and edit the content of the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsAssociatedTo")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public IActionResult IsAssociatedTo([FromRoute]int topicId)
        {
            if (_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user allowed to edit the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsAllowedToEdit")]
        public IActionResult IsAllowedToEdit([FromRoute]int topicId)
        {
            if (_topicPermissions.IsAllowedToEdit(User.Identity.GetUserIdentity(), topicId))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user allowed to review the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsReviewer")]
        public IActionResult IsAllowedToReview([FromRoute]int topicId)
        {
            if (_topicPermissions.IsReviewer(User.Identity.GetUserIdentity(), topicId))
                return Ok();
            return Forbid();
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
            if (_userPermissions.IsAllowedToAdminister(User.Identity))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user is allowed to invite users.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Users/All/Permission/IsAllowedToInvite")]
        public IActionResult IsAllowedToInvite()
        {
            if (_userPermissions.IsAllowedToInvite(User.Identity))
                return Ok();
            return Forbid();
        }

        #endregion
    }
}
