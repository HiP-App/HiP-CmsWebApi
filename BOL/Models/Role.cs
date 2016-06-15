namespace BOL.Models
{
    public static class Role
    {
        public const string Student = "Student";
        public const string Supervisor = "Supervisor";
        public const string Administrator = "Administrator";

        public static bool IsRoleValid(string role)
        {
            return role.CompareTo(Student) == 0 || 
                role.CompareTo(Supervisor) == 0 || 
                role.CompareTo(Administrator) == 0;
        }
    }
}
