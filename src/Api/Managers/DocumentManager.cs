using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class DocumentManager : BaseManager
    {
        public DocumentManager(CmsDbContext dbContext) : base(dbContext)
        {
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public Document GetDocumentById(int topicId)
        {
            return DbContext.Documents.Single(d => d.TopicId == topicId);
        }

        internal EntityResult UpdateDocument(int topicId, string userId, string htmlContent)
        {
            try
            {
                DbContext.Topics.Include(t => t.Document).Single(t => t.Id == topicId);
            }
            catch (InvalidOperationException)
            {
                return EntityResult.Error("Unknown Topic");
            }
            // already exists

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

            // Angular app trims img-tags to non-valid xml, let us repair it
            htmlContent = Regex.Replace(htmlContent, "<img ([^>]*)?>", "<img $1 />");

            // document is saved, so now we can parse it
            var stream = new System.IO.StringReader("<pseudo-root>" + htmlContent + "</pseudo-root>");
            var xmlReader = XmlReader.Create(stream);
            var tagInstances = new List<AnnotationTagInstance>();

            try
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element) continue;
                    if (!xmlReader.HasAttributes) continue;
                    if (xmlReader.GetAttribute("data-tag-model-id") == null) continue;
                    if (xmlReader.GetAttribute("data-tag-id") == null) continue;

                    var tagModelId = int.Parse(xmlReader.GetAttribute("data-tag-model-id"));
                    var tagInstanceId = int.Parse(xmlReader.GetAttribute("data-tag-id"));
                    var tagValue = xmlReader.ReadElementContentAsString();
                    var rx = new Regex("<span[^>]+?data-tag-id=\"" + tagInstanceId + "\".*?>");
                    var tagPosition = rx.Match(htmlContent).Index;

                    var tagModel = DbContext.AnnotationTags.First(t => t.Id == tagModelId);
                    var tag = new AnnotationTagInstance(tagModel)
                    {
                        IdInDocument = tagInstanceId,
                        Value = tagValue,
                        PositionInDocument = tagPosition,
                        Document = document
                    };

                    tagInstances.Add(tag);
                }
                if (DbContext.AnnotationTagInstances.Any(i => i.Document == document))
                {
                    var oldInstances = DbContext.AnnotationTagInstances.Where(i => i.Document == document);
                    DbContext.AnnotationTagInstances.RemoveRange(oldInstances);
                }
                DbContext.AnnotationTagInstances.AddRange(tagInstances);
            }
            catch (Exception)
            {
                return EntityResult.Error("Parsing Error");
            }
            
            try
            {
                DbContext.SaveChanges();
                return EntityResult.Successful();
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
