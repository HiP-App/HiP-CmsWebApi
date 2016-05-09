using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.ViewModels.UserProfile
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string Id { get; set; }
        public IList<string> Roles { get; set; }
    }
}
