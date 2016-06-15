using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.ViewModels.Users
{
    public class ChangeRoleModel
    {
        [Required(ErrorMessage = "Role is reuired")]
        public string Role { get; set; }
    }
}
