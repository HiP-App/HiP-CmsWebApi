using System.ComponentModel.DataAnnotations;

namespace Api.Models.Shared
{
    public class StringWrapper
    {
        [Required]
        public string Value { get; set; }
    }
}
