using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiteServer.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Net;

namespace ServerTests
{
    [TestClass]
    public class LoggingTests
    {
        [TestMethod]
        public void TestLoggingSuccessAsync()
        {
            
            IActionResult result = CreateTestHomeController().Log("testString",JsonConvert.SerializeObject(new { name = "TestObject"}));
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsTrue((result as JsonResult).StatusCode == (int)HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestLoggingFailure()
        {
            
            IActionResult result = CreateTestHomeController().Log("testString", "nonsenseJson");
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsTrue((result as JsonResult).StatusCode == (int)HttpStatusCode.BadRequest);
        }

        private HomeController CreateTestHomeController()
        {
            var response = new Mock<HttpResponse>();
            response.SetupAllProperties();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(a => a.Response).Returns(response.Object);
            HomeController hc = new HomeController();
            hc.ControllerContext.HttpContext = httpContext.Object;
            return hc;
        }
    }
}
