using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class AttatchmentFormModel
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Description { get; set; }
    }
}
