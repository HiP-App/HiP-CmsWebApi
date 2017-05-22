using Xunit;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using System.Linq;
namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerTest
    {
        private ControllerTester<TopicsController> _tester;
        public TopicFormModel TopicFormModel { get; set; }
        public TopicStatus TopicStatus { get; set; }
        public TopicsControllerTest()
        {
            _tester = new ControllerTester<TopicsController>();
            TopicFormModel = new TopicFormModel
            {
                Title = "Schloss Neuhaus",
                Status = "InReview",
                Description = "Castle"
            };
            TopicStatus = new TopicStatus
            {
                Status = "InProgress"
            };
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
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
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
        /// Passes if topic is added successfully
        /// </summary>
        [Fact]
        public void PostTopicTest()
        {
            _tester.TestController()
                .Calling(c => c.Post(TopicFormModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic => topic.Any(actual => actual.Title == TopicFormModel.Title)));
            // As we have problem with DI, when EmailService is invoked it returns BadRequest. Hence only a part of the positive test case is tested.
        }
        /// <summary>
        /// returns BadRequest when model is incorrect
        /// </summary>
        [Fact]
        public void PostTopicTest400()
        {
            var topicFormModel = new TopicFormModel();
            _tester.TestController()
                .Calling(c => c.Post(topicFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        /// <summary>
        /// returns BadRequest when topic status is incorrect
        /// </summary>
        [Fact]
        public void PostTopicTest400InvalidTopicStatus()
        {
            TopicFormModel.Status = "Wrong";
            _tester.TestController()
                .Calling(c => c.Post(TopicFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        /// <summary>
        /// returns status code 403 when student tries to add a topic
        /// </summary>
        [Fact]
        public void PostTopicTest403()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.Post(TopicFormModel))
                .ShouldReturn()
                .StatusCode(403);
        }
        /// <summary>
        /// Passes if topic is added successfully
        /// </summary>
        //[Fact]
        public void PutTopicTest()
        {
            //The test returns Transactions are not supported by the in-memory store.
            //See http://go.microsoft.com/fwlink/?LinkId=800142. Positive test case not possible.

            _tester.TestControllerWithMockData()
                .Calling(c => c.Put(_tester.TopicOne.Id, TopicFormModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic => topic.Any(actual => actual.Title == TopicFormModel.Title)));
            // As we have problem with DI, when EmailService is invoked it returns BadRequest. Hence only a part of the positive test case is tested.
        }
        /// <summary>
        /// returns BadRequest when model is incorrect
        /// </summary>
        [Fact]
        public void PutTopicTest400()
        {
            var topicFormModel = new TopicFormModel();
            _tester.TestControllerWithMockData()
                .Calling(c => c.Put(_tester.TopicOne.Id, topicFormModel))
                .ShouldReturn()
                .BadRequest();
        }
        /// <summary>
        /// returns status code 403 when student tries to update a topic
        /// </summary>
        [Fact]
        public void PutTopicTest403()
        {
            _tester.TestController(_tester.Student.Email)
                .Calling(c => c.Post(TopicFormModel))
                .ShouldReturn()
                .StatusCode(403);
        }
        /// <summary>
        /// Passes if topic staus is changed successfully
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic => topic.Any(actual => actual.Status == TopicStatus.Status)));
            // As we have problem with DI, when EmailService is invoked it returns NotFound. Hence only a part of the positive test case is tested.
        }
        /// <summary>
        /// returns 404 in case of exception occured
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTest404Forexception()
        {
            _tester.TestControllerWithMockData(_tester.Supervisor.Email)
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldReturn()
                .NotFound();
        }
        /// <summary>
        /// returns 403 when a different user tries to change topic status
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldReturn()
                .StatusCode(403);
        }
        /// <summary>
        /// returns 404 when topic is not found
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTest404()
        {
            _tester.TestController()
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldReturn()
                .NotFound();
        }
        /// <summary>
        /// returns BadRequest when model is incorrect
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTest403ForInvalidStatus()
        {
            TopicStatus.Status = "Wrong";
            _tester.TestControllerWithMockData()
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldReturn()
                .BadRequest();
        }
        /// <summary>
        /// returns status code 409 when topic status is done and also reviewed
        /// </summary>
        [Fact]
        public void ChangeTopicStatusTestForConflict()
        {
            TopicStatus.Status = "Done";
            var reviewer = new User
            {
                Id = 4,
                Email = "reviewer@hipapp.de",
                Role = "Reviewer"
            };
            var reviewerUser = new TopicUser
            {
                TopicId = _tester.TopicOne.Id,
                UserId = reviewer.Id,
                Role = reviewer.Role
            };
            var topicReview = new TopicReview
            {
                TopicId = _tester.TopicOne.Id,
                ReviewerId = reviewer.Id
            };
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.Add(reviewer))
                    .WithSet<TopicUser>(db => db.Add(reviewerUser))
                    .WithSet<TopicReview>(db => db.Add(topicReview)))
                .Calling(c => c.ChangeStatus(_tester.TopicOne.Id, TopicStatus))
                .ShouldReturn()
                .StatusCode(409);
        }
        /// <summary>
        /// Returns ok if topic is deleted
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.Delete(_tester.TopicOne.Id))
                .ShouldHave()
                .DbContext(db => db.WithSet<Topic>(topic => !topic.Any(actual => actual == _tester.TopicOne)));
            // As we have problem with DI, when EmailService is invoked it returns NotFound. Hence only a part of the positive test case is tested.
        }
        /// <summary>
        /// returns 404 when topic is not found
        /// </summary>
        [Fact]
        public void DeleteTest404()
        {
            _tester.TestController()
                .Calling(c => c.Delete(_tester.TopicOne.Id))
                .ShouldReturn()
                .NotFound();
        }
        /// <summary>
        /// returns 403 when a student tries to delete topic
        /// </summary>
        [Fact]
        public void DeleteTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Email)
                .Calling(c => c.Delete(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }
        #endregion
    }
}