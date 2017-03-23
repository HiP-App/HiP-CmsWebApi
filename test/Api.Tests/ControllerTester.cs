﻿using Api.Models.Entity;
using MyTested.AspNetCore.Mvc;
using MyTested.AspNetCore.Mvc.Builders.Contracts.Controllers;

namespace Api.Tests
{
    public class ControllerTester<T>
        where T: class
    {
        private readonly User _admin;
        private readonly User _student;
        private readonly User _supervisor;

        public ControllerTester()
        {
            _admin = new User
            {
                Id = 1,
                Email = "admin@hipapp.de",
                Role = "Administrator"
            };
            _student = new User
            {
                Id = 2,
                Email = "student@hipapp.de",
                Role = "Student"
            };
            _supervisor = new User
            {
                Id = 3,
                Email = "supervisor@hipapp.de",
                Role = "Supervisor"
            };
        }

        /// <summary>
        /// Use this for bootstrapping your tests.
        /// Adds an admin, student and supervisor user to the database (with IDs 1, 2 and 3 respectively).
        /// </summary>
        /// <param name="userId">The user as whom you want to make the call. Defaults to 1=admin.</param>
        /// <returns>An instance of IAndControllerBuilder, i.e. you can chain MyTested test method calls to the return value.</returns>
        public IAndControllerBuilder<T> TestController(string userId = "1")
        {
            return MyMvc
                .Controller<T>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", userId))
                .WithDbContext(dbContext => dbContext
                    .WithSet<User>(db => db.AddRange(_admin, _student, _supervisor))
                );
        }
    }
}
