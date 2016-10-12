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

        public virtual async Task<int> GetUsersCountAsync()
        {
            return await dbContext.Users.CountAsync();
        }

        public virtual async Task<User> GetUserByIdAsync(int userId)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public virtual async Task<User> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public virtual async Task<bool> UpdateUserAsync(int userId, UserFormModel model)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;

                        await dbContext.SaveChangesAsync();
                        if (model is AdminUserFormModel)
                            await dbContext.Database.ExecuteSqlCommandAsync($"UPDATE \"Users\" SET \"Role\" = '{((AdminUserFormModel) model).Role}' where \"Id\" = {userId}");
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return false;
        }

        public virtual bool AddUserAsync(User user)
        {
            try
            {
                dbContext.Add(user);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // Use dataobject
        public virtual async Task<bool> UpdateProfilePicture(User user, String fileName)
        {
            if (user != null)
            {
                try
                {
                    user.ProfilePicture = fileName;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Console.Error.Write(e);
                    return false;
                }
            }
            return false;
        }

    }
}