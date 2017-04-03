using System;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models
{
    public class TopicStatus
    {
        [Required]
        public string Status { get; set; }
        
        public bool IsStatusValid()
        {
            return IsStatusValid(Status);
        }

        public bool IsDone()
        {
            return string.Compare(Status, Done, StringComparison.Ordinal) == 0;
        }

        private const string Todo = "Todo";
        private const string InProgress = "InProgress";
        private const string InReview = "InReview";
        private const string InProgressAgain = "InProgressAgain";
        private const string Done = "Done";

        public static bool IsStatusValid(string status)
        {
            return string.Compare(status, Todo, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InProgress, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InReview, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InProgressAgain, StringComparison.Ordinal) == 0 ||
                string.Compare(status, Done, StringComparison.Ordinal) == 0;
        }
    }
}