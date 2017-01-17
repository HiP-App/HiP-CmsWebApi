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
    public partial class TopicsController : ApiController
    {
        private TopicManager topicManager;

        private TopicPermissions topicPermissions;

        public TopicsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicManager = new TopicManager(dbContext);
            topicPermissions = new TopicPermissions(dbContext);
            TopicsAttachmentsController(dbContext);
            TopicsDocumentController(dbContext);
        }

        #region GET topics

        // GET api/topics

        /// <summary>
        /// All topics matching query, status and deadline
        /// </summary>   
        /// <param name="query">Topics containing query in Title and Description</param>
        /// <param name="status">Topics in status </param>
        /// <param name="deadline">Topics with deadline </param>
        /// <param name="onlyParents">Indicates to get only parent topics</param>
        /// <param name="page">Represents the page</param>
        /// <response code="200">Returns PagedResults of TopicResults</response>
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TopicResult>), 200)]
        public IActionResult Get(string query, string status, DateTime? deadline, bool onlyParents = false, int page = 1)
        {
            var topics = topicManager.GetAllTopics(query, status, deadline, onlyParents, page);
            return Ok(topics);
        }

        // GET api/topics/OfUser/Current

        /// <summary>
        /// All topics of the current user
        /// </summary>
        /// <param name="page">Represents the page</param>
        /// <response code="200">Returns PagedResults of TopicResults</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("OfUser/Current")]
        [ProducesResponseType(typeof(PagedResult<TopicResult>), 200)]
        public IActionResult GetTopicsForUser(int page = 1)
        {
            return GetTopicsForUser(User.Identity.GetUserId(), page);
        }

        // GET api/topics/OfUser/:userId

        /// <summary>
        /// All topics of the user {userId}
        /// </summary>
        /// <param name="userId">Represents the user Id of the user</param>
        /// <param name="page">Represents the page</param>
        /// <response code="200">Returns PagedResults of TopicResults</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("OfUser/{userId}")]
        [ProducesResponseType(typeof(PagedResult<TopicResult>), 200)]
        public IActionResult GetTopicsForUser(int userId, int page = 1)
        {
            var topics = topicManager.GetTopicsForUser(userId, page);
            return Ok(topics);
        }

        // GET api/topics/:topicId

        /// <summary>
        /// Retrieves the topic {topicId}
        /// </summary>
        /// <param name="topicId">Represents the user Id of the user</param>        
        /// <response code="200">Returns the topic {topicId}</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}")]
        public IActionResult Get(int topicId)
        {
            try
            {
                return Ok(topicManager.GetTopicById(topicId));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
        #endregion

        #region Edit Topics

        // POST api/topics

        /// <summary>
        /// Add new topic
        /// </summary>
        /// <param name="model">Contains information about topic</param>                
        /// <response code="200">A new Topic is added</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to add topic</response>        
        /// <response code="401">User is denied</response>
        [HttpPost]
        [ProducesResponseType(typeof(EntityResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Post(TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToCreate(User.Identity.GetUserId()))
                return Forbidden();

            if (ModelState.IsValid)
            {
                if (!Status.IsStatusValid(model.Status))
                {
                    ModelState.AddModelError("Status", "Invalid Topic Status");
                }
                else
                {
                    var result = topicManager.AddTopic(User.Identity.GetUserId(), model);
                    if (result.Success)
                        return Ok(result);
                }
            }

            return BadRequest(ModelState);
        }

        // PUT api/topics/:topicId

        /// <summary>
        /// Edit a topic {topicId}
        /// </summary>
        /// <param name="model">Contains information about the topic</param>                
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">The Topic {topicId} is added</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Put(int topicId, TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (ModelState.IsValid)
            { // TODO createUser is Supervisor!
                if (topicManager.UpdateTopic(User.Identity.GetUserId(), topicId, model))
                    return Ok();
            }
            return BadRequest(ModelState);
        }

        // PUT api/topic/:topicId/status

        /// <summary>
        /// Change status of a topic {topicId}
        /// </summary>
        /// <param name="status">Status of the topic {topicId}</param>                
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">The Topic {topicId} status id changed</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to change topic status</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Status")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult ChangeStatus(int topicId, string status)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();

            if (!Status.IsStatusValid(status))
                ModelState.AddModelError("status", "Invalid Status");
            else if (topicManager.ChangeTopicStatus(User.Identity.GetUserId(), topicId, status))
                return Ok();

            return NotFound();
        }


        // DELETE api/topics/:id

        /// <summary>
        /// Delete a topic {topicId}
        /// </summary>
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">The Topic {topicId} is deleted</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to delete topic</response>        
        /// <response code="401">User is denied</response>
        [HttpDelete("{topicId}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult Delete(int topicId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Forbidden();
            if (topicManager.DeleteTopic(topicId, User.Identity.GetUserId()))
                return Ok();

            return NotFound();
        }

        #endregion


    }
}