using System.ComponentModel.DataAnnotations;

namespace Api.Models
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
        public bool IsRead { get; set; }
    }
}
