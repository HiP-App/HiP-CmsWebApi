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
        /// Get all existing tag instance relations.
        /// </summary>
        /// <response code="200">Returns a list of tag relations</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/Relations")]
        [ProducesResponseType(typeof(IEnumerable<RelationResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetRelations()
        {
            try
            {
                return Ok(_tagManager.GetAllTagInstanceRelations());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all existing tag relations for the tag represented by the given id.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <param name="tagId">The Id of the tag that you want the relations for</param>
        /// <response code="200">Returns a list of tag relations</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/{tagId}/Relations")]
        [ProducesResponseType(typeof(IEnumerable<RelationResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetRelationsForId([FromRoute] int tagId)
        {
            try
            {
                return Ok(_tagManager.GetAllTagInstanceRelations(tagId));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all target tags that the given tag may have a relation *rule* to.
        /// The allowed relation rules are defined by LayerRelationRules.
        /// </summary>
        /// <param name="tagId">The Id of the tag that you want the allowed relation rules for</param>
        /// <response code="200">Returns a list of tags that the user may create a relation rule to</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/{tagId}/AllowedRelationRuleTargets")]
        [ProducesResponseType(typeof(IEnumerable<TagResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetAllowedRelationRuleTargetsForTag([FromRoute] int tagId)
        {
            try
            {
                return Ok(_tagManager.GetAllowedRelationRuleTargetsForTag(tagId));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all tag relations that are available for the tag instance identified by the given id.
        /// These depend on the configured tag relation rules.
        /// The relations are ordered descending by relevance.
        /// </summary>
        /// <param name="tagInstanceId">The Id of the tag instance that you want the allowed relations for</param>
        /// <response code="200">Returns a list of tag instances</response>
        /// <response code="400">Request was misformed</response>
        [HttpGet("Tags/Instance/{id}/AllowedRelations")]
        [ProducesResponseType(typeof(IEnumerable<RelationResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult GetAllowedRelationsForInstance([FromRoute] int tagInstanceId)
        {
            try
            {
                var result = _tagManager.GetAllowedRelationsForTagInstance(tagInstanceId);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Creates the given TagRelation.
        /// </summary>
        /// <param name="model">The relation that should be created</param>
        /// <response code="200">Relation added</response>
        /// <response code="403">User not allowed to add a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PostTagRelation([FromBody] RelationFormModel model)
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

        /// <summary>
        /// Creates the given AnnotationTagRelationRule.
        /// </summary>
        /// <param name="model">The relation rule that should be created</param>
        /// <response code="200">Relation rule added</response>
        /// <response code="403">User not allowed to add a relation rule</response>
        /// <response code="400">Request was misformed</response>
        [HttpPost("Tags/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PostTagRelationRule([FromBody] RelationFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.AddTagRelationRule(model))
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
        /// Modify the tag relation rule represented by the given original model to the tag represented by {targetId}.
        /// The new relation must be given in the body of the call.
        /// Source and target tags of the relations may *not* be changed for now.
        /// NOT IMPLEMENTED YET.
        /// </summary>
        /// <param name="original">The model describing the original relation</param>
        /// <param name="changed">The model describing the changed relation</param>
        /// <response code="200">Relation modified</response>
        /// <response code="403">User not allowed to modify a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPut("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutTagRelation([FromBody] RelationFormModel original, [FromBody] RelationFormModel changed)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.ChangeTagRelation(original, changed))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Modify the tag relation rule from the tag represented by {sourceId} to the tag represented by {targetId}.
        /// The new relation rule must be given in the body of the call.
        /// Source and target tags of the relations may *not* be changed for now.
        /// </summary>
        /// <param name="original">The model describing the original relation rule</param>
        /// <param name="changed">The model describing the changed relation rule</param>
        /// <response code="200">Relation rule modified</response>
        /// <response code="403">User not allowed to modify a relation rule</response>
        /// <response code="400">Request was misformed</response>
        [HttpPut("Tags/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutTagRelationRule([FromBody] RelationFormModel original, [FromBody] RelationFormModel changed)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.ChangeTagRelationRule(original, changed))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Remove the given tag relation from the database.
        /// </summary>
        /// <param name="model">The relation to remove</param>
        /// <response code="200">Relation removed</response>
        /// <response code="403">User not allowed to remove a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Tags/Relation")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteTagRelation([FromBody] RelationFormModel model)
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

        /// <summary>
        /// Remove the given tag relation rule from the database.
        /// </summary>
        /// <param name="model">The relation rule to remove</param>
        /// <response code="200">Relation rule removed</response>
        /// <response code="403">User not allowed to remove a relation rule</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Tags/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteTagRelationRule([FromBody] RelationFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.RemoveTagRelationRule(model))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        #endregion
    }
}
