using Api.Managers;
using Api.Data;
using Api.Models.Entity;

namespace Api.Tests.Nunit.Mocks
{
    public class UserManagerMock : UserManager
    {
        public UserManagerMock(CmsDbContext dbContext) : base(dbContext) { }

        public override User GetUserById(int userId)
        {
            if (userId == 1)
            {
                return new User
                {
                    Id = userId,
                    Email = "admin@hipapp.de",
                    Role = "Administrator"
                };
            }

            else if(userId == 2)
            {
                return new User
                {
                    Id = userId,
                    Email = "abc@hipapp.de",
                    Role = "Student"
                };
            }

            else if (userId == 3)
            {
                return new User
                {
                    Id = userId,
                    Email = "abc@hipapp.de",
                    Role = "Supervisor"
                };
            }

            return new User
            {
                Id = userId,
                Email = "abc@hipapp.de",
                Role = "Reviewer"
            };
        }
    }
}
