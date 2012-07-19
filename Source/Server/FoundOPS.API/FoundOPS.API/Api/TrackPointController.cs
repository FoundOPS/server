using FoundOPS.API.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ResourceWithLastPoint = FoundOPS.API.Models.ResourceWithLastPoint;

namespace FoundOPS.API.Api
{
    [FoundOps.Core.Tools.Authorize]
    public class TrackPointController : ApiController
    {
        public struct UpdateConstants
        {
            /// <summary>
            /// The minimum amount of seconds difference between TrackPoints' TimeStamps before storing them in Azure.
            /// </summary>
            public static readonly int SecondsBetweenHistoricalTrackPoints = 30;
        }

        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public TrackPointController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        #region GET

        // GET /api/trackpoint/GetTrackPoints?roleId={Guid}&date=Datetime
        /// <summary>
        /// Used to pull all TrackPoints that are assigned to a Route on a specific date
        /// </summary>
        /// <param name="roleId">Used to find the Business Account</param>
        /// <param name="routeId">The Id of the Route to pull Track Points for</param>
        /// <param name="serviceDateUtc">The date of the Route to look for (in UTC).</param>
        /// <returns>A Queryable list of TrackPoints</returns>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<TrackPoint> GetTrackPoints(Guid roleId, Guid routeId, DateTime serviceDateUtc)
        {
            //Get the storage account from Azure
            var storageAccount = CloudStorageAccount.Parse(AzureServerHelpers.StorageConnectionString);

            //Setup the service context for the TrackPointsHistory tables
            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            var businessAccount = _coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = businessAccount.Id.TrackPointTableName();

            var trackPointsDate = serviceDateUtc.Date;

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
        [AcceptVerbs("GET", "POST")]
        public IQueryable<ResourceWithLastPoint> GetResourcesWithLatestPoints(Guid roleId)
        {
            var currentBusinessAccount = _coreEntitiesContainer.Owner(roleId).First();

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
        /// <param name="trackPoints">The list of TrackPoints being passed from an Employee's device</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        [AcceptVerbs("POST")]
        public HttpResponseMessage PostEmployeeTrackPoint(TrackPoint[] trackPoints)
        {
            if (!trackPoints.Any() || !trackPoints.First().RouteId.HasValue)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var routeId = trackPoints.First().RouteId.Value;

            var currentUserAccount = _coreEntitiesContainer.CurrentUserAccount().First();
            var currentBusinessAccount =
                _coreEntitiesContainer.Parties.OfType<BusinessAccount>().FirstOrDefault(
                    ba =>
                    ba.Routes.Any(r => r.Id == routeId && r.Employees.Any(e => e.LinkedUserAccountId == currentUserAccount.Id)));

            //Return an Unauthorized Status Code if
            //a) there is no UserAccount
            //b) the user does not have access to the route (currentBusinessAccount == null)
            if (currentUserAccount == null || currentBusinessAccount == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            var employee = _coreEntitiesContainer.Employees.FirstOrDefault(e => e.LinkedUserAccount.Id == currentUserAccount.Id
                                                                                && e.EmployerId == currentBusinessAccount.Id);
            if (employee == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            //order the new track points by their TimeStamps
            var orderedModelTrackPoints = trackPoints.OrderBy(tp => tp.CollectedTimeStamp).ToArray();

            //Save the latest (most current) TrackPoint to the database
            var latestTrackPoint = orderedModelTrackPoints.Last();  //the last TrackPoint of orderedModelTrackPoints is the latest
            employee.LastCompassDirection = latestTrackPoint.Heading;
            employee.LastLatitude = (double?)latestTrackPoint.Latitude;
            employee.LastLongitude = (double?)latestTrackPoint.Longitude;
            employee.LastSource = latestTrackPoint.Source;
            employee.LastSpeed = (double?)latestTrackPoint.Speed;
            employee.LastTimeStamp = latestTrackPoint.CollectedTimeStamp;

            //Push TrackPoints to Azure as long as either
            //the LastPushToAzureTimeStamp is null
            //the LastPushToAzureTimeStamp difference from the TrackPoint is > TimeBetweenPushesToAzure

            var lastPushToAzureTimeStamp = employee.LastPushToAzureTimeStamp;

            //send the TrackPoints to azure, that are seperated by TimeBetweenPushesToAzure (in seconds)
            foreach (var trackPoint in orderedModelTrackPoints)
            {
                if (lastPushToAzureTimeStamp.HasValue && (trackPoint.CollectedTimeStamp - lastPushToAzureTimeStamp) < TimeSpan.FromSeconds(UpdateConstants.SecondsBetweenHistoricalTrackPoints))
                    continue;

                PushTrackPointToAzure(currentBusinessAccount, trackPoint, employee, routeId);

                lastPushToAzureTimeStamp = trackPoint.CollectedTimeStamp;
            }

            employee.LastPushToAzureTimeStamp = lastPushToAzureTimeStamp;

            //Save all the changes that have been made in the Database
            _coreEntitiesContainer.SaveChanges();

            //Create the Http response 
            var response = Request.CreateResponse(HttpStatusCode.Created, trackPoints);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/trackpoints/" + employee.Id.ToString());

            //Return the response
            return response;
        }

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
        /// Takes a ModelTrackPoint and pushed it to the AzureTables
        /// </summary>
        /// <param name="currentBusinessAccount">The current BusinessAccount</param>
        /// <param name="trackPoint">The Model TrackPoint to be pushed to AzureTables</param>
        /// <param name="employee">The employee</param>
        /// <param name="routeId">The Id of the Route that the vehicle or employee are currently on</param>
        private void PushTrackPointToAzure(BusinessAccount currentBusinessAccount, TrackPoint trackPoint, FoundOps.Core.Models.CoreEntities.Employee employee, Guid routeId)
        {
            //Get the storage account information from Azure
            var storageAccount = CloudStorageAccount.Parse(AzureServerHelpers.StorageConnectionString);

            //Create the Service Context for the TrackPointsHistory Tables
            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            var tableName = CheckCreateTrackPointTable(storageAccount, currentBusinessAccount);

            //Create the object to be stored in the Azure table
            var newTrackPoint = new TrackPointsHistoryTableDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            {
                EmployeeId = employee.Id,
                VehicleId = new Guid(),
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
