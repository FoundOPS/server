using Dapper;
using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Models;
using FoundOps.Common.NET;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using BusinessAccount = FoundOps.Core.Models.CoreEntities.BusinessAccount;
using Client = FoundOps.Core.Models.CoreEntities.Client;
using Location = FoundOps.Core.Models.CoreEntities.Location;
using RouteTask = FoundOps.Api.Models.RouteTask;
using TaskStatus = FoundOps.Core.Models.CoreEntities.TaskStatus;
using UserAccount = FoundOps.Core.Models.CoreEntities.UserAccount;

namespace FoundOps.Api.Tests.Controllers
{
    public static class TestTools
    {
        /// <summary>
        /// Creates a controller of type TController with a request
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="method">The type of request</param>
        /// <param name="headers">Optional headers to add to the request </param>
        public static TController CreateRequest<TController>(HttpMethod method, Dictionary<string, string> headers = null) where TController : BaseApiController
        {
            var request = new HttpRequestMessage(method, "http://localhost");

            //Add headers to the request if they exist
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Key, header.Value);
            }

            //sometimes this is necessary to work http://stackoverflow.com/questions/11053598/how-to-mock-the-createresponset-extension-method-on-httprequestmessage
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var controller = (TController)Activator.CreateInstance(typeof(TController));
            controller.Request = request;

            return controller;
        }
    }

    [TestClass]
    public class ResourceTests
    {
        protected readonly CoreEntitiesContainer CoreEntitiesContainerLocal;

        private readonly Guid _roleIdLocal;
        private readonly Guid _gotGreaseId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");

        //private const string ConnectionString = "metadata=res://*/Models.CoreEntities.CoreEntities.csdl|res://*/Models.CoreEntities.CoreEntities.ssdl|res://*/Models.CoreEntities.CoreEntities.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=f77m2u3n4m.database.windows.net;Initial Catalog=TestCore;Persist Security Info=True;User ID=perladmin;Password=QOI1m7DzVUJiNPMofFkk;MultipleActiveResultSets=True;Min Pool Size=100;Max Pool Size=1000;Pooling=true';";
        private const string CoreConnectionsString = "Data Source=f77m2u3n4m.database.windows.net;Initial Catalog=TestCore;Persist Security Info=True;User ID=perladmin;Password=QOI1m7DzVUJiNPMofFkk;MultipleActiveResultSets=True;Min Pool Size=100;Max Pool Size=1000;Pooling=true";

        public ResourceTests()
        {
            CoreEntitiesContainerLocal = new CoreEntitiesContainer();
            CoreEntitiesContainerLocal.ContextOptions.LazyLoadingEnabled = false;
            //Admin role on GotGrease?
            _roleIdLocal = CoreEntitiesContainerLocal.Roles.First(r => r.OwnerBusinessAccountId == _gotGreaseId && r.RoleTypeInt == (int)RoleType.Administrator).Id;
        }

        private void DetachAllEntities()
        {
            //Detach all entities
            var attached = CoreEntitiesContainerLocal.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged);
            foreach (var objectStateEntry in attached)
                CoreEntitiesContainerLocal.Detach(objectStateEntry.Entity);
        }

        [TestMethod]
        public void ClientsTests()
        {
            DetachAllEntities();
            var controller = TestTools.CreateRequest<ClientsController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleIdLocal, "Apollo");

            Assert.IsNotNull(getResponse.FirstOrDefault());
        }

        [TestMethod]
        public void ErrorsTests()
        {
            DetachAllEntities();
            var controller = TestTools.CreateRequest<ErrorsController>(HttpMethod.Get);

            var newError = new ErrorEntry
            {
                Business = "GotGrease?",
                Message = "There was an error!!",
                Url = "thisisafakeurl.com/notreal?true"
            };

            var putResponse = controller.Put(newError);

            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            Debug.WriteLine("All Error Controller Tests Passed");
        }

        [TestMethod]
        public void LocationsTests()
        {
            DetachAllEntities();
            var locationId = CoreEntitiesContainerLocal.Locations.First(l => l.BusinessAccountId == _gotGreaseId).Id;
            var clientId = CoreEntitiesContainerLocal.Clients.First(c => c.BusinessAccountId == _gotGreaseId).Id;

            var controller = TestTools.CreateRequest<LocationsController>(HttpMethod.Get);

            //Testing getting one location
            var getResponse = controller.Get(_roleIdLocal, locationId, null, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing getting all locations for a client
            getResponse = controller.Get(_roleIdLocal, null, clientId, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing getting all depot locations for a business account
            getResponse = controller.Get(_roleIdLocal, null, null, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Locations Controller Tests Passed");
        }

        [TestMethod]
        public void TestLocations()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new LocationsController { Request = request };

            var location = CoreEntitiesContainerLocal.Locations.First(l => l.BusinessAccountId == _gotGreaseId);
            var getResponseFromId = controller.Get(_roleIdLocal, location.Id, null);

            var convertedLocation = Api.Models.Location.ConvertModel(location);
            if (!Tools.AreObjectsEqual(getResponseFromId.First(), convertedLocation))
                throw new Exception("Objects are not equal. Check output window for details");

            //TODO uncomment when existing locations are included in search
            //var searchText = convertedLocation.AddressLineOne + ' ' + convertedLocation.AdminDistrictTwo + ' ' + convertedLocation.AdminDistrictOne + ' ' + convertedLocation.CountryCode;
            //var getResponseFromSearch = controller.GetAllLocations(_roleId, searchText).FirstOrDefault();
            //if (!Tools.AreObjectsEqual(getResponseFromSearch, convertedLocation, new[] { "Id", "Name", "AddressLineTwo" }))
            //    throw new Exception("Objects are not equal. Check output window for details");

            var getResponseFromSearch = controller.Get(_roleIdLocal, null, null, null, "12414 english garden, 20171").FirstOrDefault();
            Assert.AreEqual("20171", getResponseFromSearch.ZipCode);
            Assert.AreEqual("Herndon", getResponseFromSearch.City);
            Assert.AreEqual("VA", getResponseFromSearch.State);
            Assert.IsTrue(getResponseFromSearch.Latitude.Contains("38.8981361"));
            Assert.IsTrue(getResponseFromSearch.Longitude.Contains("-77.3790054"));

            var newLocation = new Api.Models.Location
            {
                Id = Guid.NewGuid(),
                Name = "New Location",
                AddressLineOne = "123 Fake Street",
                AddressLineTwo = "Apt ???",
                City = "Nowheresville",
                State = "Atlantis",
                ZipCode = "012345",
                CountryCode = "PO"
            };

            controller.Post(_roleIdLocal, newLocation);

            newLocation.CountryCode = "WK";
            newLocation.City = "I Have Changed";
            newLocation.AddressLineOne = "Not The Same as I Was";
            newLocation.AddressLineTwo = "Different";
            newLocation.ZipCode = "Not Even Postal Code";

            controller.Put(_roleIdLocal, newLocation);
        }

        [TestMethod]
        public void RoutesTests()
        {
            DetachAllEntities();
            var controller = TestTools.CreateRequest<RoutesController>(HttpMethod.Get);

            //Testing deep and assigned
            var getResponse = controller.Get(_roleIdLocal, DateTime.UtcNow.Date, true, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing not deep and not assigned
            getResponse = controller.Get(_roleIdLocal, DateTime.UtcNow.Date, false, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing not deep and assigned
            getResponse = controller.Get(_roleIdLocal, DateTime.UtcNow.Date, false, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing deep and not assigned
            getResponse = controller.Get(_roleIdLocal, DateTime.UtcNow.Date, true, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Routes Controller Tests Passed");
        }

        [TestMethod]
        public void SessionsTests()
        {
            DetachAllEntities();
            var headers = new Dictionary<string, string> { { "ops-details", "true" } };
            var controller = TestTools.CreateRequest<SessionsController>(HttpMethod.Get, headers);

            var getResponseWithHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, getResponseWithHeader.StatusCode);

            //now test simple response
            controller = TestTools.CreateRequest<SessionsController>(HttpMethod.Get);

            var getResponseWithoutHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.Accepted, getResponseWithoutHeader.StatusCode);

            Debug.WriteLine("All Sessions Controller Tests Passed");
        }

        [TestMethod]
        public void ServicesTests()
        {
            var service = CoreEntitiesContainerLocal.Services.Include(s => s.ServiceTemplate).Include(s => s.ServiceTemplate.Fields).First();
            var recurringService = CoreEntitiesContainerLocal.RecurringServices.Include(rs => rs.Repeat).First();
            var serviceDate = recurringService.Repeat.StartDate;
            var serviceTemplate = CoreEntitiesContainerLocal.ServiceTemplates.First(st => st.OwnerServiceProviderId != null);

            //Tests getting a service from an Id
            var controller = TestTools.CreateRequest<ServicesController>(HttpMethod.Get);
            var getResponse = controller.Get(service.Id, null, null, null);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            //Tests getting a service from a recurring service and a date
            getResponse = controller.Get(null, serviceDate, recurringService.Id, null);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            //Tests getting a service from a service template Id
            getResponse = controller.Get(null, serviceDate, null, serviceTemplate.Id);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            controller = TestTools.CreateRequest<ServicesController>(HttpMethod.Put);
            //fake generate a service by setting an Id and clearing the recurring service id
            var modelService = Models.Service.ConvertModel(service);
            modelService.Id = Guid.NewGuid();
            foreach (var field in modelService.Fields)
                field.Id = Guid.NewGuid();

            //Tests generating a service when one does not exist
            var putResponse = controller.Put(modelService);
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            controller = TestTools.CreateRequest<ServicesController>(HttpMethod.Delete);

            //Tests deleting a service
            var deleteResponse = controller.Delete(modelService);
            Assert.AreEqual(HttpStatusCode.Accepted, deleteResponse.StatusCode);

            //Tests updating an existing service
            controller = TestTools.CreateRequest<ServicesController>(HttpMethod.Put);

            modelService = Models.Service.ConvertModel(service);
            modelService.Name = "I have changed";

            putResponse = controller.Put(modelService);
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            Debug.WriteLine("All Services Controller Tests Passed");
        }

        [TestMethod]
        public void ServiceHoldersTests()
        {
            var clientId = CoreEntitiesContainerLocal.Clients.First().Id;
            var recurringServiceId = CoreEntitiesContainerLocal.RecurringServices.First().Id;

            var controller = TestTools.CreateRequest<ServiceHoldersController>(HttpMethod.Get);

            //Tests no service type and no context
            var getResponse = controller.Get(_roleIdLocal, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with no context
            getResponse = controller.Get(_roleIdLocal, "WVO Collection", null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests no service type with client context
            getResponse = controller.Get(_roleIdLocal, null, clientId, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with client context
            getResponse = controller.Get(_roleIdLocal, "WVO Collection", null, clientId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests no service type with recurring service context
            getResponse = controller.Get(_roleIdLocal, null, null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with recurring service context
            getResponse = controller.Get(_roleIdLocal, "WVO Collection", null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests just returning the column headers
            getResponse = controller.Get(_roleIdLocal, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31), true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Service Holders Controller Tests Passed");
        }

        [TestMethod]
        public void ServiceTemplatesTests()
        {
            var controller = TestTools.CreateRequest<ServiceTemplatesController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleIdLocal);

            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Service Templates Controller Tests Passed");
        }

        [TestMethod]
        public void ResourceWithLastPointsTests()
        {
            var controller = TestTools.CreateRequest<ResourceWithLastPointsController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleIdLocal);
            Assert.IsNotNull(getResponse.FirstOrDefault());
        }

        [TestMethod]
        public void RouteTasksTest()
        {
            DetachAllEntities();

            //update a route task's status to one that remove's it from a route
            var routeTask = CoreEntitiesContainerLocal.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var unroutedStatus = CoreEntitiesContainerLocal.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && ts.RemoveFromRoute);
            routeTask.TaskStatus = unroutedStatus;

            var controller = TestTools.CreateRequest<RouteTasksController>(HttpMethod.Get);

            var putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is no longer in a route
            DetachAllEntities();
            var updatedRouteTask = CoreEntitiesContainerLocal.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNull(updatedRouteTask.RouteDestinationId);

            //update a route task's status to one that will not remove it from a route
            routeTask = CoreEntitiesContainerLocal.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var routedStatus = CoreEntitiesContainerLocal.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && !ts.RemoveFromRoute);
            routeTask.TaskStatus = routedStatus;

            putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is still in a route
            DetachAllEntities();
            updatedRouteTask = CoreEntitiesContainerLocal.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNotNull(updatedRouteTask.RouteDestinationId);

            Debug.WriteLine("All RouteTasks Controller Tests Passed");
        }

        [TestMethod]
        public void TrackPointsTests()
        {
            //CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
            //CoreEntitiesServerManagement.CreateHistoricalTrackPoints();

            DetachAllEntities();
            var controller = TestTools.CreateRequest<TrackPointsController>(HttpMethod.Get);
            var date = DateTime.UtcNow.Date;

            var routeId = CoreEntitiesContainerLocal.Routes.Where(r => r.Date == date && r.OwnerBusinessAccountId == _gotGreaseId).RandomItem().Id;

            var getResponse = controller.Get(_roleIdLocal, routeId);

            Assert.IsNotNull(getResponse);

            var trackPoints = new List<TrackPoint>();
            var fakeRouteId = Guid.NewGuid();

            var random = new Random();

            var lat = (double)random.Next(-90, 90);
            var lon = (double)random.Next(-180, 180);

            var time = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));

            //Create the test TrackPoints with random numbers as properties
            for (int i = 0; i < 50; i++)
            {
                lon += +.007;
                lon += +.007;
                time = time.AddMinutes(1);

                var trackPoint = new TrackPoint
                {
                    Id = Guid.NewGuid(),
                    Heading = random.Next(0, 359),
                    Latitude = (decimal)lat,
                    Longitude = (decimal)lon,
                    CollectedTimeStamp = time,
                    Speed = random.Next(20, 60),
                    RouteId = fakeRouteId,
                    Source = "Testing",
                    Accuracy = 1
                };

                trackPoints.Add(trackPoint);
            }

            controller = TestTools.CreateRequest<TrackPointsController>(HttpMethod.Post);

            controller.Post(_roleIdLocal, trackPoints.ToArray());
        }

        //Begin SQL function tests

        [TestMethod]
        public void DeleteBusinessAccount()
        {
            var provider = CoreEntitiesContainerLocal.Parties.OfType<BusinessAccount>().First(ba => ba.Name != "FoundOPS");

            CoreEntitiesContainerLocal.DeleteBusinessAccountBasedOnId(provider.Id);
        }

        [TestMethod]
        public void DeleteUserAccount()
        {
            var user = CoreEntitiesContainerLocal.Parties.OfType<UserAccount>().First();

            CoreEntitiesContainerLocal.DeleteUserAccountBasedOnId(user.Id);
        }

        [TestMethod]
        public void DeleteClient()
        {
            var client = CoreEntitiesContainerLocal.Clients.First();

            CoreEntitiesContainerLocal.DeleteClientBasedOnId(client.Id);
        }

        [TestMethod]
        public void DeleteFieldAndChildren()
        {
            var field = CoreEntitiesContainerLocal.Fields.First(f => f.OwnerServiceTemplate.LevelInt == 0);

            CoreEntitiesContainerLocal.DeleteFieldAndChildrenBasedOnFieldId(field.Id);
        }

        [TestMethod]
        public void DeleteLocation()
        {
            var location = CoreEntitiesContainerLocal.Locations.First();

            CoreEntitiesContainerLocal.DeleteLocationBasedOnId(location.Id);
        }

        [TestMethod]
        public void DeleteRecurringService()
        {
            var recurringService = CoreEntitiesContainerLocal.RecurringServices.First();

            CoreEntitiesContainerLocal.DeleteRecurringService(recurringService.Id);
        }

        [TestMethod]
        public void DeleteServiceTemplateAndChildrenBasedOnId()
        {
            var template = CoreEntitiesContainerLocal.ServiceTemplates.First(st => st.LevelInt == 1);

            CoreEntitiesContainerLocal.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId(template.Id);
        }

        [TestMethod]
        public void DeleteServiceTemapltesAndChildrenBasedOnContext()
        {
            //Client
            var serviceTemplate = CoreEntitiesContainerLocal.ServiceTemplates.First(st => st.OwnerClientId != null);

            var deletedId = serviceTemplate.Id;

            CoreEntitiesContainerLocal.DeleteServiceTemplatesAndChildrenBasedOnContextId(null, serviceTemplate.OwnerClientId);

            var badDelete = CoreEntitiesContainerLocal.ServiceTemplates.FirstOrDefault(st => st.Id == deletedId || st.OwnerServiceTemplateId == deletedId);
            Assert.IsNull(badDelete);

            //finds a service template with an owner service provider that is not FoundOPS
            serviceTemplate = CoreEntitiesContainerLocal.ServiceTemplates.First(st => st.OwnerServiceProviderId != null && st.OwnerServiceProviderId != new Guid("5606A728-B99F-4AA1-B0CD-0AB38A649000"));
            deletedId = serviceTemplate.Id;

            //Business Account
            CoreEntitiesContainerLocal.DeleteServiceTemplatesAndChildrenBasedOnContextId(serviceTemplate.OwnerServiceProviderId, null);

            badDelete = CoreEntitiesContainerLocal.ServiceTemplates.FirstOrDefault(st => st.Id == deletedId || st.OwnerServiceTemplateId == deletedId);
            Assert.IsNull(badDelete);
        }

        [TestMethod]
        public void GetDateRangeForServices()
        {
            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                //Testing Service Provider Context
                var parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", _gotGreaseId);
                parameters.Add("@clientIdContext", null);
                parameters.Add("@recurringServiceIdContext", null);
                parameters.Add("@seedDate", DateTime.UtcNow.Date);
                parameters.Add("@frontBackMinimum", 50);
                parameters.Add("@getPrevious", 1);
                parameters.Add("@getNext", 1);
                parameters.Add("@serviceTypeContext", null);

                //Calls a stored procedure that will find any Services scheduled for today and create a routetask for them if one doesnt exist
                //Then it will return all RouteTasks that are not in a route joined with their Locations, Location.Regions and Clients
                //Dapper will then map the output table to RouteTasks, RouteTasks.Location, RouteTasks.Location.Region and RouteTasks.Client
                //While that is being mapped we also attach the Client, Location and Region to the objectContext
                var data = conn.QueryMultiple("GetDateRangeForServices", parameters, commandType: CommandType.StoredProcedure);

                var firstDate = data.Read<DateTime>().Single();
                var lastDate = data.Read<DateTime>().Single();

                //Note: This will only pass with if you did a CCDAPDD TODAY!
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-14), firstDate);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(14), lastDate);

                //Testing Client context
                parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", null);
                parameters.Add("@clientIdContext", CoreEntitiesContainerLocal.Clients.First().Id);
                parameters.Add("@recurringServiceIdContext", null);
                parameters.Add("@seedDate", DateTime.UtcNow.Date);
                parameters.Add("@frontBackMinimum", 50);
                parameters.Add("@getPrevious", 1);
                parameters.Add("@getNext", 1);
                parameters.Add("@serviceTypeContext", null);

                data = conn.QueryMultiple("GetDateRangeForServices", parameters, commandType: CommandType.StoredProcedure);

                firstDate = data.Read<DateTime>().Single();
                lastDate = data.Read<DateTime>().Single();

                //Note: This will only pass with if you did a CCDAPDD TODAY!
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-14), firstDate);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(322), lastDate);

                //Testing Recurring Service context
                parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", null);
                parameters.Add("@clientIdContext", null);
                parameters.Add("@recurringServiceIdContext", CoreEntitiesContainerLocal.RecurringServices.First().Id);
                parameters.Add("@seedDate", DateTime.UtcNow.Date);
                parameters.Add("@frontBackMinimum", 50);
                parameters.Add("@getPrevious", 1);
                parameters.Add("@getNext", 1);
                parameters.Add("@serviceTypeContext", null);

                data = conn.QueryMultiple("GetDateRangeForServices", parameters, commandType: CommandType.StoredProcedure);

                firstDate = data.Read<DateTime>().Single();
                lastDate = data.Read<DateTime>().Single();

                //Note: This will only pass with if you did a CCDAPDD TODAY!
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-14), firstDate);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(700), lastDate);

                conn.Close();

            }
        }

        [TestMethod]
        public void GetFieldsInJavaScriptFormat()
        {
            var data = CoreEntitiesContainerLocal.GetFieldsInJavaScriptFormat(_gotGreaseId, "WVO Collection").ToArray();

            Assert.IsNotNull(data.FirstOrDefault());
        }

        [TestMethod]
        public void ResourcesWithLastPoint()
        {
            var data = CoreEntitiesContainerLocal.GetResourcesWithLatestPoint(_gotGreaseId, DateTime.UtcNow.Date).ToArray();

            Assert.IsNotNull(data.FirstOrDefault());
        }

        [TestMethod]
        public void GetServiceHolders()
        {
            using (var conn = new SqlConnection(CoreConnectionsString))
            {
                conn.Open();

                //Testing Service Provider Context
                var parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", _gotGreaseId);
                parameters.Add("@clientIdContext", null);
                parameters.Add("@recurringServiceIdContext", null);
                parameters.Add("@firstDate", new DateTime(2012, 10, 18));
                parameters.Add("@lastDate", new DateTime(2012, 10, 19));
                parameters.Add("@serviceTypeContext", null);
                parameters.Add("@withFields", false);

                var serviceHolders = conn.Query<ServiceHolder>("GetServiceHolders", parameters, commandType: CommandType.StoredProcedure);
                Assert.IsNotNull(serviceHolders.FirstOrDefault());

                var recurringService = conn.Query<RecurringService>("SELECT TOP 1 * FROM dbo.RecurringServices WHERE ClientId IN (SELECT Id FROM dbo.Clients WHERE BusinessAccountId = (SELECT Id FROM dbo.Parties_BusinessAccount WHERE Name = 'GotGrease?'))").FirstOrDefault();

                //Testing Client Context
                parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", null);
                parameters.Add("@clientIdContext", new Guid(recurringService.ClientId.ToString()));
                parameters.Add("@recurringServiceIdContext", null);
                parameters.Add("@firstDate", new DateTime(2012, 10, 18));
                parameters.Add("@lastDate", new DateTime(2012, 10, 19));
                parameters.Add("@serviceTypeContext", null);
                parameters.Add("@withFields", false);

                serviceHolders = conn.Query<ServiceHolder>("GetServiceHolders", parameters, commandType: CommandType.StoredProcedure);
                Assert.IsNotNull(serviceHolders.FirstOrDefault());

                //Testing Recurring Service Context
                parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", null);
                parameters.Add("@clientIdContext", null);
                parameters.Add("@recurringServiceIdContext", recurringService.Id);
                parameters.Add("@firstDate", new DateTime(2012, 10, 18));
                parameters.Add("@lastDate", new DateTime(2012, 10, 30));
                parameters.Add("@serviceTypeContext", null);
                parameters.Add("@withFields", false);

                serviceHolders = conn.Query<ServiceHolder>("GetServiceHolders", parameters, commandType: CommandType.StoredProcedure);
                Assert.IsNotNull(serviceHolders.FirstOrDefault());

                conn.Close();
            }
        }

        [TestMethod]
        public void GetUnroutedServicesForDate()
        {
            using (var conn = new SqlConnection(CoreConnectionsString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", _gotGreaseId);
                parameters.Add("@serviceDate", DateTime.UtcNow.Date);

                //Calls a stored procedure that will find any Services scheduled for today and create a routetask for them if one doesnt exist
                //Then it will return all RouteTasks that are not in a route joined with their Locations, Location.Regions and Clients
                //Dapper will then map the output table to RouteTasks, RouteTasks.Location, RouteTasks.Location.Region and RouteTasks.Client
                //While that is being mapped we also attach the Client, Location and Region to the objectContext
                var data = conn.Query<FoundOps.Core.Models.CoreEntities.RouteTask, Location, Region, Client, TaskStatus, FoundOps.Core.Models.CoreEntities.RouteTask>("sp_GetUnroutedServicesForDate", (routeTask, location, region, client, taskStatus) =>
                {
                    if (location != null)
                    {
                        CoreEntitiesContainerLocal.DetachExistingAndAttach(location);
                        routeTask.Location = location;

                        if (region != null)
                        {
                            CoreEntitiesContainerLocal.DetachExistingAndAttach(region);
                            routeTask.Location.Region = region;
                        }
                    }

                    if (client != null)
                    {
                        CoreEntitiesContainerLocal.DetachExistingAndAttach(client);
                        routeTask.Client = client;
                    }

                    if (taskStatus != null)
                    {
                        CoreEntitiesContainerLocal.DetachExistingAndAttach(taskStatus);
                        routeTask.TaskStatus = taskStatus;
                    }

                    return routeTask;
                }, parameters, commandType: CommandType.StoredProcedure);

                Assert.IsNotNull(data.FirstOrDefault());

                conn.Close();
            }
        }

        [TestMethod]
        public void PropagateNewFields()
        {
            #region new Numeric field

            var id = Guid.NewGuid();

            var numericField = new FoundOps.Core.Models.CoreEntities.NumericField
                {
                    Id = id,
                    Name = "Numeric Test",
                    Minimum = 10,
                    Maximum = 20,
                    Mask = "c"
                };

            var template = CoreEntitiesContainerLocal.ServiceTemplates.Where(st => st.LevelInt == 1 && st.Name == "WVO Collection" && st.OwnerServiceProviderId == _gotGreaseId).Include(st => st.Fields).First();

            template.Fields.Add(numericField);

            CoreEntitiesContainerLocal.SaveChanges();

            CoreEntitiesContainerLocal.PropagateNewFields(id);

            #endregion
        }
    }
}
