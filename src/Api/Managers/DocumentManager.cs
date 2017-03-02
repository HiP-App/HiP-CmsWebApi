using Api.Data;
using Api.Models;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Api.Managers
{
    public class DocumentManager : BaseManager
    {
        public DocumentManager(CmsDbContext dbContext) : base(dbContext) { }


        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public Document GetDocumentById(int topicId)
        {
            return DbContext.Documents.Include(d => d.Updater).Single(d => (d.TopicId == topicId));
        }

        internal EntityResult UpdateDocument(int topicId, string userIdenty, string htmlContent)
        {
            try
            {
                DbContext.Topics.Include(t => t.Document).Single(t => t.Id == topicId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Topic");
            }
            // already exitsts
            
            var userId = DbContext.Users.Select()
            Document document;
            try
            {
                document = GetDocumentById(topicId);
                document.UpdaterId = userId;
                document.Content = htmlContent;
            }
            catch (InvalidOperationException)
            {
                document = new Document(topicId, userId, htmlContent);
                DbContext.Add(document);
            }
            try
            {
                DbContext.SaveChanges();
                return EntityResult.Successfull();
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }
        }


        public bool DeleteDocument(int topicId)
        {
            try
            {
                var document = GetDocumentById(topicId);
                DbContext.Remove(document);
                DbContext.SaveChanges();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
