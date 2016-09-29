using BOL.Models;

namespace BLL.Authorities
{
    public class BaseAuthority
    {
        // Current User for the request
        protected readonly User user;
        
        public BaseAuthority(User currentUser)
        {
            user = currentUser;
        }
    }
}