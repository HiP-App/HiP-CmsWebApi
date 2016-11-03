using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.User
{
    public class UserResult
    {
        public UserResult(Api.Models.Entity.User user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Role = user.Role;
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
