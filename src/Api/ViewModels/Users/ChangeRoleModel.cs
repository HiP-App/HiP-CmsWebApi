using System.ComponentModel.DataAnnotations;

namespace Api.ViewModels.Users
{
    public class ChangeRoleModel
    {
        [Required(ErrorMessage = "Role is reuired")]
        public string Role { get; set; }
    }
}
