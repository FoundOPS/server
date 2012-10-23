﻿using Dapper;
using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Models;
using FoundOps.Common.NET;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using KellermanSoftware.CompareNetObjects;
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
using DateTimeField = FoundOps.Core.Models.CoreEntities.DateTimeField;
using Field = FoundOps.Core.Models.CoreEntities.Field;
using Location = FoundOps.Core.Models.CoreEntities.Location;
using NumericField = FoundOps.Core.Models.CoreEntities.NumericField;
using OptionsField = FoundOps.Core.Models.CoreEntities.OptionsField;
using LocationField = FoundOps.Core.Models.CoreEntities.LocationField;
using RouteTask = FoundOps.Api.Models.RouteTask;
using ServiceTemplate = FoundOps.Core.Models.CoreEntities.ServiceTemplate;
using TaskStatus = FoundOps.Core.Models.CoreEntities.TaskStatus;
using TextBoxField = FoundOps.Core.Models.CoreEntities.TextBoxField;
using UserAccount = FoundOps.Core.Models.CoreEntities.UserAccount;

namespace FoundOps.Api.Tests.Controllers
{
    [TestClass]
    public class ResourceTests
    {
        protected readonly CoreEntitiesContainer CoreEntitiesContainer;
        private static readonly DateTime Today = DateTime.UtcNow.Date;

        private readonly Guid _roleId;
        private readonly Guid _gotGreaseId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");
        private readonly Guid _foundOpsId = new Guid("5606A728-B99F-4AA1-B0CD-0AB38A649000");

        #region Fields

        private NumericField NewNumericField()
        {
            return new NumericField
                {
                    Id = Guid.NewGuid(),
                    Name = "Numeric Test",
                    Minimum = 10,
                    Maximum = 20,
                    Mask = "c"
                };
        }

        private OptionsField NewCheckListField()
        {
            return new OptionsField
                {
                    Id = Guid.NewGuid(),
                    AllowMultipleSelection = true,
                    Name = "Checklist Test",
                    TypeInt = ((int)OptionsType.Checklist),
                    OptionsString = "Option 1,Option 2,Option 3",
                    Value = "0,2"
                };
        }

        private OptionsField NewCheckBoxField()
        {
            return new OptionsField
                {
                    Id = Guid.NewGuid(),
                    AllowMultipleSelection = false,
                    Name = "Checkbox Test",
                    TypeInt = ((int)OptionsType.Checkbox),
                    OptionsString = "Option 1",
                    Value = "0"
                };
        }

        private OptionsField NewComboBoxField()
        {
            return new OptionsField
                {
                    Id = Guid.NewGuid(),
                    AllowMultipleSelection = true,
                    Name = "Combobox Test",
                    TypeInt = ((int)OptionsType.Combobox),
                    OptionsString = "Option 1,Option 2,Option 3",
                    Value = "1,2"
                };
        }

        private LocationField NewLocationField()
        {
            return new LocationField
                {
                    Id = Guid.NewGuid(),
                    Name = "Location Test",
                    LocationFieldTypeInt = 0,
                    LocationId = CoreEntitiesContainer.Locations.First().Id
                };
        }

        private TextBoxField NewSmallTextBoxField()
        {
            return new TextBoxField
                {
                    Id = Guid.NewGuid(),
                    Name = "Small TextBox Test",
                    IsMultiline = false,
                    Value = "Small TextBox"
                };
        }

        private TextBoxField NewLargeTextBoxField()
        {
            return new TextBoxField
                {
                    Id = Guid.NewGuid(),
                    Name = "Large TextBox Test",
                    IsMultiline = true,
                    Value = "Large TextBox"
                };
        }

        private DateTimeField NewDateTimeField()
        {
            return new DateTimeField
                {
                    Id = Guid.NewGuid(),
                    Name = "DateTime Test",
                    Earliest = new DateTime(Today.Year, Today.Month, Today.Day, Today.AddHours(-2).Hour, 0, 0),
                    Latest = new DateTime(Today.Year, Today.Month, Today.Day, Today.AddHours(2).Hour, 0, 0),
                    TypeInt = ((int)DateTimeType.DateTime)
                };
        }

        #endregion

        #region Helpers

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

        /// <summary>
        /// Tests if an object is returned
        /// </summary>
        /// <typeparam name="TController">The controller</typeparam>
        /// <typeparam name="T">The get response type</typeparam>
        /// <param name="get">The get method to perform</param>
        public static IQueryable<T> SimpleGetTest<TController, T>(Func<TController, IQueryable<T>> get) where TController : BaseApiController
        {
            var controller = CreateRequest<TController>(HttpMethod.Get);
            var result = get(controller);
            Assert.IsNotNull(result.FirstOrDefault());
            return result;
        }

        #endregion

        //private const string ConnectionString = "metadata=res://*/Models.CoreEntities.CoreEntities.csdl|res://*/Models.CoreEntities.CoreEntities.ssdl|res://*/Models.CoreEntities.CoreEntities.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=f77m2u3n4m.database.windows.net;Initial Catalog=TestCore;Persist Security Info=True;User ID=perladmin;Password=QOI1m7DzVUJiNPMofFkk;MultipleActiveResultSets=True;Min Pool Size=100;Max Pool Size=1000;Pooling=true';";
        private const string CoreConnectionsString = "Data Source=f77m2u3n4m.database.windows.net;Initial Catalog=TestCore;Persist Security Info=True;User ID=perladmin;Password=QOI1m7DzVUJiNPMofFkk;MultipleActiveResultSets=True;Min Pool Size=100;Max Pool Size=1000;Pooling=true";

        public ResourceTests()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
            //Admin role on GotGrease?
            _roleId = CoreEntitiesContainer.Roles.First(r => r.OwnerBusinessAccountId == _gotGreaseId && r.RoleTypeInt == (int)RoleType.Administrator).Id;
        }

        private void DetachAllEntities()
        {
            //Detach all entities
            var attached = CoreEntitiesContainer.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged);
            foreach (var objectStateEntry in attached)
                CoreEntitiesContainer.Detach(objectStateEntry.Entity);
        }

        [TestMethod]
        public void ClientsTests()
        {
            DetachAllEntities();
            SimpleGetTest<ClientsController, Models.Client>(c => c.Get(_roleId, "Apollo"));
        }

        [TestMethod]
        public void ErrorsTests()
        {
            DetachAllEntities();
            var controller = CreateRequest<ErrorsController>(HttpMethod.Put);

            var newError = new ErrorEntry
            {
                Business = "GotGrease?",
                Message = "There was an error!!",
                Url = "thisisafakeurl.com/notreal?true"
            };

            controller.Put(newError);
        }

        [TestMethod]
        public void LocationsTests()
        {
            DetachAllEntities();
            var locationId = CoreEntitiesContainer.Locations.First(l => l.BusinessAccountId == _gotGreaseId).Id;
            var clientId = CoreEntitiesContainer.Clients.First(c => c.BusinessAccountId == _gotGreaseId).Id;

            //Testing getting one location
            SimpleGetTest<LocationsController, Models.Location>(lc => lc.Get(_roleId, locationId, null, false, null));

            //Testing getting all locations for a client
            SimpleGetTest<LocationsController, Models.Location>(lc => lc.Get(_roleId, null, clientId, false, null));

            //Testing getting all depot locations for a business account
            SimpleGetTest<LocationsController, Models.Location>(lc => lc.Get(_roleId, null, null, true, null));

            //TODO uncomment when existing locations are included in search
            //var searchText = convertedLocation.AddressLineOne + ' ' + convertedLocation.AdminDistrictTwo + ' ' + convertedLocation.AdminDistrictOne + ' ' + convertedLocation.CountryCode;
            //var getResponseFromSearch = controller.GetAllLocations(_roleId, searchText).FirstOrDefault();
            //if (!Tools.AreObjectsEqual(getResponseFromSearch, convertedLocation, new[] { "Id", "Name", "AddressLineTwo" }))
            //    throw new Exception("Objects are not equal. Check output window for details");


            var getResponseFromSearch = SimpleGetTest<LocationsController, Models.Location>(lc => lc.Get(_roleId, null, null, false, "12414 english garden, 20171"))
                .FirstOrDefault();

            Assert.AreEqual("20171", getResponseFromSearch.ZipCode);
            Assert.AreEqual("Herndon", getResponseFromSearch.City);
            Assert.AreEqual("VA", getResponseFromSearch.State);
            Assert.IsTrue(getResponseFromSearch.Latitude.Contains("38.8981361"));
            Assert.IsTrue(getResponseFromSearch.Longitude.Contains("-77.3790054"));

            var newId = Guid.NewGuid();

            var newLocation = new Models.Location
            {
                Id = newId,
                Name = "New Location",
                AddressLineOne = "123 Fake Street",
                AddressLineTwo = "Apt ???",
                City = "Nowheresville",
                State = "Atlantis",
                ZipCode = "012345",
                CountryCode = "PO"
            };

            var controller = CreateRequest<LocationsController>(HttpMethod.Post);

            controller.Post(_roleId, newLocation);
            DetachAllEntities();
            var createdLocation = CoreEntitiesContainer.Locations.FirstOrDefault(l => l.Id == newId);

            Assert.IsNotNull(createdLocation);

            newLocation.CountryCode = "WK";
            newLocation.City = "I Have Changed";
            newLocation.AddressLineOne = "Not The Same as I Was";
            newLocation.AddressLineTwo = "Different";
            newLocation.ZipCode = "Not Even Postal Code";.

            controller.Put(_roleId, newLocation);
            DetachAllEntities();

            createdLocation = CoreEntitiesContainer.Locations.First(l => l.Id == newId);

            Assert.AreEqual("WK", createdLocation.CountryCode);
            Assert.AreEqual("I Have Changed", createdLocation.AdminDistrictTwo);
            Assert.AreEqual("Not The Same as I Was", createdLocation.AddressLineOne);
            Assert.AreEqual("Different", createdLocation.AddressLineTwo);
            Assert.AreEqual("Not Even Postal Code", createdLocation.PostalCode);
        }

        [TestMethod]
        public void RoutesTests()
        {
            //Testing deep and assigned
            SimpleGetTest<RoutesController, Models.Route>(r => r.Get(_roleId, DateTime.UtcNow.Date, true, true));

            //Testing not deep and not assigned
            SimpleGetTest<RoutesController, Models.Route>(r => r.Get(_roleId, DateTime.UtcNow.Date, false, false));

            //Testing not deep and assigned
            SimpleGetTest<RoutesController, Models.Route>(r => r.Get(_roleId, DateTime.UtcNow.Date, false, true));

            //Testing deep and not assigned
            SimpleGetTest<RoutesController, Models.Route>(r => r.Get(_roleId, DateTime.UtcNow.Date, true, false));
        }

        [TestMethod]
        public void SessionsTests()
        {
            DetachAllEntities();
            var headers = new Dictionary<string, string> { { "ops-details", "true" } };
            var controller = CreateRequest<SessionsController>(HttpMethod.Get, headers);

            var getResponseWithHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, getResponseWithHeader.StatusCode);

            //now test simple response
            controller = CreateRequest<SessionsController>(HttpMethod.Get);

            var getResponseWithoutHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.Accepted, getResponseWithoutHeader.StatusCode);
        }

        [TestMethod]
        public void ServicesTests()
        {
            var service = CoreEntitiesContainer.Services.Include(s => s.ServiceTemplate).Include(s => s.ServiceTemplate.Fields).First();
            var recurringService = CoreEntitiesContainer.RecurringServices.Include(rs => rs.Repeat).First();
            var serviceDate = recurringService.Repeat.StartDate;
            var serviceTemplate = CoreEntitiesContainer.ServiceTemplates.First(st => st.OwnerServiceProviderId != null);

            //Tests getting a service from an Id
            SimpleGetTest<ServicesController, Models.Service>(s => s.Get(service.Id, null, null, null));

            //Tests getting a service from a recurring service and a date
            SimpleGetTest<ServicesController, Models.Service>(s => s.Get(null, serviceDate, recurringService.Id, null));

            //Tests getting a service from a service template Id
            SimpleGetTest<ServicesController, Models.Service>(s => s.Get(null, serviceDate, null, serviceTemplate.Id));

            var controller = CreateRequest<ServicesController>(HttpMethod.Put);
            //fake generate a service by setting an Id and clearing the recurring service id
            var modelService = Models.Service.ConvertModel(service);
            modelService.Id = Guid.NewGuid();
            foreach (var field in modelService.Fields)
                field.Id = Guid.NewGuid();

            //Tests generating a service when one does not exist
            controller.Put(modelService);

            controller = CreateRequest<ServicesController>(HttpMethod.Delete);

            //Tests deleting a service
            controller.Delete(modelService);

            //Tests updating an existing service
            controller = CreateRequest<ServicesController>(HttpMethod.Put);

            modelService = Models.Service.ConvertModel(service);
            modelService.Name = "I have changed";

            controller.Put(modelService);
        }

        [TestMethod]
        public void ServiceHoldersTests()
        {
            var clientId = CoreEntitiesContainer.Clients.First().Id;
            var recurringServiceId = CoreEntitiesContainer.RecurringServices.First().Id;

            //Tests no service type and no context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests service type with no context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, "WVO Collection", null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests no service type with client context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, null, clientId, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests service type with client context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, "WVO Collection", null, clientId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests no service type with recurring service context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, null, null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests service type with recurring service context
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, "WVO Collection", null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31)));

            //Tests just returning the column headers
            SimpleGetTest<ServiceHoldersController, Dictionary<string, Object>>(sh => sh.Get(_roleId, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31), true));
        }

        [TestMethod]
        public void ServiceTemplatesTests()
        {
            SimpleGetTest<ServiceTemplatesController, Models.ServiceTemplate>(st => st.Get(_roleId));
        }

        [TestMethod]
        public void ResourceWithLastPointsTests()
        {
            SimpleGetTest<ResourceWithLastPointsController, Models.ResourceWithLastPoint>(st => st.Get(_roleId));
        }

        [TestMethod]
        public void RouteTasksTest()
        {
            DetachAllEntities();

            //update a route task's status to one that remove's it from a route
            var routeTask = CoreEntitiesContainer.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var unroutedStatus = CoreEntitiesContainer.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && ts.RemoveFromRoute);
            routeTask.TaskStatus = unroutedStatus;

            var controller = CreateRequest<RouteTasksController>(HttpMethod.Get);

            var putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is no longer in a route
            DetachAllEntities();
            var updatedRouteTask = CoreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNull(updatedRouteTask.RouteDestinationId);

            //update a route task's status to one that will not remove it from a route
            routeTask = CoreEntitiesContainer.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var routedStatus = CoreEntitiesContainer.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && !ts.RemoveFromRoute);
            routeTask.TaskStatus = routedStatus;

            putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is still in a route
            DetachAllEntities();
            updatedRouteTask = CoreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNotNull(updatedRouteTask.RouteDestinationId);
        }

        [TestMethod]
        public void TrackPointsTests()
        {
            //CoreEntitiesServerManagement.ClearHistoricalTrackPoints();
            //CoreEntitiesServerManagement.CreateHistoricalTrackPoints();

            DetachAllEntities();
            var date = DateTime.UtcNow.Date;

            var routeId = CoreEntitiesContainer.Routes.Where(r => r.Date == date && r.OwnerBusinessAccountId == _gotGreaseId).RandomItem().Id;

            SimpleGetTest<TrackPointsController, Models.TrackPoint>(tp => tp.Get(_roleId, routeId));

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

            var controller = CreateRequest<TrackPointsController>(HttpMethod.Post);

            controller.Post(_roleId, trackPoints.ToArray());
        }

        //Begin SQL function tests

        [TestMethod]
        public void DeleteBusinessAccount()
        {
            var provider = CoreEntitiesContainer.Parties.OfType<BusinessAccount>().First(ba => ba.Name != "FoundOPS");

            CoreEntitiesContainer.DeleteBusinessAccountBasedOnId(provider.Id);
        }

        [TestMethod]
        public void DeleteUserAccount()
        {
            var user = CoreEntitiesContainer.Parties.OfType<UserAccount>().First();

            CoreEntitiesContainer.DeleteUserAccountBasedOnId(user.Id);
        }

        [TestMethod]
        public void DeleteClient()
        {
            var client = CoreEntitiesContainer.Clients.First();

            CoreEntitiesContainer.DeleteClientBasedOnId(client.Id);
        }

        [TestMethod]
        public void DeleteFieldAndChildren()
        {
            var field = CoreEntitiesContainer.Fields.First(f => f.OwnerServiceTemplate.LevelInt == 0);

            CoreEntitiesContainer.DeleteFieldAndChildrenBasedOnFieldId(field.Id);
        }

        [TestMethod]
        public void DeleteLocation()
        {
            var location = CoreEntitiesContainer.Locations.First();

            CoreEntitiesContainer.DeleteLocationBasedOnId(location.Id);
        }

        [TestMethod]
        public void DeleteRecurringService()
        {
            var recurringService = CoreEntitiesContainer.RecurringServices.First();

            CoreEntitiesContainer.DeleteRecurringService(recurringService.Id);
        }

        [TestMethod]
        public void DeleteServiceTemplateAndChildrenBasedOnId()
        {
            var template = CoreEntitiesContainer.ServiceTemplates.First(st => st.LevelInt == 1);

            CoreEntitiesContainer.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId(template.Id);
        }

        [TestMethod]
        public void DeleteServiceTemapltesAndChildrenBasedOnContext()
        {
            //Client
            var serviceTemplate = CoreEntitiesContainer.ServiceTemplates.First(st => st.OwnerClientId != null);

            var deletedId = serviceTemplate.Id;

            CoreEntitiesContainer.DeleteServiceTemplatesAndChildrenBasedOnContextId(null, serviceTemplate.OwnerClientId);

            var badDelete = CoreEntitiesContainer.ServiceTemplates.FirstOrDefault(st => st.Id == deletedId || st.OwnerServiceTemplateId == deletedId);
            Assert.IsNull(badDelete);

            //finds a service template with an owner service provider that is not FoundOPS
            serviceTemplate = CoreEntitiesContainer.ServiceTemplates.First(st => st.OwnerServiceProviderId != null && st.OwnerServiceProviderId != new Guid("5606A728-B99F-4AA1-B0CD-0AB38A649000"));
            deletedId = serviceTemplate.Id;

            //Business Account
            CoreEntitiesContainer.DeleteServiceTemplatesAndChildrenBasedOnContextId(serviceTemplate.OwnerServiceProviderId, null);

            badDelete = CoreEntitiesContainer.ServiceTemplates.FirstOrDefault(st => st.Id == deletedId || st.OwnerServiceTemplateId == deletedId);
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
                parameters.Add("@clientIdContext", CoreEntitiesContainer.Clients.First().Id);
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
                parameters.Add("@recurringServiceIdContext", CoreEntitiesContainer.RecurringServices.First().Id);
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
            var data = CoreEntitiesContainer.GetFieldsInJavaScriptFormat(_gotGreaseId, "WVO Collection").ToArray();

            Assert.IsNotNull(data.FirstOrDefault());
        }

        [TestMethod]
        public void ResourcesWithLastPoint()
        {
            var data = CoreEntitiesContainer.GetResourcesWithLatestPoint(_gotGreaseId, DateTime.UtcNow.Date).ToArray();

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
                        CoreEntitiesContainer.DetachExistingAndAttach(location);
                        routeTask.Location = location;

                        if (region != null)
                        {
                            CoreEntitiesContainer.DetachExistingAndAttach(region);
                            routeTask.Location.Region = region;
                        }
                    }

                    if (client != null)
                    {
                        CoreEntitiesContainer.DetachExistingAndAttach(client);
                        routeTask.Client = client;
                    }

                    if (taskStatus != null)
                    {
                        CoreEntitiesContainer.DetachExistingAndAttach(taskStatus);
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
            var template = CoreEntitiesContainer.ServiceTemplates.Where(st => st.LevelInt == 1 && st.Name == "WVO Collection" && st.OwnerServiceProviderId == _gotGreaseId).Include(st => st.Fields).First();

            //Numeric field
            var propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewNumericField());
            Assert.AreEqual(true, propagationSuccess);

            //CheckList fields
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewCheckListField());
            Assert.AreEqual(true, propagationSuccess);

            //CheckBox fields
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewCheckBoxField());
            Assert.AreEqual(true, propagationSuccess);

            //ComboBox fields
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewComboBoxField());
            Assert.AreEqual(true, propagationSuccess);

            //Location fields
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewLocationField());
            Assert.AreEqual(true, propagationSuccess);

            //Single line TextBox field
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewSmallTextBoxField());
            Assert.AreEqual(true, propagationSuccess);

            //Multi-line TextBox field
            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewLargeTextBoxField());
            Assert.AreEqual(true, propagationSuccess);

            propagationSuccess = PropagateFieldToTemplateAndChildren(template, NewDateTimeField());
            Assert.AreEqual(true, propagationSuccess);
        }

        /// <summary>
        /// Adds the new field to the Service Template and then propagates it down
        /// After propagation completes, run a stored procedure to check to be sure that it propagated to all children Service Templates
        /// </summary>
        /// <param name="template">The top level Service Template the field was added to</param>
        /// <param name="field">The field to add</param>
        /// <returns>If propagation was successful, return true. Else, return false</returns>
        private bool PropagateFieldToTemplateAndChildren(ServiceTemplate template, Field field)
        {
            template.Fields.Add(field);

            CoreEntitiesContainer.SaveChanges();

            CoreEntitiesContainer.PropagateNewFields(field.Id);

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceTemplateId", template.Id);
                parameters.Add("@fieldName", field.Name);

                //A result of '0' means that the propagation was successfull
                //'1' means an error occurred
                var result = conn.Query<int>("TestFieldPropagationSuccess", parameters, commandType: CommandType.StoredProcedure).First();

                conn.Close();

                return result != 1;
            }
        }

        [TestMethod]
        public void PropagateServiceTemplateNameChanges()
        {
            var serviceTemplate = CoreEntitiesContainer.ServiceTemplates.Where(st => st.LevelInt == 1 && st.Name == "WVO Collection" && st.OwnerServiceProviderId == _gotGreaseId).Include(st => st.Fields).First();
            var oldName = serviceTemplate.Name;

            serviceTemplate.Name = "This Name has changed!!";

            //Save and propagate the name change
            CoreEntitiesContainer.SaveChanges();
            CoreEntitiesContainer.PropagateNameChange(serviceTemplate.Id);

            //Use the function TestServiceTemplateNamePropagationSuccess to check that all Service Templates and Routes had their names changed
            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceTemplateId", serviceTemplate.Id);
                parameters.Add("@oldName", oldName);

                var result = conn.Query<int>("TestServiceTemplateNamePropagationSuccess", parameters, commandType: CommandType.StoredProcedure).First();

                conn.Close();

                Assert.AreEqual(0, result);
            }
        }

        [TestMethod]
        public void PropagateNewServiceTemplateToClients()
        {
            var foundOpsServiceTemplate = new ServiceTemplate
            {
                Id = Guid.NewGuid(),
                Name = "This is New!!",
                ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                OwnerServiceProviderId = _foundOpsId
            };

            //Add Fields
            foundOpsServiceTemplate.Fields.Add(NewCheckBoxField());
            foundOpsServiceTemplate.Fields.Add(NewCheckListField());
            foundOpsServiceTemplate.Fields.Add(NewComboBoxField());
            foundOpsServiceTemplate.Fields.Add(NewDateTimeField());
            foundOpsServiceTemplate.Fields.Add(NewLargeTextBoxField());
            foundOpsServiceTemplate.Fields.Add(NewSmallTextBoxField());
            foundOpsServiceTemplate.Fields.Add(NewNumericField());
            foundOpsServiceTemplate.Fields.Add(NewLocationField());

            //Make a child and set it to be on GotGrease?
            var serviceTemplate = foundOpsServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
            serviceTemplate.OwnerServiceProviderId = _gotGreaseId;

            //Save and propagate the new template
            CoreEntitiesContainer.ServiceTemplates.AddObject(serviceTemplate);
            CoreEntitiesContainer.SaveChanges();
            CoreEntitiesContainer.PropagateNewServiceTemplateToClients(serviceTemplate.Id);

            //Use the function TestServiceTemplateNamePropagationSuccess to check that all Service Templates and Routes had their names changed
            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceTemplateId", serviceTemplate.Id);

                var result = conn.Query<int>("TestNewServiceTemplatePropagationSuccess", parameters, commandType: CommandType.StoredProcedure).First();

                conn.Close();

                Assert.AreEqual(0, result);
            }

            var childTemplate = CoreEntitiesContainer.ServiceTemplates.Where(st => st.LevelInt == ((int)ServiceTemplateLevel.ClientDefined) && st.OwnerServiceTemplateId == serviceTemplate.Id).Include(st => st.Fields).First();

            var compare = new CompareObjects { CompareChildren = false };

            foreach (var field in serviceTemplate.Fields)
            {
                var childField = childTemplate.Fields.First(st => st.Name == field.Name);

                compare.ElementsToIgnore.Add("Id");
                compare.ElementsToIgnore.Add("ServiceTemplateId");
                compare.ElementsToIgnore.Add("ParentFieldId");

                compare.Compare(field, childField);

                Assert.AreEqual(0, compare.Differences.Count);
            }
        }
    }
}
