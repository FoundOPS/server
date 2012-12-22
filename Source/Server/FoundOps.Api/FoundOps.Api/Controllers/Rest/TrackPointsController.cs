using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Common.Tools;
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

namespace FoundOps.Api.Controllers.Rest
{
    [Core.Tools.Authorize]
    public class TrackPointsController : BaseApiController
    {
        public struct UpdateConstants
        {
            /// <summary>
            /// The minimum amount of seconds difference between TrackPoints' TimeStamps before storing them in Azure.
            /// </summary>
            public static readonly int SecondsBetweenHistoricalTrackPoints = 30;
        }

        /// <summary>
        /// Used to pull all TrackPoints that are assigned to a Route on a specific date
        /// </summary>
        /// <param name="roleId">Used to find the Business Account</param>
        /// <param name="routeId">The Id of the Route to pull Track Points for</param>
        /// <returns>A Queryable list of TrackPoints</returns>
        public IQueryable<TrackPoint> Get(Guid roleId, Guid routeId)
        {
            var connectionString = AzureServerHelpers.StorageConnectionString;
            //Get the storage account from Azure
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            //Setup the service context for the TrackPointsHistory tables
            var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

            var businessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = businessAccount.Id.TrackPointTableName();

            //Gets all objects from the Azure table specified on the date requested and returns the result
            var trackPoints = serviceContext.CreateQuery<TrackPointsHistoryTableDataModel>(tableName)
                .Where(tp => tp.RouteId == routeId)
                //The following allows us to get more than 1000 rows at a time
                .AsTableServiceQuery().Execute().OrderBy(tp => tp.CollectedTimeStamp).Select(TrackPoint.ConvertToModel).ToArray();

            var filteredTrackPoints = new List<TrackPoint>();

            //Filter out erroneous points
            for (int i = 0; i < trackPoints.Count(); i++)
            {
                var current = trackPoints[i];

                var previous = trackPoints.ElementAtOrDefault(i - 1);
                if (previous != null)
                {
                    var timeDelta = current.CollectedTimeStamp.Subtract(previous.CollectedTimeStamp);
                    if (Erroneous(previous, current, timeDelta))
                        continue;

                    var next = trackPoints.ElementAtOrDefault(i + 1);
                    if (next != null && next.CollectedTimeStamp.Subtract(current.CollectedTimeStamp) < TimeSpan.FromSeconds(5))
                    {
                        //TODO log this and figure out why the server is saving these trackpoints
                        continue;
                    }

                    //for debugging erroneous points
                    //var distanceToErroneous = GeoLocationTools.VincentyDistanceFormula(current, new GeoLocation(20.899650842339007, -156.41764283180234));
                    //if (distanceToErroneous < .001) //clicking on the map has a certain amount of inaccuracy
                    //{
                    //    var next = trackPoints.ElementAtOrDefault(i + 1);
                    //    var nextTimeDelta = next.CollectedTimeStamp.Subtract(current.CollectedTimeStamp);
                    //    var nextDistanceDelta = GeoLocationTools.VincentyDistanceFormula(current, next) * 1000.0;
                    //    var nextVelocity = nextDistanceDelta / nextTimeDelta.TotalSeconds;
                    //}
                }

                filteredTrackPoints.Add(current);
            }

            return filteredTrackPoints.AsQueryable();
        }

        /// <summary>
        /// If the traveled distance is too far, ignore it
        /// </summary>
        /// <returns></returns>
        private bool Erroneous(IGeoLocation previous, IGeoLocation current, TimeSpan timeDelta)
        {
            var distanceDelta = GeoLocationTools.VincentyDistanceFormula(previous, current) * 1000.0;

            //meters per second
            var velocity = distanceDelta / timeDelta.TotalSeconds;

            //if the distance traveled > 45 m/s (100 mph), it is erroneous
            if (velocity > 45)
            {
                //TODO log this and figure out why the mobile phones are sending these trackpoints, or what is going wrong
                return true;
            }

            //if the point moved in the range of centimeters, it is erroneous
            if (Math.Abs(distanceDelta - 0) < 0.0000001)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stores employee trackpoints.
        /// Used by the mobile phone application.
        /// NOTE: TrackPoints CollectedTimeStamp should be in UTC.
        /// </summary>
        /// <param name="roleId">Current roleId </param>
        /// <param name="trackPoints">The list of TrackPoints being passed from an Employee's device</param>
        /// <returns>An Http response to the device signaling that the TrackPoint was successfully created</returns>
        public void Post(Guid roleId, TrackPoint trackPoint)
        {
            if (trackPoint == null || !trackPoint.RouteId.HasValue)
                throw Request.BadRequest();

            //remove inaccurate trackpoints
            if (trackPoint.Accuracy > 50)
                return;

            var routeId = trackPoint.RouteId.Value;

            var currentUserAccount = CoreEntitiesContainer.CurrentUserAccount();
            var currentBusinessAccount =
                CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator, RoleType.Regular, RoleType.Mobile })
                                     .FirstOrDefault();

            //Return an Unauthorized Status Code if
            //a) there is no UserAccount
            //b) the user does not have access to the route (currentBusinessAccount == null)
            if (currentUserAccount == null || currentBusinessAccount == null)
                throw Request.NotAuthorized();

            var employee =
                CoreEntitiesContainer.Employees.FirstOrDefault(e => e.LinkedUserAccount.Id == currentUserAccount.Id
                                                                    && e.EmployerId == currentBusinessAccount.Id);
            if (employee == null)
                throw Request.NotFound("Employee");

            //if there was a previous point today, check the latest point is not erroneous
            if (employee.LastTimeStamp.HasValue &&
                DateTime.UtcNow.Subtract(employee.LastTimeStamp.Value) < TimeSpan.FromDays(1))
            {
                var previous = new GeoLocation(employee.LastLatitude.Value, employee.LastLongitude.Value);
                if (Erroneous(trackPoint, previous, trackPoint.CollectedTimeStamp.Subtract(employee.LastTimeStamp.Value)))
                    trackPoint = null;

                //set the heading based on the previous point
                if (trackPoint != null)
                    trackPoint.Heading = (int)GeoLocationTools.Bearing(previous, trackPoint);
            }

            if (trackPoint != null)
            {
                //Save the latest (most current) TrackPoint to the database
                employee.LastCompassDirection = trackPoint.Heading;
                employee.LastLatitude = (double?)trackPoint.Latitude;
                employee.LastLongitude = (double?)trackPoint.Longitude;
                employee.LastSource = trackPoint.Source;
                employee.LastSpeed = (double?)trackPoint.Speed;
                employee.LastTimeStamp = trackPoint.CollectedTimeStamp;
                employee.LastAccuracy = trackPoint.Accuracy;
            }

            //Push TrackPoints to Azure as long as either
            //the LastPushToAzureTimeStamp is null
            //the LastPushToAzureTimeStamp difference from the TrackPoint is > TimeBetweenPushesToAzure
            var lastPushToAzureTimeStamp = employee.LastPushToAzureTimeStamp;

            //send the TrackPoint to azure, that are seperated by TimeBetweenPushesToAzure (in seconds)

            if (lastPushToAzureTimeStamp.HasValue &&
                (trackPoint.CollectedTimeStamp - lastPushToAzureTimeStamp) >=
                TimeSpan.FromSeconds(UpdateConstants.SecondsBetweenHistoricalTrackPoints))
            {
                if (trackPoint.Id == Guid.Empty)
                    trackPoint.Id = Guid.NewGuid();
                PushTrackPointToAzure(currentBusinessAccount, trackPoint, employee, routeId);
                lastPushToAzureTimeStamp = trackPoint.CollectedTimeStamp;
                employee.LastPushToAzureTimeStamp = lastPushToAzureTimeStamp;
            }

            //Save all the changes that have been made in the Database
            SaveWithRetry();
        }

        #region Helpers

        /// <summary>
        /// Checks for an existing TrackPoint table for the BusinessAccount.
        /// If there is not one, it will create a new one.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="businessAccount">The business account.</param>
        /// <returns></returns>
        private string CheckCreateTrackPointTable(CloudStorageAccount storageAccount, Core.Models.CoreEntities.BusinessAccount businessAccount)
        {
            //Create the Tables Client -> Used to Check if the table exists and to create it if it doesnt already exist
            var tableClient = storageAccount.CreateCloudTableClient();

            var tableName = businessAccount.Id.TrackPointTableName();

            //If an exception occurs, it means that the table already exists
            try
            {
                tableClient.CreateTableIfNotExist(tableName);
            }
            catch (Exception)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, "Table was not created"));
            }

            return tableName;
        }

        /// <summary>
        /// Takes a ModelTrackPoint and pushed it to the AzureTables
        /// </summary>
        /// <param name="currentBusinessAccount">The current BusinessAccount</param>
        /// <param name="trackPoint">The Model TrackPoint to be pushed to AzureTables</param>
        /// <param name="employee">The employee</param>
        /// <param name="routeId">The Id of the Route that the vehicle or employee are currently on</param>
        private void PushTrackPointToAzure(Core.Models.CoreEntities.BusinessAccount currentBusinessAccount, TrackPoint trackPoint, Core.Models.CoreEntities.Employee employee, Guid routeId)
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
                CollectedTimeStamp = trackPoint.CollectedTimeStamp,
                Accuracy = trackPoint.Accuracy
            };

            //Push to Azure Table
            serviceContext.AddObject(tableName, newTrackPoint);

            //Saves the Tables
            serviceContext.SaveChangesWithRetries();
        }

        #endregion
    }
}