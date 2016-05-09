using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app
{
    public static class Constants
    {
        // The below class is for defining constants for Roles.
        public static class Roles
        {
            private static readonly string[] listOfRoles = { "Admin", "Supervisor", "Student" };

            public static string Admin { get { return listOfRoles[0]; } }
            public static string Supervisor { get { return listOfRoles[1]; } }
            public static string Student { get { return listOfRoles[2]; } }

            // Get All Roles in a Single Variable.
            public static IEnumerable<string> AllRoles { get { return listOfRoles; } }
        }
    }
}
