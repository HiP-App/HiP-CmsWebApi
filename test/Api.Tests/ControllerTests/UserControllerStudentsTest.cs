using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;
using System.Linq;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class UserControllerStudentsTest
    {
        private ControllerTester<UserController> _tester;
        public StudentFormModel StudentFormModel { get; set; }

        public UserControllerStudentsTest()
        {
            _tester = new ControllerTester<UserController>();
            StudentFormModel = new StudentFormModel
            {
                Discipline = "ABC",
                CurrentDegree = "XYZ",
                CurrentSemester = 1
            };
        }

        #region Put student

        /// <summary>
        /// Should return 200 when trying to update user details is successful
        /// </summary>
        [Fact]
        public void PutStudentTest200()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutStudent(StudentFormModel, _tester.Student.Email))
                .ShouldHave()
                .DbContext(db => db.WithSet<StudentDetails>(sd =>
                    sd.Single(actual => actual.UserId == _tester.Student.Id)
                        .CurrentDegree == StudentFormModel.CurrentDegree))
                .AndAlso()
                .ShouldReturn()
                .Ok();
        }

        /// <summary>
        /// Should return 400 when model is incorrect
        /// </summary>
        [Fact]
        public void PutStudentTest400()
        {
            var model = new StudentFormModel();

            _tester.TestControllerWithMockData()
                .Calling(c => c.PutStudent(model, _tester.Student.Email))
                .ShouldReturn()
                .BadRequest();
        }

        /// <summary>
        /// Should return 403 when the user doesn't have permission to update the profile picture
        /// </summary>
        [Fact]
        public void PutStudentTest403()
        {
            _tester.TestControllerWithMockData("newuser@hipapp.de", "Student")
                .Calling(c => c.PutStudent(StudentFormModel, _tester.Student.Email))
                .ShouldReturn()
                .StatusCode(403);
        }

        /// <summary>
        /// Should return 404 when student does not exist
        /// </summary>
        [Fact]
        public void PutStudentTest404()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutStudent(StudentFormModel, "newstudent@hipapp.de"))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Should return 404 when the user is not a student
        /// </summary>
        [Fact]
        public void PutStudentTest404WhenHeIsNotAStudent()
        {
            _tester.TestControllerWithMockData()
                .Calling(c => c.PutStudent(StudentFormModel, _tester.Supervisor.Email))
                .ShouldReturn()
                .NotFound();
        }

        #endregion
    }
}
