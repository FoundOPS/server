using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using FoundOPS.API.Controllers;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new TrackPointController {Request = request};
            //This will no longer work, Need to pass it a real role id and not a new Giuid
            var response = controller.PostEmployeeTrackPoint(trackPoint, Guid.NewGuid());
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(response.Headers.Location);
            TrackPoint postedTrackPoint = response.Content.ReadAsync().Result;
            var trackPointExists = coreEntitiesContainer.TrackPoints.FirstOrDefault(tp => tp.Id == postedTrackPoint.Id) != null;

            Assert.AreEqual(true, trackPointExists);
        }
    }
}
