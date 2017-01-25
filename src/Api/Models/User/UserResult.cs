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
        }

        public int Id { get; set; }
        
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public string FullName
        {
            get { return FirstName + ' ' + LastName; }
        }
    }
}
