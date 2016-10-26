using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOL.Models
{
    public class NotificationFormModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChangedById { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public bool IsReadOrNot { get; set; }
    }
}
