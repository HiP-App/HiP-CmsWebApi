using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOL.Models
{
    public class UserTopic
    {
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }

        [Key]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
