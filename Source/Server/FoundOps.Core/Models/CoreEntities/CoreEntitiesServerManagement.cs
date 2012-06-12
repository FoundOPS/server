using System.Data.Services.Client;
using System.Linq;
using FoundOps.Common.NET;
using FoundOps.Common.Server;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities.DesignData;
using Microsoft.SqlServer.Management.Common;
using System;
using System.IO;
using System.Data.SqlClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.CoreEntities
{
    public class CoreEntitiesServerManagement
    {
        #region ConnectionString, Paths and Parameters

        public static readonly string SqlConnectionString = UserSpecificResourcesWrapper.ConnectionString("CoreConnectionString");
        private static readonly string ContainerConnectionString = UserSpecificResourcesWrapper.ConnectionString("CoreEntitiesContainer");

        private static readonly string RootDirectory = ServerConstants.RootDirectory;

        private static readonly string ClearCoreEntitiesDatabaseScriptLocation =
            RootDirectory + @"\FoundOps.Core\Models\CoreEntities\ClearCoreEntities.edmx.sql";

        private static readonly string CreateCoreEntitiesDatabaseScriptLocation =
            RootDirectory + @"\FoundOps.Core\Models\CoreEntities\CoreEntitiesCorrected1.edmx.sql";

        #endregion

#if DEBUG
        public static void ClearCreateCoreEntitiesDatabase()
        {
            var clearFile = new FileInfo(ClearCoreEntitiesDatabaseScriptLocation);
            var createFile = new FileInfo(CreateCoreEntitiesDatabaseScriptLocation);
            string clearScript = clearFile.OpenText().ReadToEnd();
            string createScript = createFile.OpenText().ReadToEnd();
            using (var conn = new SqlConnection(SqlConnectionString))
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(conn));
                server.ConnectionContext.ExecuteNonQuery(clearScript);
                server.ConnectionContext.ExecuteNonQuery(createScript);
            }
            clearFile.OpenText().Close();
            createFile.OpenText().Close();
        }

        public static void PopulateBlocks(CoreEntitiesContainer coreEntitiesContainer)
        {
            foreach (var block in BlocksData.AllBlocks)
            {
                block.Link = String.Format("/{0}", block.NavigateUri);
                coreEntitiesContainer.Blocks.AddObject(block);
            }
        }

        /// <summary>
        /// Clear and create the database. Then populate it with design data.
        /// </summary>
        public static void ClearCreateCoreEntitiesDatabaseAndPopulateDesignData()
        {
            ClearCreateCoreEntitiesDatabase();

            var container = new CoreEntitiesContainer(ContainerConnectionString);

            //Setup blocks
            PopulateBlocks(container);

            //Setup Business Accounts (and ServiceTemplates)
            var businessAccountsDesignData = new BusinessAccountsDesignData();

            //Setup User Accounts
            var userAccountsDesignData = new UserAccountsDesignData
                                             {
                                                 //Set Passwords
                                                 Andrew = { PasswordHash = EncryptionTools.Hash("f00sballchamp") },
                                                 Jon = { PasswordHash = EncryptionTools.Hash("seltzer") },
                                                 Oren = { PasswordHash = EncryptionTools.Hash("nachoman") },
                                                 Zach = { PasswordHash = EncryptionTools.Hash("curlyhair") },
                                                 David = { PasswordHash = EncryptionTools.Hash("starofdavid") },
                                                 Linda = { PasswordHash = EncryptionTools.Hash("auntie") },
                                                 Terri = { PasswordHash = EncryptionTools.Hash("iam1randompassword") },
                                                 AlanMcClure = { PasswordHash = EncryptionTools.Hash("youngster") }
                                             };


            //Setup roles
            new RolesDesignData(businessAccountsDesignData, userAccountsDesignData);

            //Populate ServiceProvider Design Data
            foreach (var serviceProvider in businessAccountsDesignData.DesignServiceProviders)
            {
                var employeesDesignData = new EmployeesDesignData(serviceProvider);
                foreach (var employee in employeesDesignData.DesignEmployees)
                    serviceProvider.Employees.Add(employee);

                container.SaveChanges();

                //Add Vehicles (and Vehicle Maintenance, and VehicleTypes)
                var vehiclesDesignData = new VehiclesDesignData(serviceProvider);
                foreach (var vehicle in vehiclesDesignData.DesignVehicles)
                    serviceProvider.Vehicles.Add(vehicle);

                container.SaveChanges();

                //Add Regions
                var regionsDesignData = new RegionsDesignData();
                foreach (var region in regionsDesignData.DesignRegions)
                    serviceProvider.Regions.Add(region);

                container.SaveChanges();

                //Add Clients (and ContactInfo, Locations, and RecurringServices)
                var clientsDesignData = new ClientsDesignData(serviceProvider, regionsDesignData);
                foreach (var client in clientsDesignData.DesignClients)
                    serviceProvider.Clients.Add(client);
                
                container.SaveChanges();

                //Add Routes (with RouteTasks, Vehicles, Technicians)
                var routesDesignData = new RoutesDesignData(serviceProvider, clientsDesignData, vehiclesDesignData, employeesDesignData);
                foreach (var route in routesDesignData.DesignRoutes)
                    serviceProvider.Routes.Add(route);

                container.SaveChanges();

                //Add Services
                var servicesDesignData = new ServicesDesignData(serviceProvider, clientsDesignData.DesignClient, serviceProvider.ServiceTemplates);
                foreach (var service in servicesDesignData.DesignServices)
                    serviceProvider.ServicesToProvide.Add(service);
            }

            container.SaveChanges();
        }

        /// <summary>
        /// Clear and create the HistoricalTrackPoints on the Azure tables.
        /// </summary>
        public static void ClearCreateHistoricalTrackPoints()
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            var businessAccountIds = coreEntitiesContainer.Parties.OfType<BusinessAccount>().Select(ba => ba.Id);

            foreach (var businessAccountId in businessAccountIds)
            {
                //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
                var tableName = "tp" + businessAccountId.ToString().Replace("-", "");

                var storageAccount = CloudStorageAccount.Parse(AzureHelpers.StorageConnectionString);
                var serviceContext = new TrackPointsHistoryContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

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
                            var employees = route.Employees;
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
                                            CollectedTimeStamp = timeStamp,
                                            Accuracy = 1
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
                            employees = route.Employees;
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
                                            CollectedTimeStamp = timeStamp,
                                            Accuracy = 1
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
                            employees = route.Employees;
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
                                            CollectedTimeStamp = timeStamp,
                                            Accuracy = 2
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
                            employees = route.Employees;
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
                                            CollectedTimeStamp = timeStamp,
                                            Accuracy = 3
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
        }
#endif
    }
}
