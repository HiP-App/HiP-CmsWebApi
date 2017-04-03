using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.AnnotationAnalytics
{
    public class TagFrequencyAnalyticsResult
    {
        public IEnumerable<TagFrequency> TagFrequency { get; set; }

    }

    public class TagFrequency
    {
        public string Value { get; set; }
        public int Count { get; set; }
        public int TagId { get; set; }

        public TagFrequency(int tagId, string value, int count)
        {
            Value = value;
            TagId = tagId;
            Count = count;
        }
    }
}