﻿using Api.Data;
using Api.Managers;
using Api.Models;
using System;

namespace Api.Permission
{
    public class UserPermissions : BaseManager
    {
        private readonly UserManager _userManager;


        public UserPermissions(CmsDbContext dbContext) : base(dbContext)
        {
            _userManager = new UserManager(dbContext);
        }

        public bool IsAllowedToAdminister(string userIdenty)
        {
            try
            {
                var user = _userManager.GetUserByIdenty(userIdenty);
                return user.Role.Equals(Role.Administrator);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public bool IsAllowedToInvite(string userIdenty)
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
    }
}
