using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Shared;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class TopicsController
    {
        private readonly AttachmentsManager _attachmentsManager;

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
        public async Task<IActionResult> GetAttachmentsAsync([FromRoute]int topicId)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            var attachments = _attachmentsManager.GetAttachments(topicId);
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
        [ProducesResponseType(typeof(StringWrapper), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetAttachmentAsync([FromRoute]int topicId, [FromRoute]int attachmentId)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            try
            {
                var attachment = _attachmentsManager.GetAttachmentById(attachmentId);
                string fileName = Path.Combine(Constants.AttachmentFolder, topicId.ToString(), attachment.Path);
                var hash = DownloadManager.AddFile(fileName, HttpContext.Connection.RemoteIpAddress);
                return Ok(new StringWrapper { Value = hash });
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create an attachment to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="model">contains details about Topic attachment</param>                              
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
        public async Task<IActionResult> PostAttachmentAsync([FromRoute]int topicId, [FromBody]AttachmentFormModel model)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _attachmentsManager.CreateAttachment(topicId, User.Identity.GetUserIdentity(), model);

            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }


        // POST api/topics/:id/attachments

        /// <summary>
        /// Add an file to the attachment to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">Id of the attachment</param>                
        /// <param name="file">The file to be attached with the topic</param>                
        /// <response code="200">Added attachment {attachmentId} successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to add topic attachment</response>        
        /// <response code="500">Internal server error</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Attachments/{attachmentId}")]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(EntityResult), 500)]
        public async Task<IActionResult> PutAttachmentAsync([FromRoute]int topicId, [FromRoute] int attachmentId, [FromForm]IFormFile file)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (file == null)
                ModelState.AddModelError("file", "File is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _attachmentsManager.PutAttachmentAsync(attachmentId, User.Identity.GetUserIdentity(), file);

            return result.Success
                ? Ok(result) as IActionResult
                : NotFound(result);
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
        public async Task<IActionResult> DeleteAttachmentAsync([FromRoute]int topicId, [FromRoute] int attachmentId)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (_attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return NotFound();
        }
    }
}