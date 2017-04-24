using System;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models
{
    public static class Role
    {
        public const string Student = "Student";
        public const string Supervisor = "Supervisor";
        public const string Administrator = "Administrator";
        public const string Reviewer = "Reviewer";

        public static bool IsRoleValid(string role)
        {
            return string.Compare(role, Student, StringComparison.Ordinal) == 0 || 
                string.Compare(role, Supervisor, StringComparison.Ordinal) == 0 || 
                string.Compare(role, Administrator, StringComparison.Ordinal) == 0;
        }
    }
}
