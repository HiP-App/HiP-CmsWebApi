using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity.Annotation
{
    public class RelationRule
    {
        [Key]
        public int Id { get; set; }

        public string ArrowStyle { get; set; }

        public string Color { get; set; }
    }
}
