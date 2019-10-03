using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Shared;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System.Collections.Generic;
using System.Linq;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerAttachmentsTest
    {
        private ControllerTester<TopicsController> _tester;
        public TopicAttachment TopicAttachment { get; set; }
        public AttachmentFormModel AttachmentFormModel { get; set; }

        public TopicsControllerAttachmentsTest()
        {
            _tester = new ControllerTester<TopicsController>();
            TopicAttachment = new TopicAttachment
            {
                Id = 1,
                Title = "Dom",
                TopicId = _tester.TopicOne.Id,
                Path = "C:\\Source"
            };
            AttachmentFormModel = new AttachmentFormModel
            {
                Title = "Dom"
            };
        }

        #region GET attachments

        /// <summary>
        /// Returns ok if all attachments of the topic are retrieved
        /// </summary>
        [Fact]
        public void GetAttachmentsTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicAttachment>(db => db.Add(TopicAttachment)))
                .Calling(c => c.GetAttachmentsAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<TopicAttachmentResult>>()
                .Passing(actual => actual.Any(t => t.Id == TopicAttachment.Id));
        }

        /// <summary>
        /// Should return 200 and an empty array of topic attachment if none are found
        /// </summary>
        [Fact]
        public void GetAttachmentsTest200_Empty()
        {
            var expected = 0;
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetAttachmentsAsync(_tester.TopicOne.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<IEnumerable<TopicAttachmentResult>>()
                .Passing(actual => actual.Count() == expected);
        }

        /// <summary>
        /// Returns 403 if attachments are retrieved by other user
        /// </summary>
        [Fact]
        public void GetAttachmentsTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id) // Student not attached to topic two
                .Calling(c => c.GetAttachmentsAsync(_tester.TopicTwo.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns ok if the specified attachment of the topic is retrieved
        /// </summary>
        [Fact]
        public void GetAttachmetTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicAttachment>(db => db.Add(TopicAttachment)))
                .Calling(c => c.GetAttachmentAsync(_tester.TopicOne.Id, TopicAttachment.Id))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<StringWrapper>()
                .Passing(actual => actual.Value = "D704A706C781811D7143B2D1C980DCCD");
        }

        /// <summary>
        /// Returns 404 if the specified attachment of the topic is not found
        /// </summary>
        [Fact]
        public void GetAttachmetTest404()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.GetAttachmentAsync(_tester.TopicOne.Id, TopicAttachment.Id))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Returns 403 if attachments are retrieved by other user
        /// </summary>
        [Fact]
        public void GetAttachmetTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id) // Student not attached to topic two
                .Calling(c => c.GetAttachmentAsync(_tester.TopicTwo.Id,TopicAttachment.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion

        #region post attachments
        //Positive test case not possible as we need to attach the file

        /// <summary>
        /// Returns ok if an attachment is created for the topic
        /// </summary>
        [Fact]
        public void PostAttachmentTest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PostAttachmentAsync(_tester.TopicOne.Id, AttachmentFormModel))
                .ShouldHave()
                .DbContext(db => db.WithSet<TopicAttachment>(relations =>
                    relations.Any(actual => actual.Title == AttachmentFormModel.Title)))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Returns statuscode 404 when trying to add an attachment for the topic that does not exist
        /// </summary>
        [Fact]
        public void PostAttachmentTest404()
        {
            _tester.TestController()
                .Calling(c => c.PostAttachmentAsync(_tester.TopicOne.Id, AttachmentFormModel))
                .ShouldReturn()
                .StatusCode(404);
        }

        /// <summary>
        /// Returns 403 if some other user tries to add an attachment
        /// </summary>
        [Fact]
        public void PostAttachmentTestForbidden()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id) // Student not attached to topic two
                .Calling(c => c.PostAttachmentAsync(_tester.TopicTwo.Id, AttachmentFormModel))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns bad request if there is any problem with the model
        /// </summary>
        [Fact]
        public void PostAttachmentTestBadRequest()
        {
            var attachmentFormModel = new AttachmentFormModel();

            _tester.TestControllerWithMockData()
                .Calling(c => c.PostAttachmentAsync(_tester.TopicOne.Id, attachmentFormModel))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region put attachments

        /// <summary>
        /// Returns 403 if some other user tries to update an attachment
        /// </summary>
        [Fact]
        public void PutAttachmentTestForbidden()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id) // Student not attached to topic two
                .Calling(c => c.PutAttachmentAsync(_tester.TopicTwo.Id, TopicAttachment.Id,null))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns bad request if there is no file attached
        /// </summary>
        [Fact]
        public void PutAttachmentTestBadRequest()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutAttachmentAsync(_tester.TopicOne.Id, TopicAttachment.Id, null))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion

        #region Delete attachments

        /// <summary>
        /// Returns 403 if some other user tries to delete an attachment
        /// </summary>
        [Fact]
        public void DeleteAttachmentTestForbidden()
        {
            _tester.TestControllerWithMockData(_tester.Student.Id)
                .Calling(c => c.DeleteAttachmentAsync(_tester.TopicTwo.Id, TopicAttachment.Id))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Returns not found when trying to delete an attachment that does not exist
        /// </summary>
        [Fact]
        public void DeleteAttachmentTest404()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.DeleteAttachmentAsync(_tester.TopicOne.Id, TopicAttachment.Id))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Returns ok if the attachment is deleted
        /// </summary>
        [Fact]
        public void DeleteAttachmentTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicAttachment>(db => db.Add(TopicAttachment)))
                .Calling(c => c.DeleteAttachmentAsync(_tester.TopicOne.Id, TopicAttachment.Id))
                .ShouldHave()
                .DbContext(db => db.WithSet<TopicAttachment>(actual => !(actual.Any(t => t.Id == TopicAttachment.Id))))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        #endregion
    }
}
