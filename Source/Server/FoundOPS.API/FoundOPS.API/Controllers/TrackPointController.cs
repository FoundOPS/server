using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using FoundOPS.API.Models;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Route = FoundOps.Core.Models.CoreEntities.Route;
using TrackPoint = FoundOPS.API.Models.TrackPoint;

namespace FoundOPS.API.Controllers
{
    //[Authorize]
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

        #region GET

        // GET /api/trackpoint/GetTrackPoints?roleId={Guid}&date=Datetime
        public IQueryable<TrackPoint> GetTrackPoints(Guid? roleId, Guid? routeId, DateTime serviceDate)
        {
            if (roleId == null)
                return null;

#if DEBUG
            var currentBusinessAccount = _coreEntitiesContainer.Parties.OfType<BusinessAccount>().FirstOrDefault(ba => ba.Id == roleId);
#else
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole((Guid)roleId);
#endif

            if (currentBusinessAccount == null)
                return null;

            //Check that this DataConnectionString is right
            var storageAccount = CloudStorageAccount.Parse(UpdateConstants.StorageConnectionString);

            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "tp" + currentBusinessAccount.Id.ToString().Replace("-", "");

            var trackPointsDate = serviceDate.Date;

            //Gets all objects from the Azure table specified on the date requested and returns the result
            var trackPoints = serviceContext.CreateQuery<TrackPointsHistoryTableDataModel>(tableName).
                Where(tp => tp.RouteId == routeId).ToArray(); 

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
        //GET /api/trackpoint/GetResourcesWithLatestPoints?roleId={Guid}&date=Datetime
        public IQueryable<ModelResourceWithLastPoint> GetResourcesWithLatestPoints(Guid? roleId, DateTime? serviceDate)
        {
            if (!serviceDate.HasValue)
                serviceDate = DateTime.UtcNow.Date;

            if (roleId == null)
                return null;

#if DEBUG
            var currentBusinessAccount = _coreEntitiesContainer.Parties.OfType<BusinessAccount>().FirstOrDefault(ba => ba.Id == roleId);
#else
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole((Guid)roleId);
#endif

            if (currentBusinessAccount == null)
                return null;
#if DEBUG
            var routes = _coreEntitiesContainer.Routes.Where(r => r.Date == serviceDate && r.OwnerBusinessAccountId == currentBusinessAccount.Id).OrderBy(r => r.Id);

            SetupDesignDataForGetResourcesWithLatestPoints(routes);
#endif

            var resourcesWithTrackPoint = _coreEntitiesContainer.GetResourcesWithLastPoint(currentBusinessAccount.Id, serviceDate);

            var modelResources = resourcesWithTrackPoint.Select(ModelResourceWithLastPoint.ConvertToModel);

            return modelResources.AsQueryable();
        }

        /// <summary>
        /// Sets up the design data for GetResourcesWithLatestPoints. Makes it seem like the resource is actually moving
        /// </summary>
        /// <param name="routes">The Routes that have resources</param>
        private void SetupDesignDataForGetResourcesWithLatestPoints(IOrderedQueryable<Route> routes)
        {
            var numberOfRoutes = routes.Count();

            var count = 1;

            var random = new Random();

            foreach (var route in routes)
            {
                var routeNumber = count % numberOfRoutes;

                switch (routeNumber)
                {
                    //This would be the 4th, 8th, etc
                    case 0:
                        var employees = route.Technicians;
                        foreach (var employee in employees)
                        {
                            employee.LastLatitude = employee.LastLatitude + .00005;
                            employee.LastLongitude = employee.LastLongitude + .00005;

                            employee.LastCompassDirection += 15;
                            employee.LastTimeStamp = DateTime.UtcNow;
                            employee.LastSpeed = random.Next(30, 50);
                            employee.LastSource = "iPhone";
                        }
                        break;
                    //This would be the 1st, 5th, etc
                    case 1:
                        employees = route.Technicians;
                        foreach (var employee in employees)
                        {
                            employee.LastLatitude = employee.LastLatitude - .00005;
                            employee.LastLongitude = employee.LastLongitude + .00005;

                            employee.LastCompassDirection += 15;
                            employee.LastTimeStamp = DateTime.UtcNow;
                            employee.LastSpeed = random.Next(30, 50);
                            employee.LastSource = "Android";
                        }
                        break;
                    //This would be the 2nd, 6th, etc
                    case 2:
                        employees = route.Technicians;
                        foreach (var employee in employees)
                        {
                            employee.LastLatitude = employee.LastLatitude + .00005;
                            employee.LastLongitude = employee.LastLongitude - .00005;

                            employee.LastCompassDirection += 15;
                            employee.LastTimeStamp = DateTime.UtcNow;
                            employee.LastSpeed = random.Next(30, 50);
                            employee.LastSource = "iPhone";
                        }
                        break;
                    //This would be the 3rd, 7th, etc
                    case 3:
                        employees = route.Technicians;
                        foreach (var employee in employees)
                        {
                            employee.LastLatitude = employee.LastLatitude - .00005;
                            employee.LastLongitude = employee.LastLongitude - .00005;

                            employee.LastCompassDirection += 15;
                            employee.LastTimeStamp = DateTime.UtcNow;
                            employee.LastSpeed = random.Next(30, 50);
                            employee.LastSource = "Android";
                        }
                        break;

                }

                count++;
            }

            _coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region POST

        /// <summary>
        /// Used to save TrackPoints from a mobile phone being used by an Employee
        /// </summary>
        /// <param name="modelTrackPoints">The list of TrackPoints being passed from an Employee's device</param>
        /// <param name="routeId">The Id of the Route that the Employee is currently on</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage<TrackPoint[]> PostEmployeeTrackPoint(TrackPoint[] modelTrackPoints, Guid routeId)
        {
            var currentUserAccount = AuthenticationLogic.CurrentUserAccount(_coreEntitiesContainer);

            if (currentUserAccount == null)
                return new HttpResponseMessage<TrackPoint[]>(modelTrackPoints, HttpStatusCode.Unauthorized);

            var response = PostTrackPointHelper(currentUserAccount.Id, null, modelTrackPoints, routeId);

            return response;
        }

        /// <summary>
        /// Used to save a TrackPoint from a Vehicle that has some tracking hardware
        /// </summary>
        /// <param name="trackedVehicleId"></param>
        /// <param name="modelTrackPoints"></param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public HttpResponseMessage<Models.TrackPoint[]> PostVehicleTrackPoint(Guid trackedVehicleId, Models.TrackPoint[] modelTrackPoints)
        {
            //Note: this is not yet implemented because we do not have a way to track vehicles as of 4/11/2012
            //The code below will work assuming you are passing a valid VehicleId
            return new HttpResponseMessage<TrackPoint[]>(modelTrackPoints, HttpStatusCode.NotImplemented);

            //if (trackedVehicle == null)
            //    return new HttpResponseMessage<TrackPoint>(modelTrackPoint, HttpStatusCode.Unauthorized);

            //var response = PostTrackPointHelper(null, trackedVehicleId, modelTrackPoint);

            //return response;
        }

        /// <summary>
        /// Used to save TrackPoints from a mobile device
        /// </summary>
        /// <param name="employeeId">The employeeId</param>
        /// <param name="vehicleId">The vehicleId</param>
        /// <param name="modelTrackPoints">The list of TrackPoints being passed from either an Employees device or a device on a Vehicle</param>
        /// <param name="routeId">The Id of the Route that the vehicle or employee are currently on</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created or that an error occurred</returns>
        private HttpResponseMessage<TrackPoint[]> PostTrackPointHelper(Guid? employeeId, Guid? vehicleId, TrackPoint[] modelTrackPoints, Guid routeId)
        {
            //Take the list of TrackPoints passed and order them by their TimeStamps
            var orderedModelTrackPoints = modelTrackPoints.OrderBy(tp => tp.LastTimeStamp).ToArray(); 

            //The last TrackPoint in the list above is the most current and therefore will be stored in the SQL database
            var lastTrackPoint = orderedModelTrackPoints.Last();

            BusinessAccount currentBusinessAccount;

            if (employeeId != null)
            {
                var employee = _coreEntitiesContainer.Employees.FirstOrDefault(e => e.LinkedUserAccount.Id == employeeId);

                if (employee == null)
                    return new HttpResponseMessage<TrackPoint[]>(HttpStatusCode.BadRequest);

                //Save the last TrackPoint passed to the database
                employee.LastCompassDirection = lastTrackPoint.CompassDirection;
                employee.LastLatitude = lastTrackPoint.Latitude;
                employee.LastLongitude = lastTrackPoint.Longitude;
                employee.LastSource = lastTrackPoint.Source;
                employee.LastSpeed = lastTrackPoint.Speed;
                employee.LastTimeStamp = lastTrackPoint.LastTimeStamp;

                currentBusinessAccount = employee.Employer;

                //If there is no BusinessAccount something went wrong, return a BadRequest status code
                if (currentBusinessAccount == null)
                    return new HttpResponseMessage<TrackPoint[]>(HttpStatusCode.BadRequest);

                //Checks each TrackPoint to see if it needs to be pushed to the Azure Tables
                foreach (var trackPoint in orderedModelTrackPoints)
                {
                    //If trackpoint's timestamp is more than 30 seconds passed the last one passed to AzureTables
                    //Or the LastPushToAzureTimeStamp is null
                    if (trackPoint.LastTimeStamp.AddSeconds(-(UpdateConstants.TimeBetweenPushesToAzure)) >= employee.LastPushToAzureTimeStamp || employee.LastPushToAzureTimeStamp == null)
                        PushTrackPointToAzure(currentBusinessAccount, trackPoint, employee, null, routeId);
                }
            }

            else if (vehicleId != null)
            {
                var vehicle = _coreEntitiesContainer.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

                //If there is no Vehicle that corresponds to the vehicleId passed, something went wrong => return a BadRequest status code
                if (vehicle == null)
                    return new HttpResponseMessage<TrackPoint[]>(HttpStatusCode.BadRequest);

                //Save the last TrackPoint passed to the database
                vehicle.LastCompassDirection = lastTrackPoint.CompassDirection;
                vehicle.LastLatitude = lastTrackPoint.Latitude;
                vehicle.LastLongitude = lastTrackPoint.Longitude;
                vehicle.LastSource = lastTrackPoint.Source;
                vehicle.LastSpeed = lastTrackPoint.Speed;
                vehicle.LastTimeStamp = lastTrackPoint.LastTimeStamp;

                currentBusinessAccount = vehicle.OwnerParty.ClientOwner.Vendor;

                //If there is no BusinessAccount something went wrong, return a BadRequest status code
                if (currentBusinessAccount == null)
                    return new HttpResponseMessage<TrackPoint[]>(HttpStatusCode.BadRequest);

                //Checks each TrackPoint to see if it needs to be pushed to the Azure Tables
                foreach (var trackPoint in orderedModelTrackPoints)
                {
                    //If trackpoint's timestamp is more than 30 seconds passed the last one passed to AzureTables
                    //Or the LastPushToAzureTimeStamp is null
                    if (trackPoint.LastTimeStamp.AddSeconds(-(UpdateConstants.TimeBetweenPushesToAzure)) >= vehicle.LastPushToAzureTimeStamp || vehicle.LastPushToAzureTimeStamp == null)
                        PushTrackPointToAzure(currentBusinessAccount, trackPoint, null, vehicle, routeId);
                }
            }

            //Find the one that is not null and keep it to be returned in the response header
            var employeeOrVehicleId = employeeId ?? vehicleId;

            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = new HttpResponseMessage<TrackPoint[]>(modelTrackPoints, HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + employeeOrVehicleId.ToString());

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
            var storageAccount = CloudStorageAccount.Parse(UpdateConstants.StorageConnectionString);

            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            var tableClient = storageAccount.CreateCloudTableClient();

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "tp" + currentBusinessAccount.Id.ToString().Replace("-", "");

            //If an exception occurs, it means that the table already exists
            try
            {
                tableClient.CreateTableIfNotExist(tableName);
            }
            catch { }

            #region Set employeeId, vehicleId and appropriate LastPushToAzureTimeStamp

            Guid? employeeId = Guid.NewGuid();
            Guid? vehicleId = Guid.NewGuid();

            if (employee != null)
            {
                employeeId = employee.Id;
                vehicleId = null;
                employee.LastPushToAzureTimeStamp = trackPoint.LastTimeStamp;
            }

            if (vehicle != null)
            {
                employeeId = null;
                vehicleId = vehicle.Id;
                vehicle.LastPushToAzureTimeStamp = trackPoint.LastTimeStamp;
            }

            #endregion

            //Create the object to be stored in the Azure table
            var newTrackPoint = new TrackPointsHistoryTableDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            {
                EmployeetId = employeeId,
                VehicleId = vehicleId,
                RouteId = routeId,
                Latitude = trackPoint.Latitude,
                Longitude = trackPoint.Longitude,
                LastTimeStamp = trackPoint.LastTimeStamp
            };

            //Push to Azure Table
            serviceContext.AddObject(tableName, newTrackPoint);

            //Saves the Tables
            serviceContext.SaveChangesWithRetries();
        }

        #endregion
    }
}
