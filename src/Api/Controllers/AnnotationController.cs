using System.Collections.Generic;
using Api.Data;
using Microsoft.Extensions.Logging;
using Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Api.Models.AnnotationTag;
using Api.Permission;
using Api.Utility;

namespace Api.Controllers
{
    public class AnnotationController : ApiController
    {
        private AnnotationTagManager tagManager;
        private AnnotationPermissions annotationPermissions;

        public AnnotationController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            tagManager = new AnnotationTagManager(dbContext);
            annotationPermissions = new AnnotationPermissions(dbContext);
        }

        #region GET

        // Get api/annotation/Tags
        [HttpGet("Tags")]
        [ProducesResponseType(typeof(List<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 201)]
        public IActionResult GetAllTags(bool IncludeDeleted = false)
        {
            var tags = tagManager.getAllTags(IncludeDeleted);
            return Ok(tags);
        }

        // Get api/Annotation/Tags/:id
        [HttpGet("Tags/{id}")]
        [ProducesResponseType(typeof(AnnotationTagResult), 200)]
        [ProducesResponseType(typeof(void), 201)]
        public IActionResult GetTag(int id)
        {
            var tag = tagManager.getTag(id);
            return Ok(tag);
        }


        // Get api/Annotation/Tags/:id/ChildTags
        [HttpGet("Tags/{id}/ChildTags")]
        [ProducesResponseType(typeof(List<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        public IActionResult GetChildTagsOf(int id)
        {
            var tags = tagManager.getChildTagsOf(id);
            return Ok(tags);
        }

        #endregion

        #region POST

        // Post api/Annotation/Tags
        [HttpPost("Tags")]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [ProducesResponseType(typeof(void), 500)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(AnnotationTagFormModel tag)
        {
            if (!annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserId()))
                return Forbid();

            if (ModelState.IsValid)
            {
                var result = tagManager.AddTag(tag);
                if (result.Success)
                    return Ok(result);
                return new StatusCodeResult(500);
            }
            return BadRequest();
        }

        // Post api/Annotation/Tags/:parentId/ChildTags/:childId
        [HttpPost("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(int parentId, int childId)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.AddChildTag(parentId, childId);
            if (success)
                return Ok();
            return BadRequest();
        }

        #endregion

        #region PUT

        // Put api/Annotation/Tags/:Id
        [HttpPut("Tags/{Id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PutTag(int Id, AnnotationTagFormModel model)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            var success = tagManager.EditTag(model, Id);
            if (success)
                return Ok();
            return NotFound();
        }

        #endregion

        #region DELETE

        // Delete api/Annotation/Tags/:Id
        [HttpDelete("Tags/{Id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Delete(int Id)
        {
            if (!annotationPermissions.IsAllowedToCreateTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.DeleteTag(Id);
            if (success)
                return Ok();
            return Ok(null);
        }

        [HttpDelete("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult DeleteChildOf(int parentId, int childId)
        {
            if (!annotationPermissions.IsAllowedToEditTags(User.Identity.GetUserId()))
                return Forbid();

            bool success = tagManager.RemoveChildTag(parentId, childId);
            if (success)
                return Ok();
            return Ok(null);
        }

        #endregion
    }
}
