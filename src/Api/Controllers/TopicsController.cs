using System;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.Extensions.Logging;
using BLL.Managers;
using Microsoft.AspNetCore.Mvc;
using BOL.Models;
using Api.Utility;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BOL.Data;

namespace Api.Controllers
{
    public class TopicsController : ApiController
    {
        private TopicManager topicManager;

        public TopicsController(CmsDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicManager = new TopicManager(dbContext);
        }


        // GET api/topics
        [HttpGet]
        public IActionResult Get(string query, string status, DateTime? deadline, bool onlyParents = false, int page = 1)
        {
            var topics = topicManager.GetAllTopics(query, status, deadline, onlyParents);
            int count =  topics.Count();
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
            else
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
            return Ok( topicManager.GetParentTopics(id));
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
                    if(result.Success)
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
                bool success = topicManager.UpdateTopic(User.Identity.GetUserId(), id, model);
                if(success)
                    return Ok();
            }
            return BadRequest(ModelState);
        }


        // DELETE api/topics/:id
        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Supervisor)]
        public IActionResult Delete(int id)
        {
            bool success = topicManager.DeleteTopic(id);
            
            if (success)
                return Ok();
            else
                return BadRequest();
        }

        // PUT api/topic/:id/status
        [HttpPut("{id}/Status")]
        public IActionResult ChangeStatus(int id, string status)
        {
            bool success = topicManager.ChangeTopicStatus(User.Identity.GetUserId(), id, status);

            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}