using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models
{
    public class InviteFormModel
    {
        [Required]
        public string[] Emails { get; set; }
    }
}