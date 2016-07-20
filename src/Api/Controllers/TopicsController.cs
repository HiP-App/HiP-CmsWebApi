using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: api/topics
        [HttpGet]
        public async Task<IActionResult> Get(string query, DateTime deadline, DateTime creationDate, int page = 1)
        {
            var topics = await topicManager.GetAllTopicsAsync(query, page, Constants.PageSize);
            int count = await topicManager.GetTopicsCountAsync();
            
            return Ok(new PagedResult<Topic>(topics, page, count));
        }

        // GET api/topics/:id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var topics = await topicManager.GetTopicByIdAsync(id);
            
            if (topics != null)
            {
                return Ok(topics);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/topics
        [HttpPost]
        public IActionResult Post(TopicFormModel topic)
        {
            var _topic = topicManager.AddTopicAsync(topic);
            return new ObjectResult(_topic);
        }

        // PUT api/topics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TopicFormModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = await topicManager.UpdateTopicAsync(id, model);
                return new ObjectResult(success); ;
            }

            return BadRequest(ModelState);
        }

        // DELETE api/topics/5
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            bool deletion = topicManager.DeleteTopicAsync(id);
            return deletion;
        }

    }
}
