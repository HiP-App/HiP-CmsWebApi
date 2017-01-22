using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class AttatchmentFormModel
    {
        [Required]
        public String AttatchmentName { get; set; }

        [Required]
        public String Description { get; set; }
    }
}
