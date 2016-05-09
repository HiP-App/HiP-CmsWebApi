using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.ViewModels.UserProfile
{
    public class AdminCredentialsViewModel
    {
        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
    }
}
