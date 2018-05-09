using PaderbornUniversity.SILab.Hip.CmsApi.Models;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Topic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Managers
{
    public partial class TopicManager
    {
        public IEnumerable<TopicReviewResult> GetReviews(int topicId)
        {
            var result = new List<TopicReviewResult>();
            var reviews = DbContext.TopicReviews.Where(rs => rs.TopicId == topicId).ToList();
            var reviewers = GetAssociatedUsersByRole(topicId, Role.Reviewer);

            foreach (var reviewer in reviewers)
            {
                try
                {
                    var review = reviews.Single(r => r.ReviewerId == reviewer);
                    result.Add(new TopicReviewResult(review));
                }
                catch (InvalidOperationException)
                {
                    result.Add(TopicReviewResult.NotReviewed(reviewer));
                }
            }
            return result;
        }

        public bool ChangeReviewStatus(string userId, int topicId, TopicReviewStatus status)
        {
            try
            {
                if (!DbContext.TopicReviews.Any(rs => rs.TopicId == topicId && rs.ReviewerId == userId))
                {
                    var review = new TopicReview
                    {
                        TopicId = topicId,
                        ReviewerId = userId,
                        Status = status.Status
                    };
                    DbContext.TopicReviews.Add(review);
                }
                else
                {
                    var review = DbContext.TopicReviews.Single(rs => rs.TopicId == topicId && rs.ReviewerId == userId);
                    review.Status = status.Status;
                    DbContext.Update(review);
                }
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
