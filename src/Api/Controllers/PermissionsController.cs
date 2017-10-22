using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using Microsoft.Extensions.Logging;
using PaderbornUniversity.SILab.Hip.CmsApi.Permission;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using System.Threading.Tasks;

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

        public PermissionsController(CmsDbContext dbContext, ILoggerFactory loggerFactory,
            AnnotationPermissions annotationPermissions,
            TopicPermissions topicPermissions,
            UserPermissions userPermissions)
            : base(dbContext, loggerFactory)
        {
            _annotationPermissions = annotationPermissions;
            _topicPermissions = topicPermissions;
            _userPermissions = userPermissions;
        }

        #region Annotations

        /// <summary>
        /// Is the current user allowed to create new Annotation Tags.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="401">User is denied</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Annotation/Tags/All/Permission/IsAllowedToCreate")]
        public async Task<IActionResult> IsAllowedToCreateTagsAsync()
        {
            if (await _annotationPermissions.IsAllowedToCreateTagsAsync(User.Identity.GetUserIdentity()))
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
        public async Task<IActionResult> IsAllowedToEditTagsAsync()
        {
            if (await _annotationPermissions.IsAllowedToEditTagsAsync(User.Identity.GetUserIdentity()))
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
        public async Task<IActionResult> IsAllowedToCreateAsync()
        {
            if (await _topicPermissions.IsAllowedToCreateAsync(User.Identity.GetUserIdentity()))
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
        public async Task<IActionResult> IsAssociatedToAsync([FromRoute]int topicId)
        {
            if (await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user allowed to edit the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsAllowedToEdit")]
        public async Task<IActionResult> IsAllowedToEditAsync([FromRoute]int topicId)
        {
            if (await _topicPermissions.IsAllowedToEditAsync(User.Identity.GetUserIdentity(), topicId))
                return Ok();
            return Forbid();
        }

        /// <summary>
        /// Is the current user allowed to review the topic.
        /// </summary>
        /// <response code="200">User is allowed</response>
        /// <response code="403">User is denied</response>
        [HttpGet("Topics/{topicId}/Permission/IsReviewer")]
        public async Task<IActionResult> IsAllowedToReviewAsync([FromRoute]int topicId)
        {
            if (await _topicPermissions.IsReviewerAsync(User.Identity.GetUserIdentity(), topicId))
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
