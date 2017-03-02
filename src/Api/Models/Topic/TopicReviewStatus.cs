using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Topic
{
    public class TopicReviewStatus
    {
        [Required]
        public string Status { get; set; }

        public bool IsStatusValid()
        {
            return IsStatusValid(Status);
        }

        public bool IsReviewed()
        {
            return string.Compare(Status, Reviewed, StringComparison.Ordinal) == 0;
        }

        public const string NotReviewed = "NotReviewed";
        private const string InReview = "InReview";
        private const string Reviewed = "Reviewed";

        public static bool IsStatusValid(TopicReviewStatus status)
        {
            if (status == null)
                return false;
            return IsStatusValid(status.Status);
        }

        public static bool IsStatusValid(string status)
        {
            return string.Compare(status, NotReviewed, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InReview, StringComparison.Ordinal) == 0 ||
                string.Compare(status, Reviewed, StringComparison.Ordinal) == 0;
        }
    }
}
