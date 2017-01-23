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
        public virtual TopicAttatchment GetAttachmentById(int attachmentId)
        {
            return dbContext.TopicAttatchments.Single(ta => (ta.Id == attachmentId));
        }

        public virtual IEnumerable<TopicAttachmentResult> GetAttachments(int topicId)
        {
            return dbContext.TopicAttatchments.Where(ta => (ta.TopicId == topicId)).Include(ta => ta.User).Include(ta => ta.Legal).ToList().Select(at => new TopicAttachmentResult(at));
        }

        public EntityResult CreateAttachment(int topicId, int userId, AttatchmentFormModel model)
        {
            try
            {
                dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Topic");
            }

            try
            {
                var attatchment = new TopicAttatchment(model)
                {
                    UserId = userId,
                    TopicId = topicId,
                    Type = "TODO"
                };
                
                dbContext.TopicAttatchments.Add(attatchment);
                dbContext.SaveChanges();

                return EntityResult.Successfull(attatchment.Id);
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }
        public EntityResult PutAttachment(int attachmentId, int userId, IFormFile file)
        {
            TopicAttatchment attachment;
            try
            {
                attachment = dbContext.TopicAttatchments.Include(t => t.Topic).ThenInclude(t=> t.TopicUsers).Single(t => t.Id == attachmentId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Attachment");
            }

            var topicFolder = Path.Combine(Constants.AttatchmentPath, attachment.TopicId.ToString());
            if (!System.IO.Directory.Exists(topicFolder))
                System.IO.Directory.CreateDirectory(topicFolder);

            string fileName = (Path.Combine(topicFolder, file.FileName));
            using (FileStream outputStream = new FileStream(fileName, FileMode.Create))
            {
                file.CopyTo(outputStream);
            }
            try
            {
                attachment.Path = file.FileName;
                attachment.Type = "TODO";

                dbContext.Update(attachment);
                dbContext.SaveChanges();

                new NotificationProcessor(dbContext, attachment.Topic, userId).OnAttachmetAdded(attachment.Name);

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
                string fileName = Path.Combine(Constants.AttatchmentFolder, topicId.ToString(), attachment.Path);
                DeleteFile(fileName);
                dbContext.Remove(attachment);
                dbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        #region Legal

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual Legal GetLegalById(int attachmentId)
        {
            return dbContext.Legals.Single(l => (l.TopicAttatchmentId == attachmentId));
        }

        internal EntityResult CreateLegal(int topicId, int attachmentId, int userId, LegalFormModel legalModel)
        {
            TopicAttatchment attachment;
            try
            {
                attachment = dbContext.TopicAttatchments.Include(at => at.Legal).Single(at => at.Id == attachmentId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Attachment");
            }
            // TODO already exitst´s
            try
            {
                Legal legal = new Legal(attachmentId, legalModel);

                dbContext.Add(legal);
                dbContext.SaveChanges();

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
                dbContext.Remove(legal);
                dbContext.SaveChanges();
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
