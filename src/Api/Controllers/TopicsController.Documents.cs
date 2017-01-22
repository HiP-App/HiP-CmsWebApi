using System;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using Api.Managers;
using Api.Models.Topic;

namespace Api.Controllers
{
    public partial class TopicsController
    {
        private DocumentManager _documentManager;

        private void TopicsDocumentController()
        {
            _documentManager = new DocumentManager(dbContext);
        }

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
        public IActionResult GetDocument([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

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
        public IActionResult PostDocument([FromRoute]int topicId, [FromBody]HtmlContentModel htmlContent)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (ModelState.IsValid)
            {
                var result = _documentManager.UpdateDocument(topicId, User.Identity.GetUserId(), htmlContent.HtmlContent);
                if (result.Success)
                    return Ok(result);
            }

            return BadRequest(ModelState);
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
        public IActionResult DeleteDocument([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (_documentManager.DeleteDocument(topicId))
                return Ok();
            return NotFound();
        }


    }
}