using Api.Models.Topic;
using Api.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public partial class TopicsController
    {

        /// <summary>
        /// Add an Metadata to the aattachment {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">The Id of the attachment</param>                  
        /// <param name="metadata">Metadata</param>                 
        /// <response code="200">Added Legal successfully</response>        
        /// <response code="403">User not allowed to add topic attachment</response>             
        /// <response code="401">User is denied</response>
        [HttpPost("{topicId}/Attachments/{attachmentId}/Metadata")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PostMetaData([FromRoute]int topicId, [FromRoute] int attachmentId, [FromBody]Metadata metadata)
        {
            return UpdateMetaData(topicId, attachmentId, metadata);
        }

        /// <summary>
        /// Update an Metadata to the aattachment {topicId}
        /// </summary>        
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <param name="attachmentId">The Id of the attachment</param>                  
        /// <param name="metadata">Metadata</param>                
        /// <response code="200">Added Legal successfully</response>        
        /// <response code="403">User not allowed to add topic attachment</response>             
        /// <response code="401">User is denied</response> 
        [HttpPut("{topicId}/Attachments/{attachmentId}/Metadata")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutMetaData([FromRoute]int topicId, [FromRoute] int attachmentId, [FromBody]Metadata metadata)
        {
            return UpdateMetaData(topicId, attachmentId, metadata);
        }


        private IActionResult UpdateMetaData(int topicId, int attachmentId, Metadata metadata)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _attachmentsManager.UpdateMetadata(topicId, attachmentId, User.Identity.GetUserIdentity(), metadata);
            return Ok();
        }

    }
}
