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


        // GET api/topics/:id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), id))
                return Unauthorized();

            var topics = topicManager.GetTopicById(id);
            if (topics != null)
                return Ok(topics);

            return NotFound();
        }

        // GET api/topics/:id/subtopics
        [HttpGet("{id}/SubTopics")]
        public IActionResult GetSubTopics(int id)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), id))
                return Unauthorized();

            return Ok(topicManager.GetSubTopics(id));
        }


        // GET api/topics/:id/parenttopics
        [HttpGet("{id}/ParentTopics")]
        public IActionResult GetParentTopics(int id)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), id))
                return Unauthorized();

            return Ok(topicManager.GetParentTopics(id));
        }

        #endregion

        #region Get Users

        // GET api/topics/:id/students
        [HttpGet("{id}/Students")]
        public IActionResult GetTopicStudents(int id)
        {
            return GetTopicUsers(id, Role.Student);
        }

        // GET api/topics/:id/supervisors
        [HttpGet("{id}/Supervisors")]
        public IActionResult GetTopicSupervisors(int id)
        {
            return GetTopicUsers(id, Role.Supervisor);
        }
        
        // GET api/topics/:id/reviewers
        [HttpGet("{id}/Reviewers")]
        public IActionResult GetTopicReviewers(int id)
        {
             return GetTopicUsers(id, Role.Reviewer);
        }

        private IActionResult GetTopicUsers(int id, string role)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), id))
                return Unauthorized();

            return Ok(topicManager.GetAssociatedUsersByRole(id, role));
        }

        #endregion

        #region Edit Topics


        // POST api/topics
        [HttpPost]
        public IActionResult Post(TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
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

        // PUT api/topics/:id
        [HttpPut("{id}")]
        [Authorize(Roles = Role.Supervisor)]
        public IActionResult Put(int id, TopicFormModel model)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), id))
                return Unauthorized();

            if (ModelState.IsValid)
            { // TODO createUser is Supervisor!
                if (topicManager.UpdateTopic(User.Identity.GetUserId(), id, model))
                    return Ok(42);
            }
            return BadRequest(ModelState);
        }

        // PUT api/topic/:id/status
        [HttpPut("{id}/Status")]
        public IActionResult ChangeStatus(int id, string status)
        {
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(),id))
                return Unauthorized();

            if (!Status.IsStatusValid(status))
                ModelState.AddModelError("status", "Invalid Status");
            else if (topicManager.ChangeTopicStatus(User.Identity.GetUserId(), id, status))
                return Ok();

            return BadRequest(ModelState);
        }


        // DELETE api/topics/:id
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!topicPermissions.IsAllowedToAdminister(User.Identity.GetUserId()))
                return Unauthorized();
            if (topicManager.DeleteTopic(id))
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
        public IActionResult GetPicture(int topicId, int attachmentId)
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
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
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
            if (!topicPermissions.IsAllowedToEdit(User.Identity.GetUserId(), topicId))
                return Unauthorized();

            if (attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return BadRequest();
        }

        #endregion

    }
}