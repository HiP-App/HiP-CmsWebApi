using System;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public partial class AnnotationController
    {

        #region GET

        /// <summary>
        /// Get all existing tag relations. Provide a maxDepth parameter for limiting the maximum depth of the returned tags in the tag tree.
        /// </summary>
        /// <param name="maxDepth" optional="true">The maximum depth of the returned tags. Set to 0 to get only the root tags' relations. Defaults to infinity.</param>
        /// <response code="200">Returns a list of tag relations</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/Relations/{maxDepth}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetRelations([FromRoute] int maxDepth = int.MaxValue)
        {
            return BadRequest();
        }

        #endregion

        #region POST

        /// <summary>
        /// Add Relation from the tag represented by {sourceId} to the tag represented by {targetId}.
        /// </summary>
        /// <param name="sourceId">ID of the source tag of the relation</param>
        /// <param name="targetId">ID of the target tag of the relation</param>
        /// <param name="name" optional="true">Relation name</param>
        /// <response code="200">Relation added</response>
        /// <response code="403">User not allowed to add a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/{sourceId}/Relation/{targetId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PostTagRelation([FromRoute] int sourceId, [FromRoute] int targetId, [FromQuery] string name = "")
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                var success = _tagManager.AddTagRelation(sourceId, targetId, name);
                if (success) return Ok();
                else return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Remove the relation from the tag represented by {sourceId} to the tag represented by {targetId}.
        /// </summary>
        /// <param name="sourceId">ID of the source tag of the relation</param>
        /// <param name="targetId">ID of the target tag of the relation</param>
        /// <response code="200">Relation removed</response>
        /// <response code="403">User not allowed to remove a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Tags/{sourceId}/Relation/{targetId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteTagRelation([FromRoute] int sourceId, [FromRoute] int targetId)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                var success = _tagManager.RemoveTagRelation(sourceId, targetId);
                if (success) return Ok();
                else return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion
    }
}
