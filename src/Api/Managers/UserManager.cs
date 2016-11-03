using System.Collections.Generic;
using System.Linq;
using Api.Data;
using Api.Models;
using System;
using Api.Models.Entity;
using Api.Models.User;

namespace Api.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<UserResult> GetAllUsers(string query, string role, int page, int pageSize)
        {
            var qry = dbContext.Users.Select(u => u);

            if (!string.IsNullOrEmpty(query))
                qry = qry.Where(u =>  u.Email.Contains(query) ||  u.FirstName.Contains(query) || u.LastName.Contains(query));

            if (!string.IsNullOrEmpty(role))
                qry = qry.Where(u => u.Role == role);

            var users = qry.ToList().Select(user => new UserResult(user));

            return users.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public virtual int GetUsersCount()
        {
            return dbContext.Users.Count();
        }

        public virtual User GetUserById(int userId)
        {
            return dbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        public virtual User GetUserByEmail(string email)
        {
            return dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public virtual bool UpdateUser(int userId, UserFormModel model)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                if (model is AdminUserFormModel)
                    user.Role = ((AdminUserFormModel)model).Role;

                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public virtual bool AddUser(User user)
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
        public virtual bool UpdateProfilePicture(User user, String fileName)
        {
            if (user != null)
            {
                try
                {
                    user.ProfilePicture = fileName;
                    dbContext.SaveChanges();
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