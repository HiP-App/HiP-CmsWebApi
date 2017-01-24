using System;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public partial class AnnotationController
    {

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

        /// <summary>
        /// Add Relation between the tag instance represented by {firstId} and {secondId}.
        /// </summary>
        /// <param name="firstId">ID of the first tag instance of the relation</param>
        /// <param name="secondId">ID of the second tag instance of the relation</param>
        /// <param name="name" optional="true">Relation name</param>
        /// <response code="200">Relation added</response>
        /// <response code="403">User not allowed to add a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/Instance/{firstId}/Relation/{secondId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PostTagRelation([FromRoute] int firstId, [FromRoute] int secondId, [FromQuery] string name = "")
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                var success = _tagManager.AddTagRelation(firstId, secondId, name);
                if (success) return Ok();
                else return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove relation between the tag instances represented by {firstId} and {secondId}
        /// </summary>
        /// <param name="firstId">ID of the first tag instance of the relation</param>
        /// <param name="secondId">ID of the second tag instance of the relation</param>
        /// <response code="200">Relation removed</response>
        /// <response code="403">User not allowed to remove a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Tags/{firstId}/Relation/{secondId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteTagRelation([FromRoute] int firstId, [FromRoute] int secondId)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                var success = _tagManager.RemoveTagRelation(firstId, secondId);
                if (success) return Ok();
                else return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

    }
}
