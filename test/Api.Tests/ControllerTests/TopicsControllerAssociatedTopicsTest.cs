using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using Xunit;
using MyTested.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerAssociatedTopicsTest
    {
        private ControllerTester<TopicsController> _tester;
        public AssociatedTopic AssociatedTopic { get; set; }

        public TopicsControllerAssociatedTopicsTest()
        {
            _tester = new ControllerTester<TopicsController>();
            AssociatedTopic = new AssociatedTopic
            {
                ParentTopicId = _tester.TopicOne.Id,
                ChildTopicId = _tester.TopicTwo.Id
            };
        }

        #region GET topics

        /// <summary>
        /// Returns ok if subtopics of the topic {topicId} are retrieved
        /// </summary>
        [Fact]
        public void GetTopicsTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AssociatedTopic>(db => db.Add(AssociatedTopic)))
                .Calling(c => c.GetSubTopics(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<Topic>>()
                .Passing(actual => actual.Single( t => t.Id == _tester.TopicTwo.Id));
        }

        /// <summary>
        /// Returns ok if parent topics of the topic {topicId} are retrieved
        /// </summary>
        [Fact]
        public void GetParentTopicsTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AssociatedTopic>(db => db.Add(AssociatedTopic)))
                .Calling(c => c.GetParentTopics(_tester.TopicTwo.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<Topic>>()
                .Passing(actual => actual.Single(t => t.Id == _tester.TopicOne.Id));
        }

        #endregion

        #region Put topics

        /// <summary>
        /// Returns ok if associates the topic {topicId} with parent topic {parentId}
        /// </summary>
        [Fact]
        public void PutParentTopicsTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutParentTopicsAsync(_tester.TopicTwo.Id, _tester.TopicOne.Id))
                .ShouldReturn()
                .Ok(); //The method doesn't return any model in return other than just an Ok() and so assetion is not possible
        }

        /// <summary>
        /// Returns 403 if student tries to associates the topic {topicId} with parent topic {parentId}
        /// </summary>
        [Fact]
        public void PutParentTopicsTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutParentTopicsAsync(_tester.TopicTwo.Id, _tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403); 
        }

        /// <summary>
        /// Returns Bad request if associates the exisitng relation
        /// </summary>
        [Fact]
        public void PutParentTopicsTestBadRequest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AssociatedTopic>(db => db.Add(AssociatedTopic)))
                .Calling(c => c.PutParentTopicsAsync(_tester.TopicTwo.Id, _tester.TopicOne.Id))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Returns ok if associates the topic {topicId} with child topic {childId}
        /// </summary>
        [Fact]
        public void PutSubTopicsTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutSubTopicsAsync(_tester.TopicOne.Id, _tester.TopicTwo.Id))
                .ShouldReturn()
                .Ok(); //The method doesn't return any model in return other than just an Ok() and so assetion is not possible
        }

        /// <summary>
        /// Returns 403 if student tries to associates the topic {topicId} with child topic {childId}
        /// </summary>
        [Fact]
        public void PutSubTopicsTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.PutSubTopicsAsync(_tester.TopicTwo.Id, _tester.TopicTwo.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns Bad request if associates the exisitng relation
        /// </summary>
        [Fact]
        public void PutSubTopicsTestBadRequest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<AssociatedTopic>(db => db.Add(AssociatedTopic)))
                .Calling(c => c.PutSubTopicsAsync(_tester.TopicOne.Id, _tester.TopicTwo.Id))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion
    }
}
