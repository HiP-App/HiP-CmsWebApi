using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(CmsDbContext dbContext) : base(dbContext) { }

        public IQueryable<User> GetAllUsers(string query = null, string role = null, int page = 1)
        {
            var users = from u in dbContext.Users
                        select u;

            if (!string.IsNullOrEmpty(query))
                users = users.Where(u =>
                    u.Email.Contains(query) ||
                    u.FirstName.Contains(query) ||
                    u.LastName.Contains(query));

            if (!string.IsNullOrEmpty(role))
                users = users.Where(u => u.Role == role);

            return users;
        }

        public User GetUserById(int id)
        {
            return dbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUserRole(int userId, string newRole)
        {
            dbContext.Database.ExecuteSqlCommand($"UPDATE \"User\" SET \"Role\" = '{newRole}' where \"Id\" = {userId}");
        }

        public int GetUsersCount()
        {
            return dbContext.Users.Count();
        }
    }
}
