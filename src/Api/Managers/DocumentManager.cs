using Api.Data;
using Api.Models;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Api.Models.Entity.Annotation;

namespace Api.Managers
{
    public class DocumentManager : BaseManager
    {
        public DocumentManager(CmsDbContext dbContext) : base(dbContext)
        {
        }


        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public Document GetDocumentById(int topicId)
        {
            return DbContext.Documents.Include(d => d.Updater).Single(d => (d.TopicId == topicId));
        }

        internal EntityResult UpdateDocument(int topicId, int userId, string htmlContent)
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
                //return EntityResult.Successfull();
            }
            catch (Exception e)
            {
                return EntityResult.Error(e.Message);
            }

            // document is saved, so now we can parse it
            System.IO.StringReader stream =
                new System.IO.StringReader("<pseudo-root>" + htmlContent + "</pseudo-root>");
            XmlReader xmlReader = XmlReader.Create(stream);

            List<AnnotationTagInstance> tagInstances = new List<AnnotationTagInstance>();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.HasAttributes)
                        {
                            int tagModelId = Int32.Parse(xmlReader.GetAttribute("data-tag-model-id"));
                            int tagInstanceId = Int32.Parse(xmlReader.GetAttribute("data-tag-id"));
                            string tagValue = xmlReader.ReadElementContentAsString();
                            Regex rx = new Regex("<span[^>]+?data-tag-id=\"" + tagInstanceId + "\".*?>");
                            int tagPosition = rx.Match(htmlContent).Index;

                            AnnotationTag tagModel = DbContext.AnnotationTags.First(t => t.Id == tagModelId);
                            AnnotationTagInstance tag = new AnnotationTagInstance(tagModel);
                            tag.IdInDocument = tagInstanceId;
                            tag.Value = tagValue;
                            tag.PositionInDocument = tagPosition;
                            tag.InDocument = document;

                            tagInstances.Add(tag);
                        }
                        break;
                }
            }
            DbContext.AnnotationTagInstances.AddRange(tagInstances);
            DbContext.SaveChanges();

            return EntityResult.Successfull();
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