using System.ComponentModel.DataAnnotations;

namespace BOL.Models
{
    public class UserTopic
    {
        [Key]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }

    public class StudentTopic : UserTopic
    {
        [Key]
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }

    public class SupervisorTopic : UserTopic
    {
        [Key]
        public int SupervisorId { get; set; }
        public Supervisor Supervisor { get; set; }
    }
}
