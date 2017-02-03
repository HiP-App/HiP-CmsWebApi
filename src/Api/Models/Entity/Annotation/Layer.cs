using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity.Annotation
{
    public class Layer
    {
        [Key]
        public string Name { get; set; }
    }
}
