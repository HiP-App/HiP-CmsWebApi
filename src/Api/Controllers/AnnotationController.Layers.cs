using System;
using System.Collections.Generic;
using Api.Models.AnnotationTag;
using Api.Models.Entity.Annotation;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Layer = Api.Models.Entity.Annotation.Layer;

namespace Api.Controllers
{
    public partial class AnnotationController
    {
        
        /// <summary>
        /// All tag layers saved in the system.
        /// </summary>
        /// <response code="200">A list of tag layers</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Layers")]
        [ProducesResponseType(typeof(IEnumerable<Layer>), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public IActionResult GetAllLayers()
        {
            return Ok(_tagManager.GetAllLayers());
        }

        /// <summary>
        /// All layer relation rules saved in the system.
        /// </summary>
        /// <response code="200">A list of layer relation rules</response>
        /// <response code="401">User is denied</response>
        [HttpGet("Layers/RelationRules")]
        [ProducesResponseType(typeof(IEnumerable<LayerRelationRule>), 200)]
        [ProducesResponseType(typeof(void), 401)]
        public IActionResult GetAllLayerRelationRules()
        {
            return Ok(_tagManager.GetAllLayerRelationRules());
        }

        /// <summary>
        /// Create a new layer relation rule.
        /// </summary>
        /// <response code="200">Rule created</response>
        /// <response code="403">User not allowed to create a layer relation rule</response>
        /// <response code="404">Layers not found</response>
        [HttpPost("Layers/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PostLayerRelationRule([FromBody] RelationFormModel model)
        {
            if (!_annotationPermissions.IsAllowedToCreateRelationRules(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.AddLayerRelationRule(model))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Modify the layer relation rule for the layer represented by {sourceId} to the layer represented by {targetId}.
        /// The new relation must be given in the body of the call.
        /// Source and target layers of the relations may *not* be changed for now.
        /// </summary>
        /// <param name="original">The layer relation rule that you want to modify</param>
        /// <param name="changed">The changed layer relation rule</param>
        /// <response code="200">Relation modified</response>
        /// <response code="403">User not allowed to modify a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpPut("Layers/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult PutLayerRelationRule([FromBody] RelationFormModel original, [FromBody] RelationFormModel changed)
        {
            if (!_annotationPermissions.IsAllowedToCreateRelationRules(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.ChangeLayerRelationRule(original, changed))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove the layer relation rule for the layer represented by {sourceId} to the layer represented by {targetId}.
        /// </summary>
        /// <param name="sourceId">ID of the source layer of the relation</param>
        /// <param name="targetId">ID of the target layer of the relation</param>
        /// <response code="200">Relation removed</response>
        /// <response code="403">User not allowed to delete a relation</response>
        /// <response code="400">Request was misformed</response>
        [HttpDelete("Layers/RelationRule")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult DeleteLayerRelationRule([FromQueryAttribute] int sourceId, [FromQueryAttribute] int targetId)
        {
            if (!_annotationPermissions.IsAllowedToCreateRelationRules(User.Identity.GetUserId()))
                return Forbid();

            try
            {
                if (_tagManager.RemoveLayerRelationRule(sourceId, targetId))
                    return Ok();
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }
    }
}
