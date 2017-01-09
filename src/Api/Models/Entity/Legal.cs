using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Entity
{
    public class Legal
    {
        public virtual TopicAttatchment TopicAttatchment { get; set; }

        public virtual int TopicAttatchmentId { get; set; }
    }
}
