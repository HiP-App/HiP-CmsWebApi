﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity.Annotation
{
    public class Layer
    {
        [Key]
        public int Id { get; set; }

        public String Name { get; set; }

        public List<LayerRelationRule> Relations { get; set; }

        public List<LayerRelationRule> IncomingRelations { get; set; }
    }
}
