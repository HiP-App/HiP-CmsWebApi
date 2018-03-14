using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class TopicsControllerMetadataTest
    {
        private ControllerTester<TopicsController> _tester;
        public TopicAttachment TopicAttachment { get; set; }
        public Metadata Metadata { get; set; }

        public TopicsControllerMetadataTest()
        {
            _tester = new ControllerTester<TopicsController>();
            TopicAttachment = new TopicAttachment
            {
                Id = 1,
                Title = "Dom",
                TopicId = _tester.TopicOne.Id,
                Path = "C:\\Source"
            };
            Metadata = new Metadata
            {
                Title = "Dom features",
                Type = "Info"
            };
        }
        
        #region Post Metadata
        
        /// <summary>
        /// Returns ok if a metadata is added to the attachment {topicId}
        /// </summary>
        [Fact]
        public void PostMetaDataTest()
        {
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicAttachment>(db => db.Add(TopicAttachment)))
                .Calling(c => c.PostMetaData(_tester.TopicOne.Id, TopicAttachment.Id, Metadata))
                .ShouldHave()
                .DbContext(db => db.WithSet<TopicAttachmentMetadata>(metadata =>
                    metadata.Any(actual => actual.Type == Metadata.Type)
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }
        
        /// <summary>
        /// Returns ok if a metadata is added to the attachment {topicId}
        /// </summary>
        [Fact]
        public void PostMetaDataTestBadRequest()
        {
            var metadata = new Metadata();
            _tester.TestControllerWithMockData()
                .Calling(c => c.PostMetaData(_tester.TopicOne.Id, TopicAttachment.Id, metadata))
                .ShouldReturn()
                .BadRequest();
        }
        
        /// <summary>
        /// Returns 403 if the user is not allowed to post data for the topic
        /// </summary>
        [Fact]
        public void PostMetaDataTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.UId)
                .Calling(c => c.PostMetaData(_tester.TopicTwo.Id, TopicAttachment.Id, Metadata))
                .ShouldReturn()
                .StatusCode(403);
        }
        
        #endregion
        
        #region Put MetaData
        
        /// <summary>
        /// Returns ok if a metadata is added to the attachment {topicId}
        /// </summary>
        [Fact]
        public void PutMetaDataTest()
        {
            var topicAttachmentMetadata = new TopicAttachmentMetadata(TopicAttachment.Id, Metadata);
            var metadata = new Metadata
            {
                Title = "Checking for updated one",
                Type = "updated"
            };
            _tester.TestControllerWithMockData()
                .WithDbContext(dbContext => dbContext
                    .WithSet<TopicAttachment>(db => db.Add(TopicAttachment))
                    .WithSet<TopicAttachmentMetadata>(db => db.Add(topicAttachmentMetadata)))
                .Calling(c => c.PutMetaData(_tester.TopicOne.Id, TopicAttachment.Id, metadata))
                .ShouldHave()
                .DbContext(db => db.WithSet<TopicAttachmentMetadata>(md =>
                    md.Any(actual => actual.Type == metadata.Type)
                ))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }
        
        /// <summary>
        /// Returns ok if a metadata is added to the attachment {topicId}
        /// </summary>
        [Fact]
        public void PutMetaDataTestBadRequest()
        {
            var metadata = new Metadata();
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutMetaData(_tester.TopicOne.Id, TopicAttachment.Id, metadata))
                .ShouldReturn()
                .BadRequest();
        }
        
        /// <summary>
        /// Returns ok if a metadata is added to the attachment {topicId}
        /// </summary>
        [Fact]
        public void PutMetaDataTest403()
        {
            _tester.TestControllerWithMockData(_tester.Student.UId)
                .Calling(c => c.PutMetaData(_tester.TopicTwo.Id, TopicAttachment.Id, Metadata))
                .ShouldReturn()
                .StatusCode(403);
        }

        #endregion
    }
}