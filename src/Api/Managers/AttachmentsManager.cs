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
            return dbContext.TopicAttatchments.Where(ta => (ta.TopicId == topicId)).Include(ta => ta.User).ToList().Select(at => new TopicAttachmentResult(at));
        }

        public EntityResult CreateAttachment(int topicId, int userId, AttatchmentFormModel model, IFormFile file)
        {
            Topic topic;
            try
            {
                topic = dbContext.Topics.Include(t => t.TopicUsers).Single(t => t.Id == topicId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Topic");
            }

            string topicFolder = Path.Combine(Constants.AttatchmentPath, topicId.ToString());
            if (!System.IO.Directory.Exists(topicFolder))
                System.IO.Directory.CreateDirectory(topicFolder);

            string fileName = (Path.Combine(topicFolder, file.FileName));
            using (FileStream outputStream = new FileStream(fileName, FileMode.Create))
            {
                file.CopyTo(outputStream);
            }
            try
            {
                var attatchment = new TopicAttatchment(model);
                attatchment.UserId = userId;
                attatchment.TopicId = topicId;
                attatchment.Path = file.FileName;
                attatchment.Type = "TODO";

                dbContext.TopicAttatchments.Add(attatchment);
                dbContext.SaveChanges();

                new NotificationProcessor(dbContext, topic, userId).OnAttachmetAdded(model.AttatchmentName);

                return EntityResult.Successfull(attatchment.Id);
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

    }
}
}
