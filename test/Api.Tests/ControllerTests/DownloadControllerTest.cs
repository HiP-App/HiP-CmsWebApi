using MyTested.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
using System.Net;
using Xunit;
using static PaderbornUniversity.SILab.Hip.CmsApi.Managers.DownloadManager;

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
        /// Should return 200 for download hashes
        /// </summary>
        [Fact]
        public void GetTest200()
        {
            var ipAddress = IPAddress.Parse("192.168.1.1");
            var downloadResource = new DownloadResource("example.jpg", ipAddress);
            var downloadHash = "abc";
            _tester.TestController()
                .WithDbContext(dbContext => dbContext
                    .WithSet<DownloadResource>(db => db.Add(downloadResource)))
                .Calling(c => c.Get(downloadHash))
                .ShouldReturn()
                .NotFound();
        }

        /// <summary>
        /// Should return 404 for invalid download hashes
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
