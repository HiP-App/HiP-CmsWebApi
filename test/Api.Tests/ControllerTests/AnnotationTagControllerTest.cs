using System.Collections.Generic;
using Api.Controllers;
using Api.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using NUnit.Framework;

namespace Api.Tests.ControllerTests
{
    [TestFixture]
    public class AnnotationTagControllerTest
    {
        #region GetLayers

        /// <summary>
        /// Should return code 200 and a list of all tag layers if called properly
        /// </summary>
        [Test]
        public void GetLayersTest()
        {
            var layer = new Layer() { Name = "Time" };
            var expected = new HashSet<Layer>() { layer };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .WithDbContext(dbContext => dbContext.WithSet<Layer>(db => db.Add(layer)))
                .Calling(c => c.GetAllLayers())
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<Layer>>()
                .Passing(actual => expected.SetEquals(actual));
        }

        #endregion

    }
}
