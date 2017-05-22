using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerDocumentsTest
    {
        private ControllerTester<TopicsController> _tester;
        public HtmlContentModel HtmlContentModel { get; set; }

        public TopicsControllerDocumentsTest()
        {
            _tester = new ControllerTester<TopicsController>();
            HtmlContentModel = new HtmlContentModel
            {
                HtmlContent = "Hello"
            };
        }

        #region GET documents

        /// <summary>
        /// Returns ok if the document associated with the topic is retrieved
        /// </summary>
        [Fact]
        public void GetDocumentTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetDocument(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<DocumentResult>()
                .Passing(actual => actual.Content == _tester.FirstDocument.Content);
        }

        /// <summary>
        /// Returns 404 if the document is not found
        /// </summary>
        [Fact]
        public void GetDocumentTest404()
        {
            _tester.TestController()
                .Calling(c => c.GetDocument(_tester.TopicOne.Id))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Returns 403 if some other user tries to get the document
        /// </summary>
        [Fact]
        public void GetDocumentTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.GetDocument(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion

        #region post documents

        /// <summary>
        /// Returns ok if the document associated with the topic is retrieved
        /// </summary>
        [Fact]
        public void PostDocumentTestWithContent()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PostDocument(_tester.TopicOne.Id, HtmlContentModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<Document>(actual => actual.Any(t => t.Content == _tester.FirstDocument.Content)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Returns ok if the document associated with the topic is retrieved
        /// </summary>
        [Fact]
        public void PostDocumentTest()
        {
            _tester.FirstDocument.Content = string.Empty;

            _tester.TestControllerWithMockData()
                .Calling(c => c.PostDocument(_tester.TopicOne.Id, HtmlContentModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<Document>(actual => actual.Any(t => t.Content == HtmlContentModel.HtmlContent)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Returns 403 if some other user tries to get the document
        /// </summary>
        [Fact]
        public void PostDocumentTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.PostDocument(_tester.TopicOne.Id, HtmlContentModel))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns 403 if some other user tries to get the document
        /// </summary>
        [Fact]
        public void PostDocumentTestBadRequest()
        {
            _tester.TestController()
                .Calling(c => c.PostDocument(_tester.TopicOne.Id, HtmlContentModel))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region Delete Documents

        /// <summary>
        /// Returns ok if the document associated with the topic is deleted
        /// </summary>
        [Fact]
        public void DeleteDocumentTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.DeleteDocument(_tester.TopicOne.Id))
                .ShouldHave()
                .DbContext(db => db.WithSet<Document>(actual => !actual.Any(t => t.Content == HtmlContentModel.HtmlContent)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Returns 403 if some other user tries to get the document
        /// </summary>
        [Fact]
        public void DeleteDocumentTestBadRequest()
        {
            _tester.TestController()
                .Calling(c => c.DeleteDocument(_tester.TopicOne.Id))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Returns 403 if some other user tries to get the document
        /// </summary>
        [Fact]
        public void DeleteDocumentTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de")
                .Calling(c => c.DeleteDocument(_tester.TopicOne.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion
    }
}
