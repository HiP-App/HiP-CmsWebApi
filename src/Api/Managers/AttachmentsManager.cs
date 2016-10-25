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
            var topic = dbContext.Topics.First(t => t.Id == topicId);
            if (topic != null)
                return topic.Attatchments.First(a => a.Id == attachmentId);
            return null;
        }

        public virtual List<int> GetAttachment(int topicId)
        {
            var topic = dbContext.Topics.First(t => t.Id == topicId);
            if (topic != null)
                return topic.Attatchments.Select(o => o.Id).ToList();
            return null;
        }


        public AddEntityResult CreateAttachment(int topicId, int userId, AttatchmentFormModel model, IFormFile file)
        {
            var topic = dbContext.Topics.First(t => t.Id == topicId);
            if (topic == null)
                return new AddEntityResult() { Success = false, ErrorMessage = "Unknown Topic" };

            string topicFolder = Path.Combine(Constants.AttatchmentFolder, topicId.ToString());
            if (!System.IO.Directory.Exists(topicFolder))
                System.IO.Directory.CreateDirectory(topicFolder);

            string fileName = (Path.Combine(topicFolder, file.FileName));
            using (FileStream outputStream = new FileStream(fileName, FileMode.Create))
            {
                file.CopyTo(outputStream);
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var attatchment = new TopicAttatchment(model);
                    attatchment.UserId = userId;
                    attatchment.TopicId = topic.Id;
                    attatchment.Path = fileName;
                    attatchment.Type = "TODO";

                    topic.Attatchments.Add(attatchment);
                    dbContext.SaveChanges();

                    transaction.Commit();
                    return new AddEntityResult() { Success = true, Value = attatchment.Id };
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new AddEntityResult() { Success = false, ErrorMessage = e.Message };
                }
            }
        }


        public bool DeleteAttachment(int topicId, int attachmentId)
        {
            var attachment = GetAttachmentById(topicId, attachmentId);
            if (attachment != null)
            {
                string fileName = Path.Combine(Constants.AttatchmentFolder, attachment.Path);
                DeleteFile(fileName);
                dbContext.Remove(attachment);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
