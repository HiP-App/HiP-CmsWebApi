using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.Extensions.Logging;
using Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Api.Models.AnnotationTag;

namespace Api.Controllers
{
    public class AnnotationController : ApiController
    {
        AnnotationTagManager tagManager;
        public AnnotationController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            tagManager = new AnnotationTagManager(dbContext);
        }

        // Get api/annotation/Tags
        [HttpGet("Tags")]
        [ProducesResponseType(typeof(List<AnnotationTagResult>), 200)]
        [ProducesResponseType(typeof(void), 201)]
        public IActionResult GetAllTags(bool IncludeDeleted = false)
        {
            var tags = tagManager.getAllTags(IncludeDeleted);
            return Ok(tags);
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
        
        // Post api/Annotation/Tags
        [HttpPost("Tags")]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [ProducesResponseType(typeof(void), 500)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(AnnotationTagFormModel tag)
        {
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
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(int parentId, int childId)
        {
            bool success = tagManager.AddChildTag(parentId, childId);
            if (success)
                return Ok();
            return BadRequest();
        }

        [HttpDelete("Tags/{Id}")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult Delete(int Id)
        {
            bool success = tagManager.DeleteTag(Id);
            if (success)
                return Ok();
            return Ok(null);
        }

        [HttpDelete("Tags/{parentId}/ChildTags/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult DeleteChildOf(int parentId, int childId)
        {
            bool success = tagManager.RemoveChildTag(parentId, childId);
            if (success)
                return Ok();
            return Ok(null);
        }


    }
}
