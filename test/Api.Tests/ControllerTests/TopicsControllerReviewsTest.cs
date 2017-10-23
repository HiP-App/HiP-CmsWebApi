using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerReviewsTest
    {
        private ControllerTester<TopicsController> _tester;
        public TopicReviewStatus TopicReviewStatus { get; set; }
        public TopicReview TopicReview { get; set; }

        public TopicsControllerReviewsTest()
        {
            _tester = new ControllerTester<TopicsController>();
            TopicReviewStatus = new TopicReviewStatus { Status = "Reviewed" };
            TopicReview = new TopicReview
            {
                TopicId = _tester.TopicOne.Id,
                ReviewerId = _tester.Reviewer.Id,
                Status = "InReview"
            };
        }
        #region Get status
        /// <summary>
        /// Returns ok if the status of the topic review {topicId} is retrieved
        /// </summary>
        [Fact]
        public void GetReviewStatusTest200()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicReview>(db => db.Add(TopicReview)))
                .Calling(c => c.GetReviewStatusAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<TopicReviewResult>>()
                .Passing(actual => actual.Any(u => u.Reviewer == _tester.Reviewer.Id && u.Status.Status == TopicReview.Status));
        }
        /// <summary>
        /// Returns 403 if the user is not associated with the topic to get the status of the topic review
        /// </summary>
        [Fact]
        public void GetReviewStatusTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.GetReviewStatusAsync(_tester.TopicTwo.Id))
                .ShouldReturn()
                .StatusCode(403);
        }
        /// <summary>
        /// Returns 404 if the topic does not exist for getting status of the review
        /// </summary>
        [Fact]
        public void GetReviewStatusTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetReviewStatusAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .NotFound();
        }
        #endregion
        #region Put status
        /// <summary>
        /// Returns ok if the status of the topic review {topicId} is updated
        /// </summary>
        [Fact]
        public void PutReviewStatusTest200()
        {
            _tester.TestControllerWithMockData(_tester.Reviewer.Id)
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicReview>(db => db.Add(TopicReview)))
                .Calling(c => c.PutReviewStatusAsync(_tester.TopicOne.Id, TopicReviewStatus))
                .ShouldHave()
                .DbContext(db => db.WithSet<TopicReview>(s =>
                    s.Single(r => r.Status == TopicReviewStatus.Status).ReviewerId == _tester.Reviewer.Id))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Returns 403 if the user trying to update is not a reviewer
        /// </summary>
        [Fact]
        public void PutReviewStatusTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutReviewStatusAsync(_tester.TopicOne.Id, TopicReviewStatus))
                .ShouldReturn()
                .StatusCode(403);
        }
        /// <summary>
        /// Returns 403 if the topic does not exist for getting status of the review
        /// </summary>
        [Fact]
        public void PutReviewStatusTest404()
        {
            _tester.TestControllerWithMockData(_tester.Reviewer.Email)
                .Calling(c => c.PutReviewStatusAsync(3, TopicReviewStatus))
                .ShouldReturn()
                .StatusCode(403);
        }
        #endregion
    }
}