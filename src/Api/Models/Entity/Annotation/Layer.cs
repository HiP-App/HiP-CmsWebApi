using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable CollectionNeverUpdated.Global

namespace Api.Models.Entity.Annotation
{
    public class Layer
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<LayerRelationRule> Relations { get; set; }

        public List<LayerRelationRule> IncomingRelations { get; set; }
    }
}
