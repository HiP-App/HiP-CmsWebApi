using System;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.Extensions.Logging;
using BLL.Managers;
using Microsoft.AspNetCore.Mvc;
using BOL.Models;
using Api.Utility;

namespace Api.Controllers
{
    public class TopicsController : ApiController
    {
        private TopicManager topicManager;

        public TopicsController(ApplicationDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            topicManager = new TopicManager(dbContext);
        }

        // GET api/topics
        [HttpGet]
        public async Task<IActionResult> Get(string query, string status, DateTime deadline, int page = 1)
        {
            var topics = await topicManager.GetAllTopicsAsync(query, status, deadline, page, Constants.PageSize);
            int count = await topicManager.GetTopicsCountAsync();
            
            return Ok(new PagedResult<Topic>(topics, page, count));
        }

        // GET api/topics/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var topics = await topicManager.GetTopicByIdAsync(id);
            
            if (topics != null)
                return Ok(topics);
            else
                return NotFound();
        }

        // POST api/topics
        [HttpPost]
        public async Task<IActionResult> Post(TopicFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (!Status.IsStatusValid(model.Status))
                {
                    ModelState.AddModelError("Status", "Invalid Topic Status");
                }
                else
                {
                    var result = await topicManager.AddTopicAsync(User.Identity.GetUserId(), model);

                    if(result.Success)
                        return new ObjectResult(result);
                }
            }

            return BadRequest(ModelState);
        }

        // PUT api/topics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TopicFormModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await topicManager.UpdateTopicAsync(id, model);
                
                if(success)
                    return Ok();
            }

            return BadRequest(ModelState);
        }

        // DELETE api/topics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await topicManager.DeleteTopicAsync(id);
            
            if (success)
                return Ok();
            else
                return BadRequest();
        }

    }
}