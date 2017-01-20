using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Shared
{
    public class StringWrapper
    {
        [Required]
        public string Value { get; set; }
    }
}
