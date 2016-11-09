using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.AnnotationTag
{
    public class AnnotationTagResult
    {
        public AnnotationTagResult(Entity.AnnotationTag tag)
        {
            TagId = tag.Id;
            Name = tag.Name;
            ShortName = tag.ShortName;
            Layer = tag.Layer;
            Style = tag.Style;
            Description = tag.Description;
        }

        public int TagId { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string Layer { get; private set; }
        public string Style { get; private set; }
        public string Description { get; private set; }
    }
}
