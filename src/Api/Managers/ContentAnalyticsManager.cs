using Api.Data;
using Api.Models.AnnotationAnalytics;
using System.Collections.Generic;
using System.Linq;

namespace Api.Managers
{
    public class ContentAnalyticsManager : BaseManager
    {
        public ContentAnalyticsManager(CmsDbContext dbContext) : base(dbContext) { }

        public IEnumerable<TagFrequency> GetFrequencyAnalytic(int topicId)
        {
            var document = DbContext.Documents.First(d => d.TopicId == topicId);
            var tags = DbContext.AnnotationTagInstances
                .Where(t => t.InDocument == document)
                .GroupBy(t => new { t.Value, t.TagModelId });


            List<TagFrequency> result = new List<TagFrequency>();
            foreach (var tag in tags)
            {
                result.Add(new TagFrequency(tag.Key.TagModelId, tag.Key.Value, tag.Count()));
            }
            return result;
        }
    }
}