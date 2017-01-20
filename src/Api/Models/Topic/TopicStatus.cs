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
            return status.CompareTo(Todo) == 0 ||
                status.CompareTo(InProgress) == 0 ||
                status.CompareTo(InReview) == 0 ||
                status.CompareTo(Done) == 0;
        }
    }
}