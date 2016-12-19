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
    public class TopicsController : ApiController
    {
        private TopicManager topicManager;
        private AttachmentsManager attachmentsManager;

        private TopicPermissions topicPermissions;

        public TopicsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicManager = new TopicManager(dbContext);
            attachmentsManager = new AttachmentsManager(dbContext);
            topicPermissions = new TopicPermissions(dbContext);
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

        #region AssociatedTopics

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

        #endregion

        #region Get Users

        // GET api/topics/:topicId/students

        /// <summary>
        /// All students associated with the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <response code="200">A list of students assocaited with the topic {topicId}</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Students")]
        [ProducesResponseType(typeof(IEnumerable<UserResult>), 200)]
        public IActionResult GetTopicStudents(int topicId)
        {
            return GetTopicUsers(topicId, Role.Student);
        }

        // GET api/topics/:topicId/supervisors

        /// <summary>
        /// All supervisors associated with the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <response code="200">A list of supervisors assocaited with the topic {topicId}</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Supervisors")]
        [ProducesResponseType(typeof(IEnumerable<UserResult>), 200)]
        public IActionResult GetTopicSupervisors(int topicId)
        {
            return GetTopicUsers(topicId, Role.Supervisor);
        }

        // GET api/topics/:topicId/reviewers

        /// <summary>
        /// All reviewers associated with the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <response code="200">A list of reviewers assocaited with the topic {topicId}</response>        
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Reviewers")]
        [ProducesResponseType(typeof(IEnumerable<UserResult>), 200)]
        public IActionResult GetTopicReviewers(int topicId)
        {
            return GetTopicUsers(topicId, Role.Reviewer);
        }

        private IActionResult GetTopicUsers(int topicId, string role)
        {
            return Ok(topicManager.GetAssociatedUsersByRole(topicId, role));
        }

        #endregion

        #region PUT users

        // PUT api/topics/:topicId/students

        /// <summary>
        /// Edit assigned students for the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <param name="users">An array of users</param>        
        /// <response code="200">Edited assigned students for the topic {topicId}</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Students")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutTopicStudents(int topicId, int[] users)
        {
            return PutTopicUsers(topicId, Role.Student, users);
        }

        // PUT api/topics/:topicId/Supervisors

        /// <summary>
        /// Edit supervisors for the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <param name="users">An array of users</param>        
        /// <response code="200">Edited supervisors for the topic {topicId}</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Supervisors")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutTopicSupervisors(int topicId, int[] users)
        {
            return PutTopicUsers(topicId, Role.Supervisor, users);
        }

        // PUT api/topics/:topicId/Reviewers

        /// <summary>
        /// Edit reviewers for the topic {topicId}
        /// </summary>
        /// <param name="topicId">The Id of the topic</param>        
        /// <param name="users">An array of users</param>        
        /// <response code="200">Edited reviewers for the topic {topicId}</response>        
        /// <response code="400">Request incorrect</response>        
        /// <response code="403">User not allowed to edit topic</response>        
        /// <response code="401">User is denied</response>
        [HttpPut("{topicId}/Reviewers")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult PutTopicReviewers(int topicId, int[] users)
        {
            return PutTopicUsers(topicId, Role.Reviewer, users);
        }

        private IActionResult PutTopicUsers(int topicId, string role, int[] users)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (topicManager.ChangeAssociatedUsersByRole(User.Identity.GetUserId(), topicId, role, users))
                return Ok();
            return BadRequest();
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