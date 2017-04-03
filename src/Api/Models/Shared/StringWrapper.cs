using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Shared
{
    public class StringWrapper
    {
        [Required]
        public string Value { get; set; }
    }
}
