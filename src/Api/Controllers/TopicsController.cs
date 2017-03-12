using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using Api.Managers;
using Api.Models;
using Api.Data;
using Api.Permission;
using Api.Models.Topic;

namespace Api.Controllers
{
    public partial class TopicsController : ApiController
    {
        private readonly TopicManager _topicManager;

        private readonly TopicPermissions _topicPermissions;

        public TopicsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            _topicManager = new TopicManager(dbContext);
            _topicPermissions = new TopicPermissions(dbContext);
            TopicsAttachmentsController();
            TopicsDocumentController();
            TopicsAnalyticsController();
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
        /// <param name="pageSize">Size of the requested page</param>
        /// <response code="200">Returns PagedResults of TopicResults</response>
        /// <response code="401">User is denied</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TopicResult>), 200)]
        public IActionResult Get([FromQuery]string query, [FromQuery] string status, [FromQuery]DateTime? deadline, [FromQuery]bool onlyParents = false, [FromQuery]int page = 0, [FromQuery] int pageSize = Constants.PageSize)
        {
            var topics = _topicManager.GetAllTopics(query, status, deadline, onlyParents, page, pageSize);
            return Ok(topics);
        }

        // GET api/topics/OfUser

        /// <summary>
        /// All topics of the current user
        /// </summary>
        /// <param name="userIdenty">Represents the user Identy of the user</param>
        /// <param name="page">Represents the page</param>
        /// <param name="pageSize">Size of the requested page</param>
        /// <response code="200">Returns PagedResults of TopicResults</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("OfUser")]
        [ProducesResponseType(typeof(PagedResult<TopicResult>), 200)]
        public IActionResult GetTopicsForUser([FromQuery]string userIdenty, [FromQuery]int page = 0, [FromQuery] int pageSize = Constants.PageSize)
        {
            var topics = _topicManager.GetTopicsForUser(userIdenty ?? User.Identity.GetUserIdenty(), page, pageSize);
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
        public IActionResult Get([FromRoute]int topicId)
        {
            try
            {
                return Ok(_topicManager.GetTopicById(topicId));
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
        public IActionResult Post([FromBody]TopicFormModel model)
        {
            if (!_topicPermissions.IsAllowedToCreate(User.Identity.GetUserIdenty()))
                return Forbidden();

            if (ModelState.IsValid)
            {
                if (!TopicStatus.IsStatusValid(model.Status))
                {
                    ModelState.AddModelError("Status", "Invalid Topic Status");
                }
                else
                {
                    var result = _topicManager.AddTopic(User.Identity.GetUserIdenty(), model);
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
        public IActionResult Put([FromRoute]int topicId, [FromBody] TopicFormModel model)
        {
            if (!_topicPermissions.IsAllowedToEdit(User.Identity.GetUserIdenty(), topicId))
                return Forbidden();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO createUser is Supervisor!
            if (_topicManager.UpdateTopic(User.Identity.GetUserIdenty(), topicId, model))
                return Ok();
            return BadRequest(ModelState);
        }

        // PUT api/topic/:topicId/status

        /// <summary>
        /// Change status of a topic {topicId}
        /// </summary>
        /// <param name="topicStatus">Status of the topic {topicId}</param>                
        /// <param name="topicId">the Id of the Topic {topicId}</param>                
        /// <response code="200">The Topic {topicId} status id changed</response>        
        /// <response code="404">Resource not found</response>        
        /// <response code="403">User not allowed to change topic status</response>        
        /// <response code="401">User is denied</response>
        /// <response code="409">If some reviews are unfinnished</response>
        [HttpPut("{topicId}/Status")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 409)]
        public IActionResult ChangeStatus([FromRoute] int topicId, [FromBody] TopicStatus topicStatus)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdenty(), topicId))
                return Forbidden();

            if (!_topicManager.IsValidTopicId(topicId))
                return NotFound();

            if (!topicStatus.IsStatusValid())
            {
                ModelState.AddModelError("status", "Invalid Status");
                return BadRequest(ModelState);
            }
            if (topicStatus.IsDone() &&_topicManager.GetReviews(topicId).Any(r => !r.Status.IsReviewed()))
                    return Conflict();

            if (_topicManager.ChangeTopicStatus(User.Identity.GetUserIdenty(), topicId, topicStatus.Status))
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
        public IActionResult Delete([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAllowedToEdit(User.Identity.GetUserIdenty(), topicId))
                return Forbidden();
            if (_topicManager.DeleteTopic(topicId, User.Identity.GetUserIdenty()))
                return Ok();

            return NotFound();
        }

        #endregion


    }
}