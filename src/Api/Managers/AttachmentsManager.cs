using Api.Data;
using Api.Models;
using Api.Models.Entity;
using Api.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Api.Managers
{
    public class AttachmentsManager : BaseManager
    {
        public AttachmentsManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual TopicAttatchment GetAttachmentById(int topicId, int attachmentId)
        {
            return dbContext.TopicAttatchments.Single(ta => (ta.TopicId == topicId && ta.Id == attachmentId));
        }

        public virtual List<int> GetAttachments(int topicId)
        {
            return dbContext.TopicAttatchments.Where(ta => (ta.TopicId == topicId)).Select(ta => ta.Id).ToList();
        }

        public AddEntityResult CreateAttachment(int topicId, int userId, AttatchmentFormModel model, IFormFile file)
        {
            var topic = dbContext.Topics.Single(t => t.Id == topicId);
            if (topic == null)
                return new AddEntityResult() { Success = false, ErrorMessage = "Unknown Topic" };

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
                attatchment.Path = fileName;
                attatchment.Type = "TODO";

                dbContext.TopicAttatchments.Add(attatchment);
                dbContext.SaveChanges();

                new NotificationProcessor(dbContext, topic, userId).OnAttachmetAdded(model.Name);

                return new AddEntityResult() { Success = true, Value = attatchment.Id };
            }
            catch (Exception e)
            {
                return new AddEntityResult() { Success = false, ErrorMessage = e.Message };
            }
        }


        public bool DeleteAttachment(int topicId, int attachmentId)
        {
            var attachment = GetAttachmentById(topicId, attachmentId);
            if (attachment != null)
            {
                string fileName = Path.Combine(Constants.AttatchmentPath, attachment.Path);
                DeleteFile(fileName);
                dbContext.Remove(attachment);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
