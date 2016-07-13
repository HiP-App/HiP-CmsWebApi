using System.Collections.Generic;

namespace BOL.Models
{
    public class Student : User
    {
        public string MatriculationNumber { get; set; }

        public virtual ICollection<UserTopic> StudentTopics { get; set; }
    }
}
