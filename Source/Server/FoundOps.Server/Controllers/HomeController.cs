using System;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FoundOps.Common.Tools;
using FoundOps.Core.Models;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Server.Controllers
{
    public class HomeController : Controller
    {
        public struct Constants
        {
#if DEBUG
            public const string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=opsappdebug;AccountKey=Wbs5xOmAKdNw8ef9XgRZF2lhE+DYH1uN0qgETVKSCLqIXaaTRjiFIj4sT2cf0iQxUdOAYEej2VI4aPBr7TVOYA==";
#else
            public const string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=opsapplive;AccountKey=j9CJzOyyBkPmVccwc269o52l283iyGFgX8UMlpyfIMTdF0+Ka14kAAlHJiRC/WhdcrVdO0Kd+MRG/iTdddJATw==";
#endif
        }

        public ActionResult Index()
        {
#if DEBUG
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
                return RedirectToAction("Silverlight", "Home");
#endif

            if (!HttpContext.User.Identity.IsAuthenticated)
                return Redirect(Global.RootFrontSiteUrl);

            return RedirectToAction("Silverlight", "Home");
        }

        //Cannot require the page to be HTTPS until we have our own tile server
        //#if !DEBUG
        //        [RequireHttps]
        //#endif
        [AddTestUsersThenAuthorize]
        public ActionResult Silverlight()
        {
#if DEBUG
            var random = new Random();
            var version = random.Next(10000).ToString();
#else
            var request = (HttpWebRequest)WebRequest.Create(AzureTools.BlobStorageUrl + "xaps/version.txt");

            // *** Retrieve request info headers
            var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(response.GetResponseStream());

            var version = stream.ReadToEnd();
            response.Close();
            stream.Close();
#endif

            //Must cast the string as an object so it uses the correct overloaded method
            return View((object)version);
        }

#if DEBUG

        /// <summary>
        /// Clear, create database, and populate design data.
        /// </summary>
        /// <returns></returns>
        public ActionResult CCDAPDD()
        {
            if (ServerConstants.AutomaticLoginFoundOPSAdmin || ServerConstants.AutomaticLoginOPSManager)
            {
                CoreEntitiesServerManagement.ClearCreateCoreEntitiesDatabaseAndPopulateDesignData();
                return View();
            }

            throw new Exception("Invalid attempted access logged for investigation.");
        }

        public ActionResult ClearCreateHistoricalTrackPoints()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var businessAccountIds = coreEntitiesContainer.Parties.OfType<BusinessAccount>().Select(ba => ba.Id);

            foreach (var businessAccountId in businessAccountIds)
            {
                //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
                var tableName = "tp" + businessAccountId.ToString().Replace("-", "");

                var storageAccount = CloudStorageAccount.Parse(Constants.StorageConnectionString);

                var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(),
                                                                        storageAccount.Credentials);

                var tableClient = storageAccount.CreateCloudTableClient();

                try
                {
                    //Delete the Table in Azure and all rows in it
                    //tableClient.DeleteTableIfExist(tableName);

                    //Create the empty table once again
                    tableClient.CreateTableIfNotExist(tableName);
                }
                catch { }

                var serviceDate = DateTime.UtcNow;

                var routes = coreEntitiesContainer.Routes.Where(r => r.Date == serviceDate.Date && r.OwnerBusinessAccountId == businessAccountId).OrderBy(r => r.Id);

                #region Setup Design Data for Historical Track Points

                var numberOfRoutes = routes.Count();

                var count = 1;

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
                                var latitude = 40.4599;
                                var longitude = -86.9309;

                                //Sets timeStamp to be 9:00 AM yesterday
                                var timeStamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 0, 0);

                                for (var totalCount = 1; totalCount <= 3; totalCount++)
                                {
                                    var partitionKey = Guid.NewGuid().ToString();
                                    for (var innerCount = 1; innerCount < 99; innerCount++)
                                    {
                                        //Create the object to be stored in the Azure table
                                        var newTrackPoint = new TrackPointsHistoryTableDataModel(partitionKey, Guid.NewGuid().ToString())
                                        {
                                            EmployeeId = employee.Id,
                                            VehicleId = null,
                                            RouteId = route.Id,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            CollectedTimeStamp = timeStamp
                                        };

                                        latitude = latitude + .001;
                                        longitude = longitude + .001;
                                        timeStamp = timeStamp.AddSeconds(30);

                                        //Push to Azure Table
                                        serviceContext.AddObject(tableName, newTrackPoint);
                                    }
                                    serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                                }
                            }
                            break;
                        //This would be the 1st, 5th, etc
                        case 1:
                            employees = route.Technicians;
                            foreach (var employee in employees)
                            {
                                var latitude = 40.4599;
                                var longitude = -86.9309;

                                //Sets timeStamp to be 9:00 AM yesterday
                                var timeStamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 0, 0);

                                for (var totalCount = 1; totalCount <= 3; totalCount++)
                                {
                                    var partitionKey = Guid.NewGuid().ToString();

                                    for (var innerCount = 1; innerCount <= 99; innerCount++)
                                    {
                                        //Create the object to be stored in the Azure table
                                        var newTrackPoint = new TrackPointsHistoryTableDataModel(partitionKey, Guid.NewGuid().ToString())
                                        {
                                            EmployeeId = employee.Id,
                                            VehicleId = null,
                                            RouteId = route.Id,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            CollectedTimeStamp = timeStamp
                                        };

                                        latitude = latitude - .001;
                                        longitude = longitude + .001;
                                        timeStamp = timeStamp.AddSeconds(30);

                                        //Push to Azure Table
                                        serviceContext.AddObject(tableName, newTrackPoint);
                                    }
                                    serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);

                                }
                            }
                            break;
                        //This would be the 2nd, 6th, etc
                        case 2:
                            employees = route.Technicians;
                            foreach (var employee in employees)
                            {
                                var latitude = 40.4599;
                                var longitude = -86.9309;

                                //Sets timeStamp to be 9:00 AM yesterday
                                var timeStamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 0, 0);

                                for (var totalCount = 1; totalCount <= 3; totalCount++)
                                {
                                    var partitionKey = Guid.NewGuid().ToString();

                                    for (var innerCount = 1; innerCount <= 99; innerCount++)
                                    {
                                        //Create the object to be stored in the Azure table
                                        var newTrackPoint = new TrackPointsHistoryTableDataModel(partitionKey, Guid.NewGuid().ToString())
                                        {
                                            EmployeeId = employee.Id,
                                            VehicleId = null,
                                            RouteId = route.Id,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            CollectedTimeStamp = timeStamp
                                        };

                                        latitude = latitude + .001;
                                        longitude = longitude - .001;
                                        timeStamp = timeStamp.AddSeconds(30);

                                        //Push to Azure Table
                                        serviceContext.AddObject(tableName, newTrackPoint);
                                    }
                                    serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                                }
                            }
                            break;
                        //This would be the 3rd, 7th, etc
                        case 3:
                            employees = route.Technicians;
                            foreach (var employee in employees)
                            {
                                var latitude = 40.4599;
                                var longitude = -86.9309;

                                //Sets timeStamp to be 9:00 AM yesterday
                                var timeStamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 0, 0);

                                for (var totalCount = 1; totalCount <= 3; totalCount++)
                                {
                                    var partitionKey = Guid.NewGuid().ToString();

                                    for (var innerCount = 1; innerCount <= 99; innerCount++)
                                    {
                                        //Create the object to be stored in the Azure table
                                        var newTrackPoint = new TrackPointsHistoryTableDataModel(partitionKey, Guid.NewGuid().ToString())
                                        {
                                            EmployeeId = employee.Id,
                                            VehicleId = null,
                                            RouteId = route.Id,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            CollectedTimeStamp = timeStamp
                                        };

                                        latitude = latitude - .001;
                                        longitude = longitude - .001;
                                        timeStamp = timeStamp.AddSeconds(30);

                                        //Push to Azure Table
                                        serviceContext.AddObject(tableName, newTrackPoint);
                                    }
                                    serviceContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                                }
                            }
                            break;
                    }
                    count++;
                }

                #endregion
            }

            return View();
        }

        [AddTestUsersThenAuthorize]
        public ActionResult PerformServerOperations()
        {
            //TODO Authenticate first
            ServerManagement.PerformServerOperations();
            return View();
        }
#endif
    }
}