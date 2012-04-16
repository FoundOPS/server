using System.Linq;
using FoundOPS.API.Controllers;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using TrackPoint = FoundOPS.API.Models.TrackPoint;

namespace API.Tests.Controllers
{
    [TestClass]
    public class TrackPointControllerTest
    {
        [TestMethod]
        public void PostTrackPoint()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var trackPoint = new TrackPoint
            {
                Id = Guid.NewGuid(),
                CompassDirection = 0,
                Latitude = 40.45997,
                Longitude = -86.93089,
                TimeStamp = DateTime.Now,
                Speed = 50.23,
            };

            var currentUser = AuthenticationLogic.CurrentUserAccount(coreEntitiesContainer);
            var fakeRouteId = Guid.NewGuid();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController { Request = request };

            var response = controller.PostEmployeeTrackPoint(trackPoint, currentUser.Id, fakeRouteId);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(response.Headers.Location);
            var postedTrackPoint = response.Content.ReadAsync().Result;
        }

        [TestMethod]
        public void GetTrackPoints()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var currentUser = AuthenticationLogic.CurrentUserAccount(coreEntitiesContainer);

            var roleId = currentUser.OwnedRoles.FirstOrDefault().Id;

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController { Request = request };

            var date = new DateTime(2012, 4, 16);

            var response = controller.GetTrackPoints(new Guid("F7349D7F-77AF-4AED-8425-5AF979AC74B9"), date);
        }
    }
}
