using System.Collections.Generic;
using System.Security.Claims;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity.Annotation;
using MyTested.AspNetCore.Mvc;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Tests.ControllerTests
{
    public class DownloadControllerTest
    {
        private ControllerTester<DownloadController> _tester;        

        public DownloadControllerTest()
        {
            _tester = new ControllerTester<DownloadController>();            
        }

        /// <summary>
        /// Should return ok when admin tries to download the file
        /// </summary>
        [Fact]
        public void GetTest404()
        {
            var downloadHash = "abc";
            _tester.TestController()
                .Calling(c => c.Get(downloadHash))
                .ShouldReturn()
                .NotFound();
        }
    }
}
