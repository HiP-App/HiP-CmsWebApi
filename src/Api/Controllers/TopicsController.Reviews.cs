using System.Collections.Generic;
using Api.Models.Topic;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;

namespace Api.Controllers
{
    public partial class TopicsController
    {

        /// <summary>
        /// Get Status of all reviews for {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>            
        /// <response code="200">List of Reviews</response>              
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        /// <response code="404">Topic not found</response>
        [ProducesResponseType(typeof(IEnumerable<TopicReviewResult>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [HttpGet("{topicId}/ReviewStatus")]
        public IActionResult GetReviewStatus([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdenty(), topicId))
                return Forbidden();
            if (!_topicManager.IsValidTopicId(topicId))
                return NotFound();

            return Ok(_topicManager.GetReviews(topicId));
        }


        /// <summary>
        /// Change the Status of the review {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <param name="status">ReviewStatus</param>        
        /// <response code="200">Edited reviewes for the topic {topicId}</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        /// <response code="404">Topic not found</response>
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [HttpPut("{topicId}/ReviewStatus")]
        public IActionResult PutReviewStatus([FromRoute] int topicId, [FromBody] TopicReviewStatus status)
        {
            if (!_topicPermissions.IsReviewer(User.Identity.GetUserIdenty(), topicId))
                return Forbidden();
            if (!ModelState.IsValid || !status.IsStatusValid())
                return BadRequest();
            if(!_topicManager.IsValidTopicId(topicId))
                return NotFound();

            if (_topicManager.ChangeReviewStatus(User.Identity.GetUserIdenty(), topicId, status))
                return Ok();

            return BadRequest();
        }
    }
}
