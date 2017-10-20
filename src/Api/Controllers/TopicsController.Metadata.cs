using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Controllers
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
        public async Task<IActionResult> PostMetaDataAsync([FromRoute]int topicId, [FromRoute] int attachmentId, [FromBody]Metadata metadata)
        {
            return await UpdateMetaDataAsync(topicId, attachmentId, metadata);
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
        public async Task<IActionResult> PutMetaDataAsync([FromRoute]int topicId, [FromRoute] int attachmentId, [FromBody]Metadata metadata)
        {
            return await UpdateMetaDataAsync(topicId, attachmentId, metadata);
        }


        private async Task<IActionResult> UpdateMetaDataAsync(int topicId, int attachmentId, Metadata metadata)
        {
            if (!(await _topicPermissions.IsAssociatedToAsync(User.Identity.GetUserIdentity(), topicId)))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _attachmentsManager.UpdateMetadata(topicId, attachmentId, User.Identity.GetUserIdentity(), metadata);
            return Ok();
        }

    }
}
