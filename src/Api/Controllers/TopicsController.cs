using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Api.Managers;
using Api.Models;
using Api.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Api.Models.Entity;
using Api.Permission;

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
        [HttpGet]
        public IActionResult Get(string query, string status, DateTime? deadline, bool onlyParents = false, int page = 1)
        {
            var topics = topicManager.GetAllTopics(query, status, deadline, onlyParents);
            int count = topics.Count();
            var entities = topics.Skip((page - 1) * Constants.PageSize).Take(Constants.PageSize).ToList();

            return Ok(new PagedResult<Topic>(entities, page, count));
        }

        // GET api/topics
        [HttpGet("OfUser/Current")]
        public IActionResult GetTopicsForUser(int page = 1)
        {
            return GetTopicsForUser(User.Identity.GetUserId(), page);
        }

        // GET api/topics
        [HttpGet("OfUser/{userId}")]
        public IActionResult GetTopicsForUser(int userId, int page = 1)
        {
            var topics = topicManager.GetTopicsForUser(userId);
            int count = topics.Count();
            var entities = topics.Skip((page - 1) * Constants.PageSize).Take(Constants.PageSize).ToList();

            return Ok(new PagedResult<Topic>(entities, page, count));
        }

        // GET api/topics/:topicId
        [HttpGet("{topicId}")]
        public IActionResult Get(int topicId)
        {
            var topics = topicManager.GetTopicById(topicId);
            if (topics != null)
                return Ok(topics);

            return NotFound();
        }

        // GET api/topics/:id/subtopics
        [HttpGet("{topicId}/SubTopics")]
        public IActionResult GetSubTopics(int topicId)
        {
            return Ok(topicManager.GetSubTopics(topicId));
        }


        // GET api/topics/:topicId/parenttopics
        [HttpGet("{topicId}/ParentTopics")]
        public IActionResult GetParentTopics(int topicId)
        {
            return Ok(topicManager.GetParentTopics(topicId));
        }

        #endregion

        #region Get Users

        // GET api/topics/:topicId/students
        [HttpGet("{topicId}/Students")]
        public IActionResult GetTopicStudents(int topicId)
        {
            return GetTopicUsers(topicId, Role.Student);
        }

        // GET api/topics/:topicId/supervisors
        [HttpGet("{topicId}/Supervisors")]
        public IActionResult GetTopicSupervisors(int topicId)
        {
            return GetTopicUsers(topicId, Role.Supervisor);
        }

        // GET api/topics/:topicId/reviewers
        [HttpGet("{topicId}/Reviewers")]
        public IActionResult GetTopicReviewers(int topicId)
        {
            return GetTopicUsers(topicId, Role.Reviewer);
        }

        private IActionResult GetTopicUsers(int topicId, string role)
        {
            return Ok(topicManager.GetAssociatedUsersByRole(topicId, role));
        }

        #endregion

        #region Edit Topics


        // POST api/topics
        [HttpPost]
        public IActionResult Post(TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToCreate(User.Identity.GetUserId()))
                return Unauthorized();

            if (ModelState.IsValid)
            {
                if (!Status.IsStatusValid(model.Status))
                {
                    ModelState.AddModelError("Status", "Invalid Topic Status");
                } // TODO createUser is Supervisor!
                else
                {
                    var result = topicManager.AddTopic(User.Identity.GetUserId(), model);
                    if (result.Success)
                        return new ObjectResult(result);
                }
            }

            return BadRequest(ModelState);
        }

        // PUT api/topics/:topicId
        [HttpPut("{topicId}")]
        [Authorize(Roles = Role.Supervisor)]
        public IActionResult Put(int topicId, TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            if (ModelState.IsValid)
            { // TODO createUser is Supervisor!
                if (topicManager.UpdateTopic(User.Identity.GetUserId(), topicId, model))
                    return Ok(42);
            }
            return BadRequest(ModelState);
        }

        // PUT api/topic/:topicId/status
        [HttpPut("{topicId}/Status")]
        public IActionResult ChangeStatus(int topicId, string status)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            if (!Status.IsStatusValid(status))
                ModelState.AddModelError("status", "Invalid Status");
            else if (topicManager.ChangeTopicStatus(User.Identity.GetUserId(), topicId, status))
                return Ok();

            return BadRequest(ModelState);
        }


        // DELETE api/topics/:id
        [HttpDelete("{topicId}")]
        public IActionResult Delete(int topicId)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Unauthorized();
            if (topicManager.DeleteTopic(topicId))
                return Ok();

            return BadRequest();
        }

        #endregion

        #region Attachments


        // GET api/topics/:id/attachments
        [HttpGet("{topicId}/Attachments")]
        public IActionResult GetAttachments(int topicId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            var attachments = attachmentsManager.GetAttachments(topicId);
            if (attachments != null)
                return Ok(attachments);
            return NotFound();
        }


        [HttpGet("{topicId}/Attachments/{attachmentId}")]
        public IActionResult GetAttachmet(int topicId, int attachmentId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            var attachment = attachmentsManager.GetAttachmentById(topicId, attachmentId);
            if (attachment != null)
            {
                string contentType = MimeKit.MimeTypes.GetMimeType(Path.Combine(Constants.AttatchmentPath, attachment.Path));
                return base.File(Path.Combine(Constants.AttatchmentFolder, attachment.Path), contentType);
            }
            return BadRequest();
        }

        // POST api/topics/:id/attachments
        [HttpPost("{topicId}/Attachments")]
        public IActionResult PostAttachment(int topicId, AttatchmentFormModel model, IFormFile file)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            if (file == null)
                ModelState.AddModelError("file", "File is null");


            if (ModelState.IsValid)
            {
                var result = attachmentsManager.CreateAttachment(topicId, User.Identity.GetUserId(), model, file);
                if (result.Success)
                    return new ObjectResult(result);
            }

            return BadRequest(ModelState);
        }

        // DELETE api/topics/:id/attachments
        [HttpDelete("{topicId}/Attachments/{attachmentId}")]
        public IActionResult DeleteAttachment(int topicId, int attachmentId)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            if (attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return BadRequest();
        }

        #endregion

        #region Permission

        [HttpGet("All/Permission/IsAllowedToCreate")]
        public IActionResult IsAllowedToCreate()
        {
            if (topicPermissions.IsAllowedToCreate(User.Identity.GetUserId()))
                return Ok();
            return Unauthorized();
        }

        [HttpGet("{topicId}/Permission/IsAssociatedTo")]
        public IActionResult IsAssociatedTo(int topicId)
        {
            if (topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Ok();
            return Unauthorized();
        }

        [HttpGet("{topicId}/Permission/IsAllowedToEdit")]
        public IActionResult IsAllowedToEdit(int topicId)
        {
            if (topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Ok();
            return Unauthorized();
        }

        #endregion
    }
}
