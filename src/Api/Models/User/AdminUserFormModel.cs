using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BOL.Models
{
    public class AdminUserFormModel : UserFormModel
    {
        [Required]
        public string Role { get; set; }
    }
}
