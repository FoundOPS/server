using System;
using FoundOPS.API.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers
{
    [TestClass]
    public class ServiceControllerTest
    {
        [TestMethod]
        public void GetLatestTrackPoints()
        {
            //var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new ServiceController();

            var response = controller.GetTaskDetails(new Guid("3524C015-B95E-476A-A37A-6DA81DDF1927"));
        }
    }
}
