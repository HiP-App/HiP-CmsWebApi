using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.User
{
    public class Base64Image
    {
        public Base64Image(string base64)
        {
            this.Base64 = base64;
        }

        public string Base64 { get; set; }
    }
}
