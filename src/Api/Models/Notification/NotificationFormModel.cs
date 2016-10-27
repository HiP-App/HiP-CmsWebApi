using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class NotificationFormModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChangedByUserId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public bool IsReadOrNot { get; set; }
    }
}
