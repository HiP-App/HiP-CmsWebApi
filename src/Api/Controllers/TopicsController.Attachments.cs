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
        private AttachmentsManager attachmentsManager;

        private void TopicsAttachmentsController(CmsDbContext dbContext)
        {
            attachmentsManager = new AttachmentsManager(dbContext);
        }

        #region Attachments

        // GET api/topics/:id/attachments

        /// <summary>
        /// All attachments of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">A list of attachments of the Topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachments</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Attachments")]
        [ProducesResponseType(typeof(IEnumerable<TopicAttachmentResult>), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAttachments(int topicId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            var attachments = attachmentsManager.GetAttachments(topicId);
            if (attachments != null)
                return Ok(attachments);
            return NotFound();
        }

        // topicId not needed, but looks better at the api

        /// <summary>
        /// Specific attachment {attachmentId} of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">the Id of the Topic attachment</param>                
        /// <response code="200">An attachment {attachmentId} of the topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachment</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAttachmet(int topicId, int attachmentId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            try
            {
                var attachment = attachmentsManager.GetAttachmentById(attachmentId);
                string fileName = Path.Combine(Constants.AttatchmentFolder, topicId.ToString(), attachment.Path);
                var hash = DownloadManager.AddFile(fileName, HttpContext.Connection.RemoteIpAddress);
                return Ok(hash);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // POST api/topics/:id/attachments

        /// <summary>
        /// Add an attachment to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="model">contains details about Topic attachment</param>                
        /// <param name="file">The file to be attached with the topic</param>                
        /// <response code="200">Added attachment {attachmentId} successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to add topic attachment</response>        
        /// <response code="500">Internal server error</response>        
        /// <response code="401">User is denied</response>
        [HttpPost("{topicId}/Attachments")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(EntityResult), 500)]
        public IActionResult PostAttachment(int topicId, AttatchmentFormModel model, IFormFile file)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (file == null)
                ModelState.AddModelError("file", "File is null");

            if (ModelState.IsValid)
            {
                var result = attachmentsManager.CreateAttachment(topicId, User.Identity.GetUserId(), model, file);
                if (result.Success)
                    return Ok(result);
                return InternalServerError(result);
            }

            return BadRequest(ModelState);
        }

        // DELETE api/topics/:id/attachments

        /// <summary>
        /// Delete an attachment {attachmentId} in the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">The Id of the attachment</param>                
        /// <response code="200">Attachment {attachmentId} deleted successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to delete topic attachment</response>                
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteAttachment(int topicId, int attachmentId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return NotFound();
        }

        #endregion

    }
}