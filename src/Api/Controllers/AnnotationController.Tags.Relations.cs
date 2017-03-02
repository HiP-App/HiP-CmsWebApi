using System;
using System.Collections.Generic;
using Api.Models.AnnotationTag;
using Api.Models.Entity.Annotation;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public partial class AnnotationController
    {

        #region GET

        /// <summary>
        /// Get all existing tag relations.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <response code="200">Returns a list of tag relations</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/Relations")]
        [ProducesResponseType(typeof(List<AnnotationTagRelationResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetRelations([FromQuery] int maxDepth = int.MaxValue)
        {
            // TODO: Do we need pagination here?
            return ServiceUnavailable();
        }

        /// <summary>
        /// Get all existing tag relations for the tag represented by the given id.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <param name="tagId">The Id of the tag that you want the relations for</param>
        /// <response code="200">Returns a list of tag relations</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/{tagId}/Relations")]
        [ProducesResponseType(typeof(List<AnnotationTagRelationResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetRelationsForId([FromRoute] int tagId)
        {
            return ServiceUnavailable();
        }

        /// <summary>
        /// Get all target tags that the given tag may have a relation *rule* to.
        /// The allowed relation rules are defined by LayerRelationRules.
        /// </summary>
        /// <param name="tagId">The Id of the tag that you want the allowed relation rules for</param>
        /// <response code="200">Returns a list of tags that the user may create a relation rule to</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/{tagId}/AllowedRelations")]
        [ProducesResponseType(typeof(List<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetAllowedRelationRulesForTag([FromRoute] int tagId)
        {
            try
            {
                return Ok(_tagManager.GetAllowedRelationRulesForTag(tagId));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all tag relations that are available for the tag instance identified by the given id.
        /// The relations are ordered descending by relevance.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <param name="id">The Id of the tag that you want the allowed relations for</param>
        /// <response code="200">Returns a list of tags</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/Instance/{id}/AvailableRelations")]
        [ProducesResponseType(typeof(List<AnnotationTagRelation>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetAvailableRelationsForInstance([FromRoute] int id)
        {
            // TODO: Waiting for relations in tag instances / documents
            return ServiceUnavailable();
        }

        #endregion

        #region POST

        /// <summary>
        /// Creates the given AnnotationTagRelation.
        /// </summary>
        /// <param name="model">The relation that should be created</param>
        /// <response code="200">Relation added</response>
        /// <response code="403">User not allowed to add a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PostTagRelation([FromBody] AnnotationTagRelationFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.AddTagRelation(model))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion

        #region PUT

        /// <summary>
        /// Modify the relation from the tag represented by {sourceId} to the tag represented by {targetId}.
        /// The new relation must be given in the body of the call.
        /// Source and target tags of the relations may *not* be changed for now.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <param name="sourceId">ID of the source tag of the relation</param>
        /// <param name="targetId">ID of the target tag of the relation</param>
        /// <param name="model">The changed AnnotationTagRelation</param>
        /// <response code="200">Relation modified</response>
        /// <response code="403">User not allowed to modify a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPut("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutTagRelation([FromQueryAttribute] int sourceId, [FromQueryAttribute] int targetId, [FromBody] AnnotationTagRelationFormModel model)
        {
            return ServiceUnavailable();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Remove the given relation from the database.
        /// </summary>
        /// <param name="model">The relation to remove</param>
        /// <response code="200">Relation removed</response>
        /// <response code="403">User not allowed to remove a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteTagRelation([FromBody] AnnotationTagRelationFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.RemoveTagRelation(model)) return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion

        // TODO: Documents may also have relations in them --> need GET, POST, PUT, DELETE etc. for relations of a document (i.e. relations between tag instances)
    }
}
