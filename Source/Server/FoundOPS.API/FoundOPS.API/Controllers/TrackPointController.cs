using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ResourceWithLastPoint = FoundOPS.API.Models.ResourceWithLastPoint;
using TrackPoint = FoundOPS.API.Models.TrackPoint;

namespace FoundOPS.API.Controllers
{
#if !DEBUG 
    [Authorize]
#endif
    public class TrackPointController : ApiController
    {
        public struct UpdateConstants
        {
            /// <summary>
            /// The minimum amount of seconds difference between TrackPoints' TimeStamps before storing them in Azure.
            /// </summary>
            public static readonly int SecondsBetweenHistoricalTrackPoints = 30;
        }

        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        #region GET

        // GET /api/trackpoint/GetTrackPoints?roleId={Guid}&date=Datetime
        /// <summary>
        /// Used to pull all TrackPoints that are assigned to a Route on a specific date
        /// </summary>
        /// <param name="roleId">Used to find the Business Account</param>
        /// <param name="routeId">The Id of the Route to pull Track Points for</param>
        /// <param name="serviceDate">The date of the Route to look for</param>
        /// <returns>A Queryable list of TrackPoints</returns>
        public IQueryable<TrackPoint> GetTrackPoints(Guid roleId, Guid routeId, DateTime serviceDate)
        {
            //Get the storage account from Azure
            var storageAccount = CloudStorageAccount.Parse(AzureHelpers.StorageConnectionString);

            //Setup the service context for the TrackPointsHistory tables
            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole(roleId);
            if (currentBusinessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = currentBusinessAccount.Id.TrackPointTableName();

            var trackPointsDate = serviceDate.Date;

            //Gets all objects from the Azure table specified on the date requested and returns the result
            var trackPoints = serviceContext.CreateQuery<TrackPointsHistoryTableDataModel>(tableName)
                .Where(tp => tp.CollectedDate == trackPointsDate && tp.RouteId == routeId)
                //The following allows us to get more than 1000 rows at a time
                .AsTableServiceQuery().Execute().ToArray();

            //Return the list of converted track points as a queryable
            var modelTrackPoints = trackPoints.Select(TrackPoint.ConvertToModel);

            return modelTrackPoints.OrderBy(tp => tp.CollectedTimeStamp).AsQueryable();
        }

        //GET /api/trackpoint/GetResourcesWithLatestPoints?roleId={Guid}&date=Datetime
        /// <summary>
        /// Gets all resources for a BusinessAccount and their last recorded location today.
        /// </summary>
        /// <param name="roleId">Used to find the Business Account</param>
        /// <returns>A list of Resource (employees or vehicles) with their latest tracked point</returns>
        public IQueryable<ResourceWithLastPoint> GetResourcesWithLatestPoints(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole(roleId);

#if DEBUG
            //Setup the design data
            //Makes it seem like each resource being tracked on a Route is moving
            SetupDesignDataForGetResourcesWithLatestPoints(currentBusinessAccount.Id);
#endif

            //Calls the sql function to pull all the necessary information for a ResourcesWithTrackPoint
            var resourcesWithTrackPoint = _coreEntitiesContainer.GetResourcesWithLastPoint(currentBusinessAccount.Id);

            var modelResources = resourcesWithTrackPoint.Select(ResourceWithLastPoint.ConvertToModel);
            return modelResources.AsQueryable();
        }

#if DEBUG

        /// <summary>
        /// Sets up the design data for GetResourcesWithLatestPoints. Makes it seem like the resource is actually moving
        /// </summary>
        /// <param name="currentBusinessAccountId">The business account to update the design data on.</param>
        private void SetupDesignDataForGetResourcesWithLatestPoints(Guid currentBusinessAccountId)
        {
            var serviceDate = DateTime.UtcNow.Date;
            var routes = _coreEntitiesContainer.Routes.Where(r => r.Date == serviceDate && r.OwnerBusinessAccountId == currentBusinessAccountId).OrderBy(r => r.Id);
            var numberOfRoutes = routes.Count();

            var count = 1;

            var random = new Random();
            foreach (var route in routes)
            {
                var routeNumber = count % numberOfRoutes;

                #region Adjust Technician Location, Speed and Heading

                foreach (var employee in route.Employees)
                {
                    employee.LastCompassDirection = (employee.LastCompassDirection + 15) % 360;
                    employee.LastTimeStamp = DateTime.UtcNow;
                    employee.LastSpeed = random.Next(30, 50);

                    switch (routeNumber)
                    {
                        //This would be the 4th, 8th, etc
                        case 0:
                            employee.LastLatitude = employee.LastLatitude + .005;
                            employee.LastLongitude = employee.LastLongitude + .007;
                            break;
                        //This would be the 1st, 5th, etc
                        case 1:
                            employee.LastLatitude = employee.LastLatitude - .005;
                            employee.LastLongitude = employee.LastLongitude + .003;
                            break;
                        //This would be the 2nd, 6th, etc
                        case 2:
                            employee.LastLatitude = employee.LastLatitude + .002;
                            employee.LastLongitude = employee.LastLongitude - .005;
                            break;
                        //This would be the 3rd, 7th, etc
                        case 3:
                            employee.LastLatitude = employee.LastLatitude - .006;
                            employee.LastLongitude = employee.LastLongitude - .005;
                            break;
                    }

                    employee.LastSource = random.Next(0, 1) == 0 ? "iPhone" : "Android";
                }

                #endregion

                count++;
            }

            _coreEntitiesContainer.SaveChanges();
        }

#endif

        #endregion

        #region POST

        /// <summary>
        /// Stores employee trackpoints.
        /// Used by the mobile phone application.
        /// </summary>
        /// <param name="modelTrackPoints">The list of TrackPoints being passed from an Employee's device</param>
        /// <param name="routeId">The Id of the Route that the Employee is currently on</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage PostEmployeeTrackPoint(TrackPoint[] modelTrackPoints, Guid routeId)
        {
            var currentUserAccount = AuthenticationLogic.CurrentUserAccount(_coreEntitiesContainer);

            //Return an Unauthorized Status Code
            //a) if there is no UserAccount
            //b) the user does not have access to the business account of the route
            if (currentUserAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, modelTrackPoints);

            //Provides an HTTP response back to the Mobile Phone to signal either a successful or unseccessful POST
            return PostTrackPointHelper(currentUserAccount.Id, null, modelTrackPoints, routeId);
        }

        //NOTE: this is not yet implemented because we do not have a way to track vehicles as of 4/11/2012
        //The code below will work assuming you are passing a valid VehicleId
        ///// <summary>
        ///// Used to save a TrackPoint from a Vehicle that has some tracking hardware
        ///// </summary>
        ///// <param name="trackedVehicleId">The Id of the vehicle currently being tracked</param>
        ///// <param name="modelTrackPoints">The list of TrackPoints being passed from an Employee's device</param> 
        ///// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        //public HttpResponseMessage<TrackPoint[]> PostVehicleTrackPoint(Guid trackedVehicleId, Models.TrackPoint[] modelTrackPoints)
        //{
        // 
        //    return new HttpResponseMessage<TrackPoint[]>(modelTrackPoints, HttpStatusCode.NotImplemented);
        //if (trackedVehicle == null)
        //    return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.Unauthorized);
        //var response = PostTrackPointHelper(null, trackedVehicleId, modelTrackPoint);
        //return response;
        //}

        #region Helpers

        /// <summary>
        /// Checks for an existing TrackPoint table for the BusinessAccount.
        /// If there is not one, it will create a new one.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="businessAccount">The business account.</param>
        /// <returns></returns>
        private string CheckCreateTrackPointTable(CloudStorageAccount storageAccount, BusinessAccount businessAccount)
        {
            //Create the Tables Client -> Used to Check if the table exists and to create it if it doesnt already exist
            var tableClient = storageAccount.CreateCloudTableClient();

            var tableName = businessAccount.Id.TrackPointTableName();

            //If an exception occurs, it means that the table already exists
            try
            {
                tableClient.CreateTableIfNotExist(tableName);
            }
            catch { }

            return tableName;
        }

        /// <summary>
        /// Pushes the validated TrackPoints to azure.
        /// The TrackPoints are validated by their difference in TimeStamps.
        /// It will only send TimeStamps seperated by TimeBetweenPushesToAzure seconds
        /// </summary>
        /// <param name="lastPushToAzureTimeStamp">The time that the last TrackPoint was pushed to Azure for either the Vehicle or the Employee</param>
        /// <param name="currentBusinessAccount">The current BusinessAccount</param>
        /// <param name="orderedModelTrackPoints">The list of TrackPoints being pulled, ordered by time</param>
        /// <param name="employee">The (optional) employee being tracked (null if you are posting for a vehicle)</param>
        /// <param name="vehicle">The (optional) vehicle being tracked (null if you are posting for an employee)</param>
        /// <param name="routeId">The Id of the Route that you are posting on</param>
        private void CheckTimeStampPushToAzure(DateTime? lastPushToAzureTimeStamp, BusinessAccount currentBusinessAccount, IEnumerable<TrackPoint> orderedModelTrackPoints,
            Employee employee, Vehicle vehicle, Guid routeId)
        {
            //Push TrackPoints to Azure as long as:
            //the vehicle or employee's LastPushToAzureTimeStamp is null
            //or trackpoint's timestamp is more than 30 seconds passed the last one passed to AzureTables
            foreach (var trackPoint in orderedModelTrackPoints)
            {
                if (lastPushToAzureTimeStamp.HasValue && (trackPoint.CollectedTimeStamp - lastPushToAzureTimeStamp) < TimeSpan.FromSeconds(UpdateConstants.SecondsBetweenHistoricalTrackPoints))
                    continue;

                PushTrackPointToAzure(currentBusinessAccount, trackPoint, employee, vehicle, routeId);

                lastPushToAzureTimeStamp = trackPoint.CollectedTimeStamp;
            }
        }

        /// <summary>
        /// Used to save TrackPoints from a mobile device
        /// </summary>
        /// <param name="employeeId">The employeeId (Will be null if you are posting from vehicle tracking hardware)</param>
        /// <param name="vehicleId">The vehicleId (Will be null if you are posting from a Mobile Phone)</param>
        /// <param name="modelTrackPoints">The list of TrackPoints being passed from either an Employees device or a device on a Vehicle</param>
        /// <param name="routeId">The Id of the Route that the vehicle or employee are currently on</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created or that an error occurred</returns>
        private HttpResponseMessage PostTrackPointHelper(Guid? employeeId, Guid? vehicleId, TrackPoint[] modelTrackPoints, Guid routeId)
        {
            //Take the list of TrackPoints passed and order them by their TimeStamps
            var orderedModelTrackPoints = modelTrackPoints.OrderBy(tp => tp.CollectedTimeStamp).ToArray();

            //The last TrackPoint in the list above is the most current and therefore will be stored in the SQL database
            var lastTrackPoint = orderedModelTrackPoints.Last();

            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountsQueryable().FirstOrDefault(ba => ba.Routes.Any(r => r.Id == routeId));

            //if the current business account for the route is not found, the user is not authorized for that business account (or the route does not exist)
            if(currentBusinessAccount == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (employeeId != null)
            {
                var employee = _coreEntitiesContainer.Employees.FirstOrDefault(
                        e => e.EmployerId == currentBusinessAccount.Id && e.LinkedUserAccount.Id == employeeId);

                if (employee == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                #region Save the last TrackPoint passed to the database

                employee.LastCompassDirection = lastTrackPoint.Heading;
                employee.LastLatitude = (double?) lastTrackPoint.Latitude;
                employee.LastLongitude = (double?) lastTrackPoint.Longitude;
                employee.LastSource = lastTrackPoint.Source;
                employee.LastSpeed = (double?) lastTrackPoint.Speed;
                employee.LastTimeStamp = lastTrackPoint.CollectedTimeStamp;

                #endregion

                //Checks each TrackPoint to see if it needs to be pushed to the Azure Tables
                //Pushes if needed
                CheckTimeStampPushToAzure(employee.LastPushToAzureTimeStamp, currentBusinessAccount, orderedModelTrackPoints, employee, null, routeId);
            }

            else if (vehicleId != null)
            {
                var vehicle = _coreEntitiesContainer.Vehicles.FirstOrDefault(v => v.OwnerPartyId == currentBusinessAccount.Id && v.Id == vehicleId);

                //If there is no Vehicle that corresponds to the vehicleId passed, something went wrong => return a BadRequest status code
                if (vehicle == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                #region Save the last TrackPoint passed to the database

                vehicle.LastCompassDirection = lastTrackPoint.Heading;
                vehicle.LastLatitude = (double?) lastTrackPoint.Latitude;
                vehicle.LastLongitude = (double?) lastTrackPoint.Longitude;
                vehicle.LastSource = lastTrackPoint.Source;
                vehicle.LastSpeed = (double?) lastTrackPoint.Speed;
                vehicle.LastTimeStamp = lastTrackPoint.CollectedTimeStamp;

                #endregion
                
                //Checks each TrackPoint to see if it needs to be pushed to the Azure Tables
                //Pushes if needed
                CheckTimeStampPushToAzure(vehicle.LastPushToAzureTimeStamp, currentBusinessAccount, orderedModelTrackPoints, null, vehicle, routeId);
            }

            //Find the one that is not null and keep it to be returned in the response header
            var employeeOrVehicleId = employeeId ?? vehicleId;

            //Save all the changes that have been made in the Database
            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = Request.CreateResponse(HttpStatusCode.Created, modelTrackPoints);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + employeeOrVehicleId.ToString());

            //Return the response
            return response;
        }

        /// <summary>
        /// Takes a ModelTrackPoint and pushed it to the AzureTables
        /// </summary>
        /// <param name="currentBusinessAccount">The current BusinessAccount</param>
        /// <param name="trackPoint">The Model TrackPoint to be pushed to AzureTables</param>
        /// <param name="employee">The employee</param>
        /// <param name="vehicle">The vehicle</param>
        /// <param name="routeId">The Id of the Route that the vehicle or employee are currently on</param>
        private void PushTrackPointToAzure(BusinessAccount currentBusinessAccount, TrackPoint trackPoint, Employee employee, Vehicle vehicle, Guid routeId)
        {
            //Get the storage account information from Azure
            var storageAccount = CloudStorageAccount.Parse(AzureHelpers.StorageConnectionString);

            //Create the Service Context for the TrackPointsHistory Tables
            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            var tableName = CheckCreateTrackPointTable(storageAccount, currentBusinessAccount);

            #region Set employeeId, vehicleId and appropriate LastPushToAzureTimeStamp

            Guid? employeeId = Guid.NewGuid();
            Guid? vehicleId = Guid.NewGuid();

            if (employee != null)
            {
                employeeId = employee.Id;
                vehicleId = null;
                employee.LastPushToAzureTimeStamp = trackPoint.CollectedTimeStamp;
            }

            if (vehicle != null)
            {
                employeeId = null;
                vehicleId = vehicle.Id;
                vehicle.LastPushToAzureTimeStamp = trackPoint.CollectedTimeStamp;
            }

            #endregion

            //Create the object to be stored in the Azure table
            var newTrackPoint = new TrackPointsHistoryTableDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            {
                EmployeeId = employeeId,
                VehicleId = vehicleId,
                RouteId = routeId,
                Latitude = (double?)trackPoint.Latitude,
                Longitude = (double?)trackPoint.Longitude,
                CollectedTimeStamp = trackPoint.CollectedTimeStamp
            };

            //Push to Azure Table
            serviceContext.AddObject(tableName, newTrackPoint);

            //Saves the Tables
            serviceContext.SaveChangesWithRetries();
        }

        #endregion

        #endregion
    }
}
