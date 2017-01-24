using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Models;
using Api.Models.Entity;
using MyTested.AspNetCore.Mvc;
using NUnit.Framework;

namespace Api.Tests.ControllerTests
{
    [TestFixture]
    public class AnnotationTagRelationsControllerTest
    {

        #region GetRelations

        /// <summary>
        /// Should return code 200 and a list of all tag relations if called properly
        /// </summary>
        [Test]
        public void GetRelationsTest()
        {
            var relation = new TagRelation(); // TODO: Create a meaningful relation here and pass it into the mocked database
            var expected = new List<TagRelation>() { relation };
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetRelations(0))
                .ShouldReturn()
                .Ok()
                .WithModelOfType<List<TagRelation>>()
                .Passing(actual => expected.SequenceEqual(actual));
        }

        /// <summary>
        /// Should return 400 for negative maxDepth values
        /// </summary>
        [Test]
        public void GetRelationsTest400()
        {
            MyMvc
                .Controller<AnnotationController>()
                .WithAuthenticatedUser(user => user.WithClaim("Id", "1"))
                .Calling(c => c.GetRelations(-1))
                .ShouldReturn()
                .BadRequest();
        }

        #endregion
    }
}
