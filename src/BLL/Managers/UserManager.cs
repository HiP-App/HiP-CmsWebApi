using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOL.Data;
using BOL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BLL.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual async Task<IEnumerable<User>> GetAllUsersAsync(string query, string role, int page, int pageSize)
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

            return await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<User> GetUserByIdAsync(int id)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        //To Check the user with email
        public virtual async Task<User> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

       
        public virtual async Task<int> GetUsersCountAsync()
        {
            return await dbContext.Users.CountAsync();
        }

        public virtual async Task<bool> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                await dbContext.Database.ExecuteSqlCommandAsync($"UPDATE \"User\" SET \"Role\" = '{newRole}' where \"Id\" = {userId}");
                return true;
            }
            else
                return false;
        }        

        //Method to Add a User
        public virtual async Task<User> AddUserAsync(string username, string role)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Email == username);
            if (user == null)
            {                
                //Check if the user is an Admin or default
                if (role == Role.Administrator)
                {
                    user = new Administrator()
                    {
                        Email = username
                    };
                }
                else
                {
                    user = new Student()
                    {
                        Email = username
                    };
                }

                try
                {
                    dbContext.Add(user);
                    await dbContext.SaveChangesAsync();
                }catch(Exception ex)
                {
                    return user;
                }
                
                return user;
            }
            //If user already exists then return false
            return user;
        }

    }
}
