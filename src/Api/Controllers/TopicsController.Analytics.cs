using System;
using Microsoft.AspNetCore.Mvc;
using Api.Utility;
using Api.Managers;
using Api.Models.AnnotationAnalytics;

namespace Api.Controllers
{
    public partial class TopicsController
    {
        private ContentAnalyticsManager _analyticsManager;

        private void TopicsAnalyticsController()
        {
            _analyticsManager = new ContentAnalyticsManager(DbContext);
        }

        /// <summary>
        /// gets the AnnotationTag Frequency Analytics of {topicId}
        /// </summary>
        /// <param name="topicId">the Id of the Topic {topicId}</param>
        /// <response code="200">The Analytics of</response>
        /// <response code="404">Resource not found</response>
        /// <response code="403">User not allowed to get Analytics</response>
        /// <response code="401">User is denied</response>
        [HttpGet("{topicId}/Analytics/TagFrequency")]
        [ProducesResponseType(typeof(TagFrequencyAnalyticsResult), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 403)]
        public IActionResult GetTagFrequencyAnalytics([FromRoute]int topicId)
        {
            if (!_topicPermissions.IsAssociatedTo(User.Identity.GetUserIdentity(), topicId))
                return Forbidden();

            try
            {
                return Ok(_analyticsManager.GetFrequencyAnalytic(topicId));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}