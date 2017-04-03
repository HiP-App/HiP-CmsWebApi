using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationTag;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class AnnotationController
    {
        #region GET

        // Get api/annotation/Tags

        /// <summary>
        /// All Annotation Tags saved in the system
        /// </summary>
        /// <param name="includeDeleted">Include already deleted, but still used Tags?</param>
        /// <param name="includeOnlyRoot">Include only root tags?</param>
        /// <response code="200">A List of AnnotationTagResults</response>
        /// <response code="204">There are no Tags in the system</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Tags")]
        [ProducesResponseType(typeof(IEnumerable<TagResult>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult GetAllTags([FromQuery]bool includeDeleted = false, [FromQuery]bool includeOnlyRoot = false)
        {
            return Ok(_tagManager.GetAllTags(includeDeleted, includeOnlyRoot));
        }

        // Get api/Annotation/Tags/:id

        /// <summary>
        /// A specific AnnotationTag save in the system
        /// </summary>
        /// <param name="id">The id of the Tag</param>
        /// <response code="200">A TagResult</response>
        /// <response code="404">There is no AnnotationTag {id} in the system</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Tags/{id}")]
        [ProducesResponseType(typeof(TagResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetTag([FromRoute]int id)
        {
            try
            {
                var tag = _tagManager.GetTag(id);
                return Ok(tag);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }


        // Get api/Annotation/Tags/:id/ChildTags

        /// <summary>
        /// All ChildTags from {id}
        /// </summary>
        /// <param name="id">The id of the parent tag</param>
        /// <response code="200">A List of AnnotationTagResults</response>
        /// <response code="204">The parent AnnotationTag {id} has no Children</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Tags/{id}/ChildTags")]
        [ProducesResponseType(typeof(List<TagResult>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult GetChildTagsOf([FromRoute]int id)
        {
            var tags = _tagManager.GetChildTagsOf(id);
            return Ok(tags);
        }

        #endregion

        #region POST

        // Post api/Annotation/Tags

        /// <summary>
        /// Add a new Tag.
        /// </summary>
        /// <param name="tag">The tag to be added</param>
        /// <response code="200">Tag created</response>
        /// <response code="403">User not allowed to created Tags</response>
        /// <response code="400">Request was missformed</response>
        /// <response code="401">User is denied</response>
        [HttpPost("Tags")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post([FromBody]TagFormModel tag)
        {
            if (!_annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserIdentity()))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest();

            var result = _tagManager.AddTag(tag);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        // Post api/Annotation/Tags/:parentId/ChildTags/:childId

        /// <summary>
        /// Add AnnotationTag {childId} as Child to AnnotationTag {parentId} 
        /// </summary>
        /// <param name="parentId">Tag to add Child to</param>
        /// <param name="childId">Tag to be added as Child</param>
        /// <response code="200">child added</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="400">Request was missformed or cycle dependency was detected</response>
        /// <response code="401">User is denied</response>
        [HttpPost("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post([FromRoute]int parentId, [FromRoute]int childId)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserIdentity()))
                return Forbid();

            if (_tagManager.AddChildTag(parentId, childId))
                return Ok();
            return BadRequest();
        }

        #endregion

        #region PUT

        // Put api/Annotation/Tags/:Id

        /// <summary>
        /// Edit AnnotationTag {Id} 
        /// </summary>
        /// <param name="id">Tag to be edited</param>
        /// <param name="model">Date to be changed</param>
        /// <response code="200">Tag edited successful</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="404">No such Tag</response>
        /// <response code="401">User is denied</response>
        [HttpPut("Tags/{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutTag([FromRoute]int id, [FromBody]TagFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserIdentity()))
                return Forbid();

            if (_tagManager.EditTag(model, id))
                return Ok();
            return NotFound();
        }

        #endregion

        #region DELETE

        // Delete api/Annotation/Tags/:Id

        /// <summary>
        /// Delete AnnotationTag {Id} 
        /// </summary>
        /// <param name="id">Tag to be delete</param>
        /// <response code="200">Tag ddeleted successful</response>
        /// <response code="403">User not allowed to delete Tags</response>
        /// <response code="404">No such Tag</response>
        /// <response code="401">User is denied</response>
        [HttpDelete("Tags/{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Delete([FromRoute]int id)
        {
            if (!_annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserIdentity()))
                return Forbid();

            if (_tagManager.DeleteTag(id))
                return Ok();
            return NotFound();
        }

        /// <summary>
        /// remove Child {childId} from AnnotationTag {parentId} 
        /// </summary>
        /// <param name="parentId">Parent AnnotationTag to remove child from</param>
        /// <param name="childId">Child to be removed</param>
        /// <response code="200">Child removed successful</response>
        /// <response code="403">User not allowed to edit Tags</response>
        /// <response code="404">No such Tag</response>
        /// <response code="401">User is denied</response>
        [HttpDelete("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteChildOf([FromRoute]int parentId, [FromRoute]int childId)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserIdentity()))
                return Forbid();

            if (_tagManager.RemoveChildTag(parentId, childId))
                return Ok();
            return NotFound();
        }

        #endregion
    }
}
