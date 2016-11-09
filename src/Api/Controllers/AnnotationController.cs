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
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAllTags()
        {
            var tags = tagManager.getAllTags();
            if (tags != null)
                return Ok(tags);
            return NotFound();
        }

        // Get api/Annotation/Tags/:id/ChildTags
        [HttpGet("Tags/{id}/ChildTags")]
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

        // Post api/Annotation/Tags/:parentId/AddChildTag/:childId
        [HttpPost("Tags/{parentId}/AddChildTag/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Post(int parentId, int childId)
        {
            bool success = tagManager.AddChildTag(parentId, childId);
            if (success)
                return Ok();
            return BadRequest();
        }


    }
}
