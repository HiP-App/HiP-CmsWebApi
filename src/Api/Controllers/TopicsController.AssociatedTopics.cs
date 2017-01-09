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

        #region GET
        // GET api/topics/:topicId/subtopics

        /// <summary>
        /// Retrieves the subtopics of the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/SubTopics")]
        public IActionResult GetSubTopics(int topicId)
        {
            return Ok(topicManager.GetSubTopics(topicId));
        }


        // GET api/topics/:topicId/parenttopics

        /// <summary>
        /// Retrieves the parent topics of the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/ParentTopics")]
        public IActionResult GetParentTopics(int topicId)
        {
            return Ok(topicManager.GetParentTopics(topicId));
        }

        #endregion

        #region PUT

        // PUT api/topics/:topicId/ParentTopics/:parentId

        /// <summary>
        /// Associates the topic {topicId} with parent topic {parentId}
        /// </summary>
        /// <param name="topicId">The Id of the topic to be associated</param>
        /// <param name="parentId">The Id of the parent topic</param>
        /// <response code="200">Topics {topicId} and {parentId} associated successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to associate topics</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/ParentTopics/{parentId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(EntityResult), 403)]
        public IActionResult PutParentTopics(int topicId, int parentId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Forbidden();

            var result = topicManager.AssociateTopic(parentId, topicId);
            if (result.Success)
                return Ok();
            return BadRequest(result);
        }

        // PUT api/topics/:topicId/SubTopics/:parentId

        /// <summary>
        /// Associates the topic {topicId} with child topic {childId}
        /// </summary>
        /// <param name="topicId">The Id of the topic to be associated</param>
        /// <param name="childId">The Id of the child topic</param>
        /// <response code="200">Topics {topicId} and {childId} associated successfully</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to associate topics</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/SubTopics/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(EntityResult), 403)]
        public IActionResult PutSubTopics(int topicId, int childId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Forbidden();

            var result = topicManager.AssociateTopic(topicId, childId);
            if (result.Success)
                return Ok();
            return BadRequest(result);
        }

        #endregion

        #region DELETE

        // DELETE api/topics/:id/ParentTopics/:parentId

        /// <summary>
        /// Deletes the associated topic {topicId} and parent topic {parentId}
        /// </summary>
        /// <param name="topicId">The Id of the child topic</param>
        /// <param name="parentId">The Id of the parent topic</param>
        /// <response code="200">Deleted Associated parent {parentId} and child {topicId}</response>        
        /// <response code="404">Not found</response>                      
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/ParentTopics/{parentId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteParentTopics(int topicId, int parentId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Unauthorized();
            if (topicManager.DeleteAssociated(parentId, topicId))
                return Ok();

            return NotFound();
        }

        // DELETE api/topics/:id/SubTopics/:childId

        /// <summary>
        /// Deletes the associated topic {topicId} and child topic {childId}
        /// </summary>
        /// <param name="topicId">The Id of the parent topic</param>
        /// <param name="childId">The Id of the child topic</param>
        /// <response code="200">Deleted Associated parent {topicId} and child {childId}</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="404">Not found</response>     
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}/SubTopics/{childId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteSubTopics(int topicId, int childId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Forbidden();
            if (topicManager.DeleteAssociated(topicId, childId))
                return Ok();

            return NotFound();
        }

        #endregion

    }
}