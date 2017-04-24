using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using PaderbornUniversity.SILab.Hip.CmsApi.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class AttachmentsManager : BaseManager
    {
        public AttachmentsManager(CmsDbContext dbContext) : base(dbContext) { }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public TopicAttachment GetAttachmentById(int attachmentId)
        {
            return DbContext.TopicAttachments.Single(ta => (ta.Id == attachmentId));
        }

        public IEnumerable<TopicAttachmentResult> GetAttachments(int topicId)
        {
            return DbContext.TopicAttachments.Where(ta => (ta.TopicId == topicId)).Include(ta => ta.User).Include(ta => ta.Metadata).ToList().Select(at => new TopicAttachmentResult(at));
        }

        public EntityResult CreateAttachment(int topicId, string identity, AttachmentFormModel model)
        {
            if (!DbContext.Topics.Include(t => t.TopicUsers).Any(t => t.Id == topicId))
                return EntityResult.Error("Unknown Topic");

            try
            {
                var user = GetUserByIdentity(identity);
                var attatchment = new TopicAttachment(model)
                {
                    UserId = user.Id,
                    TopicId = topicId,
                    Type = "TODO"
                };

                DbContext.TopicAttachments.Add(attatchment);
                DbContext.SaveChanges();

                return EntityResult.Successfull(attatchment.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }
        public EntityResult PutAttachment(int attachmentId, string identity, IFormFile file)
        {
            TopicAttachment attachment;
            try
            {
                attachment = DbContext.TopicAttachments.Include(t => t.Topic).ThenInclude(t => t.TopicUsers).Single(t => t.Id == attachmentId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Attachment");
            }

            var topicFolder = Path.Combine(Constants.AttachmentPath, attachment.TopicId.ToString());
            if (!Directory.Exists(topicFolder))
                Directory.CreateDirectory(topicFolder);

            var fileName = (Path.Combine(topicFolder, file.FileName));
            using (var outputStream = new FileStream(fileName, FileMode.Create))
            {
                file.CopyTo(outputStream);
            }
            try
            {
                attachment.Path = file.FileName;
                attachment.Type = "TODO";

                DbContext.Update(attachment);
                DbContext.SaveChanges();

                new NotificationProcessor(DbContext, attachment.Topic, identity).OnAttachmetAdded(attachment.Title);

                return EntityResult.Successfull(attachment.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }


        public bool DeleteAttachment(int topicId, int attachmentId)
        {
            try
            {
                var attachment = GetAttachmentById(attachmentId);
                if (!string.IsNullOrEmpty(attachment.Path))
                {
                    var fileName = Path.Combine(Constants.AttachmentFolder, topicId.ToString(), attachment.Path);
                    DeleteFile(fileName);
                }
                DbContext.Remove(attachment);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        #region Metadata

        internal void UpdateMetadata(int topicId, int attachmentId, string identity, Metadata metadata)
        {
            // Delete current
            try
            {
                var existing = DbContext.TopicAttachmentMetadata.Single(tam => tam.TopicAttachmentId == attachmentId);
                DbContext.TopicAttachmentMetadata.Remove(existing);
                DbContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                // Does not exists.
            }
            // Add new
            var newMetadata = new TopicAttachmentMetadata(attachmentId, metadata);
            var attachment = DbContext.TopicAttachments.Single(t => t.Id == attachmentId);
            attachment.Title = metadata.Title;

            DbContext.Add(newMetadata);
            DbContext.SaveChanges();
        }
        #endregion
    }
}
