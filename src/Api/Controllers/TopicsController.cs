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

namespace Api.Controllers
{
    public class TopicsController : ApiController
    {
        private TopicManager topicManager;
        private AttachmentsManager attachmentsManager;

        public TopicsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicManager = new TopicManager(dbContext);
            attachmentsManager = new AttachmentsManager(dbContext);
        }

        #region topics


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
            var topics = topicManager.GetTopicById(id);
            if (topics != null)
                return Ok(topics);

            return NotFound();
        }


        // GET api/topics/:id/students
        [HttpGet("{id}/Students")]
        public IActionResult GetTopicStudents(int id)
        {
            return Ok(topicManager.GetAssociatedUsersByRole(id, Role.Student));
        }


        // GET api/topics/:id/supervisors
        [HttpGet("{id}/Supervisors")]
        public IActionResult GetTopicSupervisors(int id)
        {
            return Ok(topicManager.GetAssociatedUsersByRole(id, Role.Supervisor));
        }


        // GET api/topics/:id/reviewers
        [HttpGet("{id}/Reviewers")]
        public IActionResult GetTopicReviewers(int id)
        {
            return Ok(topicManager.GetAssociatedUsersByRole(id, Role.Reviewer));
        }


        // GET api/topics/:id/subtopics
        [HttpGet("{id}/SubTopics")]
        public IActionResult GetSubTopics(int id)
        {
            return Ok(topicManager.GetSubTopics(id));
        }


        // GET api/topics/:id/parenttopics
        [HttpGet("{id}/ParentTopics")]
        public IActionResult GetParentTopics(int id)
        {
            return Ok(topicManager.GetParentTopics(id));
        }


        // POST api/topics
        [HttpPost]
        [Authorize(Roles = Role.Supervisor)]
        public IActionResult Post(TopicFormModel model)
        {
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
            if (ModelState.IsValid)
            { // TODO createUser is Supervisor!
                if (topicManager.UpdateTopic(User.Identity.GetUserId(), id, model))
                    return Ok();
            }
            return BadRequest(ModelState);
        }


        // DELETE api/topics/:id
        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Supervisor)]
        public IActionResult Delete(int id)
        {
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
            var attachments = attachmentsManager.GetAttachment(topicId);
            if (attachments != null)
                return Ok(attachments);
            return NotFound();
        }


        [HttpGet("{topicId}/Attachments/{attachmentId}")]
        public IActionResult GetPicture(int topicId, int attachmentId)
        {
            var attachment = attachmentsManager.GetAttachmentById(topicId, attachmentId);
            if (attachment != null)
            {
                string fileName = Path.Combine(Constants.AttatchmentFolder, attachment.Path);
                string contentType = MimeKit.MimeTypes.GetMimeType(fileName);
                return base.File(fileName, contentType);
            }
            return BadRequest();
        }

        // POST api/topics/:id/attachments
        [HttpPost("{topicId}/Attachments")]
        public IActionResult PostAttachment(int topicId, AttatchmentFormModel model, IFormFile file)
        {
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
            if (attachmentsManager.DeleteAttachment(topicId, attachmentId))
                return Ok();
            return BadRequest();
        }

        #endregion
    }
}