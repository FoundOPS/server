using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using TrackPoint = FoundOPS.API.Models.TrackPoint;

namespace FoundOPS.API.Controllers
{
    [Authorize]
    public class TrackPointController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region POST

        /// <summary>
        /// Used to save a TrackPoint from a mobile phone being used by an Employee
        /// </summary>
        /// <param name="modelTrackPoint"></param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage<Models.TrackPoint> PostEmployeeTrackPoint(Models.TrackPoint modelTrackPoint)
        {
            var currentUserAccount = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).FirstOrDefault();

            if(currentUserAccount == null)
                return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.Unauthorized);

            var response = PostTrackPointHelper(currentUserAccount.Id, null, modelTrackPoint);

            return response;
        }

        /// <summary>
        /// Used to save a TrackPoint from a Vehicle that has some tracking hardware
        /// </summary>
        /// <param name="trackedVehicleId"></param>
        /// <param name="modelTrackPoint"></param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage<Models.TrackPoint> PostVehicleTrackPoint(Guid trackedVehicleId, Models.TrackPoint modelTrackPoint)
        {
            //Note: this is not yet implemented because we do not have a way to track vehicles as of 4/11/2012
            //The code below will work assuming you are passing a valid VehicleId
            return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.NotImplemented);
            
            //if (trackedVehicle == null)
            //    return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.Unauthorized);

            //var response = PostTrackPointHelper(null, trackedVehicleId, modelTrackPoint);

            //return response;
        }

        /// <summary>
        /// Used to save TrackPoints from a mobile device
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="modelTrackPoint"></param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        private HttpResponseMessage<Models.TrackPoint> PostTrackPointHelper(Guid? employeeId, Guid? vehicleId, Models.TrackPoint modelTrackPoint)
        {
            //Convert the model TrackPoint to database version
            var trackPoint = Models.TrackPoint.ConvertFromModel(modelTrackPoint);
            
            //Add in both the VehicleId and EmployeeId. One of the should be null
            trackPoint.EmployeeId = employeeId;
            trackPoint.VehicleId = vehicleId;

            //Find the one that is not null and keep it to be returned in the response header
            var employeeOrVehicleId = employeeId ?? vehicleId;

            //Add the new TrackPoint to the DB and save
            _coreEntitiesContainer.TrackPoints.AddObject(trackPoint);
            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = new HttpResponseMessage<Models.TrackPoint>(modelTrackPoint, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + employeeOrVehicleId.ToString() + modelTrackPoint.Id.ToString());

            return response;
        }

        #endregion
    }
}
