using System.Collections.Generic;

namespace BOL.Models
{
    public class Supervisor : User
    {
        public virtual ICollection<UserTopic> SupervisorTopics { get; set; }
    }
}
