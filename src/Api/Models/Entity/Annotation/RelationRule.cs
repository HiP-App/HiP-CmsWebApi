using System.ComponentModel.DataAnnotations;

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
