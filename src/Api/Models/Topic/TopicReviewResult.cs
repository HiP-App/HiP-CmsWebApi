﻿using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic
{
    public class TopicReviewResult
    {
        public TopicReviewResult(TopicReview review)
        {
            Status = new TopicReviewStatus() { Status = review.Status };
            TimeStamp = review.TimeStamp;
            if (review.Reviewer != null)
                Reviewer = new UserResultLegacy(review.Reviewer);
        }

        public TopicReviewResult(UserResultLegacy reviewer)
        {
            // No Review present!
            Reviewer = reviewer;
            Status = new TopicReviewStatus() { Status = TopicReviewStatus.NotReviewed };
        }

        public TopicReviewStatus Status { get; set; }

        public DateTime TimeStamp { get; set; }

        public UserResultLegacy Reviewer { get; set; }
    }
}
