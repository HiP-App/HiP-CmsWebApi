﻿using Api.Data;
using Api.Managers;
using Api.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Api.Permission
{
    public class TopicPermissions : BaseManager
    {
        // TODO change User.Email to the Identy if changed!
        private readonly UserManager _userManager;


        public TopicPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }


        public bool IsAllowedToEdit(string userIdenty, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (DbContext.Topics.Include(t => t.CreatedBy).Any(t => (t.Id == topicId && t.CreatedById == user.Id)))
                    return true;
                // Supervisor?
                if (DbContext.TopicUsers.Include(t => t.User).Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Supervisor)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool IsAssociatedTo(string userIdenty, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                if (user.Role.Equals(Role.Administrator))
                    return true;
                // Created?
                if (DbContext.Topics.Include(t => t.CreatedBy).Any(t => (t.Id == topicId && t.CreatedById == user.Id)))
                    return true;
                // Is associated
                if (DbContext.TopicUsers.Include(t => t.User).Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool IsAllowedToCreate(string userIdenty)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                return user.Role.Equals(Role.Administrator) || user.Role.Equals(Role.Supervisor);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsReviewer(string userIdenty, int topicId)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                if (DbContext.TopicUsers.Any(tu => (tu.TopicId == topicId && tu.UserId == user.Id && tu.Role == Role.Reviewer)))
                    return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }
    }
}
