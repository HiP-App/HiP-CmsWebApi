using Xunit;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Tests.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerTest : IClassFixture<TestEmailSender>
    {
        public TestEmailSender TestEmailSender { get; }
        private ControllerTester<TopicsController> _tester;

        public TopicsControllerTest(TestEmailSender testEmailSender)
        {
            TestEmailSender = testEmailSender;
            _tester = new ControllerTester<TopicsController>();
        }

        #region GET topics
        
        /// <summary>
        /// Returns ok if all topics are retrieved
        /// </summary>
        [Fact]
        public void GetTopicsTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.Get(null, null, null, false, 0, 0))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<TopicResult>>()
                .Passing(topic => topic.Metadata.ItemsCount == 2);
        }
        
        /// <summary>
        /// Returns ok if all topics are retrieved matching query, status and deadline
        /// </summary>
        [Fact]
        public void GetTopicsWithQueryTest()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.Get("Paderborner", "InReview", new DateTime(2017, 5, 04), false, 0, 0))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<TopicResult>>()
                .Passing(topic => topic.Metadata.ItemsCount == 1);
        }
        
        #endregion
        
        #region GET topics of user
        
        /// <summary>
        /// Returns ok if all topics are retrieved for the user
        /// </summary>
        [Fact]
        public void GetTopicsForUserTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicsForUser(_tester.Student.Email, 0, 0, null))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<TopicResult>>()
                .Passing(topic => topic.Metadata.ItemsCount == 1);
        }
        
        /// <summary>
        /// Returns ok if all topics are retrieved for the user matching query
        /// </summary>
        [Fact]
        public void GetTopicsForUserWithQueryTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTopicsForUser(_tester.Student.Email, 0, 0, "Paderborner"))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<PagedResult<TopicResult>>()
                .Passing(topic => topic.Metadata.ItemsCount == 1);
        }
        
        #endregion
        
        #region GET topic based on topicId
        
        /// <summary>
        /// Returns ok if the topic is retrieved given topicId
        /// </summary>
        [Fact]
        public void GetTopicTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.Get(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .Equals(_tester.TopicOne);
        }
        
        /// <summary>
        /// Returns ok if the topic is retrieved given topicId
        /// </summary>
        [Fact]
        public void GetTopicNotFoundTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.Get(3))
                .ShouldReturn()
                .NotFound();
        }
        
        #endregion
        
        #region Edit Topics
        
        /// <summary>
        /// Returns ok if new topic is created
        /// </summary>
        [Fact]
        public void PostTopicTest() //https://xunit.github.io/docs/shared-context.html#class-fixture
        {
            var topicFormModel = new TopicFormModel
            {
                Title = "Schloss Neuhaus",
                Status = "InReview",
                Description = "Castle"
            };
            _tester.TestController()
                .Calling(c => c.Post(topicFormModel))
                .ShouldReturn()
                .Ok();
        }
        
        #endregion
    }
}