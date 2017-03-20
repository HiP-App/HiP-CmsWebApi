using Api.Data;
using Api.Models;
using Api.Models.Entity;
using Api.Models.Topic;
using Api.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Api.Managers
{
    public class AttachmentsManager : BaseManager
    {
        public AttachmentsManager(CmsDbContext dbContext) : base(dbContext) { }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public TopicAttatchment GetAttachmentById(int attachmentId)
        {
            return DbContext.TopicAttatchments.Single(ta => (ta.Id == attachmentId));
        }

        public IEnumerable<TopicAttachmentResult> GetAttachments(int topicId)
        {
            return DbContext.TopicAttatchments.Where(ta => (ta.TopicId == topicId)).Include(ta => ta.User).Include(ta => ta.Legal).ToList().Select(at => new TopicAttachmentResult(at));
        }

        public EntityResult CreateAttachment(int topicId, string identity, AttatchmentFormModel model)
        {
            if (!DbContext.Topics.Include(t => t.TopicUsers).Any(t => t.Id == topicId))
                return EntityResult.Error("Unknown Topic");

            try
            {
                var user = GetUserByIdentity(identity);
                var attatchment = new TopicAttatchment(model)
                {
                    UserId = user.Id,
                    TopicId = topicId,
                    Type = "TODO"
                };

                DbContext.TopicAttatchments.Add(attatchment);
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
            TopicAttatchment attachment;
            try
            {
                attachment = DbContext.TopicAttatchments.Include(t => t.Topic).ThenInclude(t => t.TopicUsers).Single(t => t.Id == attachmentId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Attachment");
            }

            var topicFolder = Path.Combine(Constants.AttatchmentPath, attachment.TopicId.ToString());
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

                new NotificationProcessor(DbContext, attachment.Topic, identity).OnAttachmetAdded(attachment.Name);

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
                    var fileName = Path.Combine(Constants.AttatchmentFolder, topicId.ToString(), attachment.Path);
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
        #region Legal

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        private Legal GetLegalById(int attachmentId)
        {
            return DbContext.Legals.Single(l => (l.TopicAttatchmentId == attachmentId));
        }

        internal EntityResult CreateLegal(int topicId, int attachmentId, string identity, LegalFormModel legalModel)
        {
            try
            {
                DbContext.TopicAttatchments.Include(at => at.Legal).Single(at => at.Id == attachmentId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Attachment");
            }
            // TODO already exitst´s
            try
            {
                Legal legal = new Legal(attachmentId, legalModel);

                DbContext.Add(legal);
                DbContext.SaveChanges();

                return EntityResult.Successfull();
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }

        public bool DeleteLegal(int topicId, int attachmentId)
        {
            try
            {
                var legal = GetLegalById(attachmentId);
                DbContext.Remove(legal);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        #endregion
    }
}
