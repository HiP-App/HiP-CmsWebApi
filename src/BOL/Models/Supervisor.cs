using System.Collections.Generic;

namespace BOL.Models
{
    public class Supervisor : User
    {
        public virtual ICollection<SupervisorTopic> SupervisorTopics { get; set; }

        public virtual ICollection<Topic> ReviewTopic { get; set; }
    }
}
