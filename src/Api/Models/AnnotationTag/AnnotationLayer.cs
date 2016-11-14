using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.AnnotationTag
{
    public class AnnotationLayer
    {
        public const string Place = "Raum";
        public const string Time = "Zeit";
        public const string Perspective = "Perspective";

        public bool isValidLayer(string layer)
        {
            return Place.Equals(layer) || Time.Equals(layer) || Perspective.Equals(layer);
        }
    }
}
