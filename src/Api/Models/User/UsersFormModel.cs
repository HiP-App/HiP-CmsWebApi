using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.User
{
    public class UsersFormModel
    {
        [Required]
        public int[] Users { get; set; }
    }
}
