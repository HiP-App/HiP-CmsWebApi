namespace Api.Models.User
{
    public class UserResult
    {
        public UserResult(Entity.User user)
        {
            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
            if (user.StudentDetails != null)
            {
                StudentDetails = new StudentDetails()
                {
                    Discipline = user.StudentDetails.Discipline,
                    CurrentDegree = user.StudentDetails.CurrentDegree,
                    CurrentSemester = user.StudentDetails.CurrentSemester
                };
            }
        }

        public int Id { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public StudentDetails StudentDetails { get; set; }

    public string FullName
        {
            get { return FirstName + ' ' + LastName; }
        }
    }

    public class StudentDetails
    {
        public string Discipline { get; set; }

        public string CurrentDegree { get; set; }

        public short CurrentSemester { get; set; }
    }
}
