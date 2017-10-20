﻿using System;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.User
{
    public class UserResult
    {
        /// <summary>
        /// The user ID (not the internally used integer-ID).
        /// </summary>
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; }

        /// <summary>
        /// URL from which the profile picture or a thumbnail of it can be obtained.
        /// </summary>
        public string ProfilePicture { get; set; }
    }

    [Obsolete("Use UserResult instead", error: true)]
    public class UserResultLegacy
    {
        public UserResultLegacy(Entity.User user)
        {
            Identity = user.UId;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
            if (user.StudentDetails != null)
            {
                StudentDetails = new StudentDetailsResult()
                {
                    Discipline = user.StudentDetails.Discipline,
                    CurrentDegree = user.StudentDetails.CurrentDegree,
                    CurrentSemester = user.StudentDetails.CurrentSemester
                };
            }
        }

        public string Identity { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public StudentDetailsResult StudentDetails { get; set; }

        public string FullName
        {
            get { return FirstName + ' ' + LastName; }
        }
    }

    public class StudentDetailsResult
    {
        public string Discipline { get; set; }

        public string CurrentDegree { get; set; }

        public short CurrentSemester { get; set; }
    }
}
