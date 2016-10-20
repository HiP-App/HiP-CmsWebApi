namespace BOL.Models
{
    public class Role
    {
        public const string Student = "Student";
        public const string Supervisor = "Supervisor";
        public const string Administrator = "Administrator";
        public const string Reviewer = "Reviewer";

        public static bool IsRoleValid(string role)
        {
            return role.CompareTo(Student) == 0 || 
                role.CompareTo(Supervisor) == 0 || 
                role.CompareTo(Administrator) == 0;
        }
    }
}
