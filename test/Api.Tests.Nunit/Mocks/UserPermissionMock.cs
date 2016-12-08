using Api.Data;
using Api.Permission;
using Api.Models;

namespace Api.Tests.Nunit.Mocks
{
    public class UserPermissionMock : UserPermissions
    {
        private UserManagerMock userManagerMock;

        public UserPermissionMock(CmsDbContext dbContext) : base(dbContext)
        {
            this.userManagerMock = new UserManagerMock(dbContext);
        }

        public override bool IsAllowedToAdminister(int userId)
        {
            var user = userManagerMock.GetUserById(userId);
            return user.Role.Equals(Role.Administrator);
        }
    }
}
