using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Topic
{
    public class HtmlContentModel
    {
        [Required]
        public string HtmlContent { get; set; }
    }
}
