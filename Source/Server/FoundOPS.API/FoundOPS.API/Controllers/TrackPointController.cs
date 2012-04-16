using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TrackPoint = FoundOPS.API.Models.TrackPoint;

namespace FoundOPS.API.Controllers
{
    [Authorize]
    public class TrackPointController : ApiController
    {
        public struct UpdateConstants
        {
            public static readonly int TimeBetweenPushesToAzure = 30;

        #if DEBUG
            public const string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=opsappdebug;AccountKey=Wbs5xOmAKdNw8ef9XgRZF2lhE+DYH1uN0qgETVKSCLqIXaaTRjiFIj4sT2cf0iQxUdOAYEej2VI4aPBr7TVOYA==";
        #else
            public const string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=fstoreroledata;AccountKey=bs7B22OoZr0jKz9xCJFlNacCemDvaTT8pj3yV2PENA6GwYkERELymg+hDOU2Yz+nAkU8IyvS4lDUmzkfkQsCuQ==";
        #endif
        }

        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region Get

        // GET /api/trackpoints
        public IQueryable<TrackPoint> GetTrackPoints(Guid roleId, DateTime date)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole(roleId);

            if (currentBusinessAccount == null)
                return null;

            //Check that this DataConnectionString is right
            var storageAccount = CloudStorageAccount.Parse(UpdateConstants.StorageConnectionString);

            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "tp" + currentBusinessAccount.Id.ToString().Replace("-", "");

            //Gets all objects from the Azure table specified on the date requested and returns the result
            var trackPoints = serviceContext.CreateQuery<TrackPointsHistoryTableDataModel>(tableName).ToArray().Where(tp => tp.TimeStamp.Date == date.Date);

            //Return the list of converted track points as a queryable
            var modelTrackPoints = trackPoints.Select(TrackPoint.ConvertToModel);

            return modelTrackPoints.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="serviceDate">The requested ServiceDate</param>
        /// <returns>A list of Resource (employees or vehicles) with their latest tracked point</returns>
        public IQueryable<ResourceWithLastPoint> GetResourcesWithLatestPoints(Guid roleId, DateTime serviceDate)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole(roleId);

            if (currentBusinessAccount == null)
                return null;

            var resourcesWithTrackPoint = _coreEntitiesContainer.GetResourcesWithLastPoint(currentBusinessAccount.Id, serviceDate);

            return resourcesWithTrackPoint.AsQueryable();
        }

        #endregion

        #region POST

        /// <summary>
        /// Used to save a TrackPoint from a mobile phone being used by an Employee
        /// </summary>
        /// <param name="modelTrackPoint"></param>
        /// <param name="roleId"> </param>
        /// <param name="routeId"> </param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage<Models.TrackPoint> PostEmployeeTrackPoint(Models.TrackPoint modelTrackPoint, Guid roleId, Guid routeId)
        {
            var currentUserAccount = AuthenticationLogic.CurrentUserAccount(_coreEntitiesContainer);

            if (currentUserAccount == null)
                return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.Unauthorized);

            var response = PostTrackPointHelper(roleId, currentUserAccount.Id, null, modelTrackPoint, routeId);

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
        /// <param name="roleId"> </param>
        /// <param name="employeeId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="modelTrackPoint"></param>
        /// <param name="routeId"> </param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        private HttpResponseMessage<Models.TrackPoint> PostTrackPointHelper(Guid roleId, Guid? employeeId, Guid? vehicleId, Models.TrackPoint modelTrackPoint, Guid routeId)
        {
            var pushTrackPointToAzure = false;

            var currentBusinessAccount = new BusinessAccount();

            if (employeeId != null)
            {
                var employee = _coreEntitiesContainer.Employees.FirstOrDefault(e => e.LinkedUserAccount.Id == employeeId);

                if(employee == null) 
                    return new HttpResponseMessage<TrackPoint>(HttpStatusCode.BadRequest);

                employee.LastCompassDirection = modelTrackPoint.CompassDirection;
                employee.LastLatitude = modelTrackPoint.Latitude;
                employee.LastLongitude = modelTrackPoint.Longitude;
                employee.LastSource = modelTrackPoint.Source;
                employee.LastSpeed = modelTrackPoint.Speed;
                employee.LastTimeStamp = modelTrackPoint.TimeStamp;

                currentBusinessAccount = employee.Employer;

                if (modelTrackPoint.TimeStamp.AddSeconds(-(UpdateConstants.TimeBetweenPushesToAzure)) >= employee.LastPushToAzureTimeStamp)
                    pushTrackPointToAzure = true;
            }

            else if (vehicleId != null)
            {
                var vehicle = _coreEntitiesContainer.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

                if (vehicle == null)
                    return new HttpResponseMessage<TrackPoint>(HttpStatusCode.BadRequest);

                vehicle.LastCompassDirection = modelTrackPoint.CompassDirection;
                vehicle.LastLatitude = modelTrackPoint.Latitude;
                vehicle.LastLongitude = modelTrackPoint.Longitude;
                vehicle.LastSource = modelTrackPoint.Source;
                vehicle.LastSpeed = modelTrackPoint.Speed;
                vehicle.LastTimeStamp = modelTrackPoint.TimeStamp;

                currentBusinessAccount = vehicle.OwnerParty.ClientOwner.Vendor;

                if (modelTrackPoint.TimeStamp.AddSeconds(-(UpdateConstants.TimeBetweenPushesToAzure)) >= vehicle.LastPushToAzureTimeStamp)
                    pushTrackPointToAzure = true;
            }

            pushTrackPointToAzure = true;

            if (pushTrackPointToAzure)
            {
                var storageAccount = CloudStorageAccount.Parse(UpdateConstants.StorageConnectionString);

                var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(),
                                                                        storageAccount.Credentials);

                // Create the table if there is not already a table with the name of tp + EmployeeId/VehicleId
                var tableClient = storageAccount.CreateCloudTableClient();

                if (currentBusinessAccount == null)
                    return new HttpResponseMessage<TrackPoint>(HttpStatusCode.BadRequest);

                //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
                var tableName = "tp" + currentBusinessAccount.Id.ToString().Replace("-", "");

                try
                {
                    tableClient.CreateTableIfNotExist(tableName);
                }
                catch { }

                //Create the object to be stored in the Azure table
                var newTrackPoint = new TrackPointsHistoryTableDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                                        {
                                            EmployeetId = employeeId,
                                            VehicleId = vehicleId,
                                            RouteId = routeId,
                                            Latitude = modelTrackPoint.Latitude,
                                            Longitude = modelTrackPoint.Longitude,
                                            TimeStamp = modelTrackPoint.TimeStamp
                                        };

                //Push to Azure Table
                serviceContext.AddObject(tableName, newTrackPoint);

                //Saves the Tables
                serviceContext.SaveChangesWithRetries();
            }

            //Find the one that is not null and keep it to be returned in the response header
            var employeeOrVehicleId = employeeId ?? vehicleId;

            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = new HttpResponseMessage<Models.TrackPoint>(modelTrackPoint, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + employeeOrVehicleId.ToString() + modelTrackPoint.Id.ToString());

            return response;
        }

        #endregion
    }
}
