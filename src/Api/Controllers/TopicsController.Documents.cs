using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using System.Linq;
using Api.Managers;
using Api.Models;
using Api.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Api.Permission;
using Api.Models.User;
using System.Collections.Generic;
using Api.Models.Topic;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    public partial class TopicsController
    {
        private DocumentManager documentManager;

        protected void TopicsDocumentController(CmsDbContext dbContext)
        {
            documentManager = new DocumentManager(dbContext);
        }

        /// <summary>
        /// All attachments of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">A list of attachments of the Topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachments</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Document")]
        [ProducesResponseType(typeof(DocumentResult), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetDocument(int topicId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            try
            {
                return Ok(new DocumentResult(documentManager.GetDocumentById(topicId)));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }


        [HttpPost("{topicId}/Document")]

        public IActionResult PostDocument(int topicId, String htmlContent)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (ModelState.IsValid && !String.IsNullOrEmpty(htmlContent))
            {
                var result = documentManager.UpdateDocument(topicId, User.Identity.GetUserId(), htmlContent);
                if (result.Success)
                    return Ok(result);
            }

            return BadRequest(ModelState);
        }


        [HttpPut("{topicId}/Document")]

        public IActionResult PutDocument(int topicId, String htmlContent)
        {
            if (ModelState.IsValid && !String.IsNullOrEmpty(htmlContent))
            {
                var result = documentManager.UpdateDocument(topicId, User.Identity.GetUserId(), htmlContent);
                if (result.Success)
                    return Ok(result);
            }

            return BadRequest(ModelState);
        }


        /// <summary>
        /// Delete the Legal {attachmentId} in the attachment {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                         
        /// <response code="200">Attachment {attachmentId} deleted successfully</response>           
        /// <response code="403">User not allowed to delete topic attachment</response>                
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/Document")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteDocument(int topicId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (documentManager.DeleteDocument(topicId))
                return Ok();
            return NotFound();
        }


    }
}