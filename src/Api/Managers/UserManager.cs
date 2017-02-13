using System.Collections.Generic;
using System.Linq;
using Api.Data;
using Api.Models;
using System;
using Api.Models.Entity;
using Api.Models.User;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(CmsDbContext dbContext) : base(dbContext) { }

        public virtual IEnumerable<UserResult> GetAllUsers(string query, string role, int page, int pageSize)
        {
            var qry = DbContext.Users.Select(u => u)
               .Where(u => 
                   (string.IsNullOrEmpty(query) || (u.Email.Contains(query) || u.FirstName.Contains(query) || u.LastName.Contains(query)))
                   && (string.IsNullOrEmpty(role) || u.Role == role)).Include(u => u.StudentDetails);

            return qry.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(user => new UserResult(user));
        }

        public virtual int GetUsersCount()
        {
            return DbContext.Users.Count();
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual User GetUserById(int userId)
        {
            return DbContext.Users.Single(u => u.Id == userId);
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual User GetUserByEmail(string email)
        {
            return DbContext.Users.Single(u => u.Email == email);
        }

        /// <exception cref="InvalidOperationException">The input sequence contains more than one element. -or- The input sequence is empty.</exception>
        public virtual User GeStudentById(int userId)
        {
            return DbContext.Users.Include(u => u.StudentDetails).Single(u => u.Id == userId && string.Equals(u.Role,Role.Student));
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

                DbContext.SaveChanges();
                return true;
            }
            return false;
        }

        private void AddUserbyEmail(string email)
        {
            var user = new User
            {
                Email = email,
                Role = Role.Student
            };

            AddUser(user);
        }

        public bool AddUser(User user)
        {
            try
            {
                DbContext.Add(user);
                DbContext.SaveChanges();
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
                    DbContext.SaveChanges();
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

        /// <summary>
        /// Checks whether the given email is already used for any user in the database.
        /// </summary>
        /// <param name="email">The email to search for</param>
        /// <returns>true if the email is already used</returns>
        private bool IsExistingEmail(string email)
        {
            return DbContext.Users.Any(u => u.Email == email);
        }

        public struct InvitationResult
        {
            public List<string> FailedInvitations;
            public List<string> ExistingUsers;
        }

        /// <summary>
        /// Invite the users identified by the given email addresses.
        /// This also creates users in the database.
        /// Existing users and failed invitation attempts are added to
        /// separate lists and returned in a tuple.
        /// </summary>
        /// <param name="emails">A string array of email addresses</param>
        /// <param name="emailSender">Email Service</param>
        /// <returns>Tuple containing (1) the failed invitations and (2) existing users lists.</returns>
        public InvitationResult InviteUsers(IEnumerable<string> emails, IEmailSender emailSender)
        {
            var failedInvitations = new List<string>();
            var existingUsers = new List<string>();
            foreach (var email in emails)
            {
                try
                {
                    if (IsExistingEmail(email))
                    {
                        existingUsers.Add(email);
                    }
                    else
                    {
                        AddUserbyEmail(email);
                        emailSender.InviteAsync(email);
                    }
                }

                catch (DbUpdateException)
                {
                    //user already exists in Database
                    existingUsers.Add(email);
                }
            }
            return new InvitationResult() { FailedInvitations = failedInvitations, ExistingUsers = existingUsers };
        }

        public bool PutStudentDetials(User student, StudentFormModel model)
        {
            try
            {
                student.StudentDetails = new Models.Entity.StudentDetails(student, model);
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
            }
            return false;
        }
    }
}