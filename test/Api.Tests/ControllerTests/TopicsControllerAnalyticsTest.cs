using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationAnalytics;
using System.Collections.Generic;
using Xunit;
namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerAnalyticsTest
    {
        private ControllerTester<TopicsController> _tester;
        public TopicsControllerAnalyticsTest()
        {
            _tester = new ControllerTester<TopicsController>();
        }
        #region GET Analytics
        /// <summary>
        /// Returns ok if tag frequency analytics are retrieved
        /// </summary>
        [Fact]
        public void GetTagFrequencyAnalyticsTest()
        {
            var expectedTagFrequencyList = new List<TagFrequency>();
            var tagFrequency = new TagFrequency(_tester.Tag1.Id, null, 1);
            expectedTagFrequencyList.Add(tagFrequency);
            var result = new TagFrequencyAnalyticsResult
            {
                TagFrequency = expectedTagFrequencyList
            };

            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTagFrequencyAnalytics(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<TagFrequencyAnalyticsResult>()
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                .Equals(result);
        }
        /// <summary>
        /// Returns 404 if frequency analytics are not found
        /// </summary>
        [Fact]
        public void GetTagFrequencyAnalyticsTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetTagFrequencyAnalytics(_tester.TopicOne.Id))
                .ShouldReturn()
                .NotFound();
        }
        /// <summary>
        /// Returns 404 if frequency analytics are not found
        /// </summary>
        [Fact]
        public void GetTagFrequencyAnalyticsTest404ForDifferentTopic()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetTagFrequencyAnalytics(_tester.TopicTwo.Id))
                .ShouldReturn()
                .NotFound();
        }
        /// <summary>
        /// Returns 403 if a student tries to get the analytics
        /// </summary>
        [Fact]
        public void GetTagFrequencyAnalyticsTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.GetTagFrequencyAnalytics(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }
        #endregion
    }
}