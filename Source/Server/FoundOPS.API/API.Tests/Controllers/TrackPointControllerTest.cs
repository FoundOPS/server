using System.Collections.Generic;
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
            var trackPoints = new List<TrackPoint>();

            //Create the test TrackPoints with random numbers as properties
            for (int i = 0; i < 10; i++)
            {
                var random = new Random();

                var trackPoint = new TrackPoint
                {
                    Id = Guid.NewGuid(),
                    CompassDirection = random.Next(0, 359),
                    Latitude = random.Next(-90, 90),
                    Longitude = random.Next(-180, 180),
                    LastTimeStamp = DateTime.Now.AddSeconds(10 * i),
                    Speed = random.Next(20, 60),
                };

                trackPoints.Add(trackPoint);
            }

            var fakeRouteId = Guid.NewGuid();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController { Request = request };

            var response = controller.PostEmployeeTrackPoint(trackPoints.ToArray(), fakeRouteId);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var postedTrackPoint = response.Content.ReadAsync().Result;
        }

        [TestMethod]
        public void GetTrackPoints()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var currentUser = AuthenticationLogic.CurrentUserAccount(coreEntitiesContainer);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController { Request = request };

            var date = new DateTime(2012, 4, 16);

            //var response = controller.GetTrackPoints(new Guid("F7349D7F-77AF-4AED-8425-5AF979AC74B9"), date);
        }

        [TestMethod]
        public void GetLatestTrackPoints()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController { Request = request };

            var date = new DateTime(2012, 4, 18);

            var response = controller.GetResourcesWithLatestPoints(new Guid("6E0B4C05-7089-44B7-A9AA-88BF83CEEC12"), date);
        }
    }
}
