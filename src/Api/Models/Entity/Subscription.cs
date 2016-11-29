using Api.Models.User;
using System.ComponentModel.DataAnnotations;
using Api.Models.Notifications;

namespace Api.Models.Entity
{
    public class Subscription
    {

        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        public UserResult Subscriber { get; set; }

        [Required]
        public NotificationType Type { get; set; }
    }
}
