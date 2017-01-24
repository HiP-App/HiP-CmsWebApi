using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class TopicStatus
    {
        [Required]
        public string Status { get; set; }
        
        public bool IsStatusValid()
        {
            return IsStatusValid(Status);
        }

        public const string Todo = "Todo";
        public const string InProgress = "InProgress";
        public const string InReview = "InReview";
        public const string Done = "Done";

        public static bool IsStatusValid(string status)
        {
            return string.Compare(status, Todo, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InProgress, StringComparison.Ordinal) == 0 ||
                string.Compare(status, InReview, StringComparison.Ordinal) == 0 ||
                string.Compare(status, Done, StringComparison.Ordinal) == 0;
        }
    }
}