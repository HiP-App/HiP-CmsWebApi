using PaderbornUniversity.SILab.Hip.CmsApi.Data;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationAnalytics;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public class ContentAnalyticsManager : BaseManager
    {
        public ContentAnalyticsManager(CmsDbContext dbContext) : base(dbContext) { }

        public TagFrequencyAnalyticsResult GetFrequencyAnalytic(int topicId)
        {
            var document = DbContext.Documents.First(d => d.TopicId == topicId);
            var tags = DbContext.AnnotationTagInstances
                .Where(t => t.Document == document)
                .GroupBy(t => new { t.Value, t.TagModelId });


            List<TagFrequency> result = new List<TagFrequency>();
            foreach (var tag in tags)
            {
                result.Add(new TagFrequency(tag.Key.TagModelId, tag.Key.Value, tag.Count()));
            }
            return new TagFrequencyAnalyticsResult {TagFrequency = result};
        }
    }
}