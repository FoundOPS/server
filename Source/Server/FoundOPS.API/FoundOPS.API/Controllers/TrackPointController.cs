using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOPS.API.Controllers
{
    [Authorize]
    public class TrackPointController : ApiController
    {
        #region POST

        public HttpResponseMessage<Models.TrackPoint> PostTrackPoint(Guid roleId, Models.TrackPoint modelTrackPoint)
        {
            //Convert the model TrackPoint to database version
            var trackPoint = Models.TrackPoint.ConvertFromModel(modelTrackPoint);

            trackPoint.EmployeeId = roleId;

            var coreEntitiesContainer = new CoreEntitiesContainer();

            coreEntitiesContainer.TrackPoints.AddObject(trackPoint);
            coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = new HttpResponseMessage<Models.TrackPoint>(modelTrackPoint, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + roleId.ToString() + modelTrackPoint.Id.ToString());

            return response;
        }

        #endregion
    }
}
