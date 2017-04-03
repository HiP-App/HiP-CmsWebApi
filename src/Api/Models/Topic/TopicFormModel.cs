using System;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models
{
    public class TopicFormModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime? Deadline { get; set; }
        [Required]
        public string Description { get; set; }

        public string Requirements { get; set; }   

    }
}
