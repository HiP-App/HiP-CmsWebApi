using System;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Managers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
{
    public partial class TopicsController
    {
        private readonly DocumentManager _documentManager;

        /// <summary>
        /// The Document of the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">The document of the Topic {topicId}</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to get topic attachments</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Document")]
        [ProducesResponseType(typeof(DocumentResult), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetDocumentAsync([FromRoute]int topicId)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            try
            {
                return Ok(new DocumentResult(_documentManager.GetDocumentById(topicId)));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add htmlContent to the topic {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="htmlContent">contains the content as HTML</param>                               
        /// <response code="200">Added content successfully</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to add topic content</response>             
        /// <response code="401">User is denied</response>
        [HttpPost("{topicId}/Document")]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public async Task<IActionResult> PostDocumentAsync([FromRoute]int topicId, [FromBody]HtmlContentModel htmlContent)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _documentManager.UpdateDocument(topicId, User.Identity.GetUserIdentity(), htmlContent.HtmlContent);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete the Document of {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                         
        /// <response code="200">Document for {topicId} deleted successfully</response>           
        /// <response code="403">User not allowed to delete topic attachment</response>                
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/Document")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> DeleteDocumentAsync([FromRoute]int topicId)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (_documentManager.DeleteDocument(topicId))
                return Ok();
            return NotFound();
        }
    }
}