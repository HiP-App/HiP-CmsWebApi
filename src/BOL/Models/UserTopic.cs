namespace BOL.Models
{
    public class UserTopic
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SupervisorId { get; set; }
        public Supervisor Supervisor { get; set; }

        public int StudentTopicId { get; set; }
        public StudentTopic StudentTopic { get; set; }

        public int SupervisorTopicId { get; set; }
        public SupervisorTopic SupervisorTopic { get; set; }
    }
}
