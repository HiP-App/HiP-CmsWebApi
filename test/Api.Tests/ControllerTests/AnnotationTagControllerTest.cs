using System.Collections.Generic;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using Xunit;
using PaderbornUniversity.SILab.Hip.CmsApi.Tests.Utility;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class AnnotationTagControllerTest
    {
        #region GetLayers

        /// <summary>
        /// Should return code 200 and a list of all tag layers if called properly
        /// </summary>
        [Fact]
        public void GetLayersTest()
        {
            var layer = new Layer() { Name = "Time" };
            var expected = new HashSet<Layer>() { layer };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim(CustomClaims.Sub, "admin@hipapp.de"))
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
