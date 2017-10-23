using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class TopicReviewResult
    {
        private TopicReviewResult()
        {
        }

        public TopicReviewResult(TopicReview review)
        {
            Status = new TopicReviewStatus { Status = review.Status };
            TimeStamp = review.TimeStamp;
            if (review.ReviewerId != null)
                Reviewer = review.ReviewerId;
        }

        public static TopicReviewResult NotReviewed(string reviewerId)
        {
            // No Review present!
            return new TopicReviewResult
            {
                Reviewer = reviewerId,
                Status = new TopicReviewStatus { Status = TopicReviewStatus.NotReviewed }
            };
        }

        public TopicReviewStatus Status { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Reviewer { get; set; }
    }
}
