using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.User
{
    public class UsersFormModel
    {
        [Required]
        public string[] Users { get; set; }
    }
}
