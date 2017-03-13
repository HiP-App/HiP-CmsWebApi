using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using Api.Models;
using Api.Models.User;
using System.Collections.Generic;

namespace Api.Controllers
{
    public partial class TopicsController
    {

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
        public IActionResult GetTopicStudents([FromRoute]int topicId)
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
        public IActionResult GetTopicSupervisors([FromRoute]int topicId)
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
        public IActionResult GetTopicReviewers([FromRoute]int topicId)
        {
            return GetTopicUsers(topicId, Role.Reviewer);
        }

        private IActionResult GetTopicUsers(int topicId, string role)
        {
            return Ok(_topicManager.GetAssociatedUsersByRole(topicId, role));
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
        public IActionResult PutTopicStudents([FromRoute]int topicId, [FromBody]UsersFormModel users)
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
        public IActionResult PutTopicSupervisors([FromRoute]int topicId, [FromBody]UsersFormModel users)
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
        public IActionResult PutTopicReviewers([FromRoute]int topicId, [FromBody]UsersFormModel users)
        {
            return PutTopicUsers(topicId, Role.Reviewer, users);
        }

        private IActionResult PutTopicUsers(int topicId, string role, UsersFormModel users)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserId(), topicId))
                return Forbidden();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (_topicManager.ChangeAssociatedUsersByRole(User.Identity.GetUserId(), topicId, role, users.Users))
                return Ok();
            return BadRequest();
        }

        #endregion

    }
}