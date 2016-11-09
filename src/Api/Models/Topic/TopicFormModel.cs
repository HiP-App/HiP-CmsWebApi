using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class TopicFormModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime? Deadline { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }   
    }
}