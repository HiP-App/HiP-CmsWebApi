using System.Collections.Generic;
using System.Linq;
using Api.Data;
using Api.Models;
using System;
using Api.Models.Entity;
using Api.Models.User;
using Api.Utility;

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

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual User GetUserById(int userId)
        {
            return dbContext.Users.Single(u => u.Id == userId);
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual User GetUserByEmail(string email)
        {
            return dbContext.Users.Single(u => u.Email == email);
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

        internal void AddUserbyEmail(string email)
        {
            User user = new User();
            user.Email = email;
            user.Role = Role.Student;

            this.AddUser(user);
        }

        /// <summary>
        /// Checks whether the given email is already used for any user in the database.
        /// </summary>
        /// <param name="email">The email to search for</param>
        /// <returns>true if the email is already used</returns>
        private bool isExistingEmail(string email)
        {
            return dbContext.Users.Any(u => u.Email == email);
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

        public struct InvitationResult
        {
            public List<String> failedInvitations;
            public List<String> existingUsers;
        }

        /// <summary>
        /// Invite the users identified by the given email addresses.
        /// This also creates users in the database.
        /// Existing users and failed invitation attempts are added to
        /// separate lists and returned in a tuple.
        /// </summary>
        /// <param name="emails">A string array of email addresses</param>
        /// <returns>Tuple containing (1) the failed invitations and (2) existing users lists.</returns>
        public InvitationResult InviteUsers(string[] emails, IEmailSender emailSender)
        {
            List<String> failedInvitations = new List<string>();
            List<String> existingUsers = new List<string>();
            foreach (string email in emails)
            {
                try
                {
                    if (isExistingEmail(email))
                    {
                        existingUsers.Add(email);
                    } else
                    {
                        AddUserbyEmail(email);
                        emailSender.InviteAsync(email);
                    }
                }

                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    //user already exists in Database
                    existingUsers.Add(email);
                }
                catch (MailKit.Net.Smtp.SmtpCommandException)
                {
                    //something went wrong when sending email
                    failedInvitations.Add(email);
                }
            }
            return new InvitationResult() { failedInvitations = failedInvitations, existingUsers = existingUsers };
        }

    }
}