using PaderbornUniversity.SILab.Hip.CmsApi.Controllers;
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
