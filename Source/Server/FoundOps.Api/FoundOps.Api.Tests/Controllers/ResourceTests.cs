using System.Collections.Generic;
using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Models;
using System.Data;
using System.Data.Entity;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using RouteTask = FoundOps.Api.Models.RouteTask;

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
            if(headers != null)
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
        protected readonly CoreEntitiesContainer CoreEntitiesContainer;
        private readonly Guid _roleId;
        private readonly Guid _gotGreaseId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");

        public ResourceTests()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;

            //Admin role on GotGrease?
            _roleId = CoreEntitiesContainer.Roles.First(r => r.OwnerBusinessAccountId == _gotGreaseId && r.RoleTypeInt == (int)Core.Models.CoreEntities.RoleType.Administrator).Id;
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
            var controller = TestTools.CreateRequest<ClientsController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleId, "Apollo");

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
            var locationId = CoreEntitiesContainer.Locations.First(l => l.BusinessAccountId == _gotGreaseId).Id;
            var clientId = CoreEntitiesContainer.Clients.First(c => c.BusinessAccountId == _gotGreaseId).Id;

            var controller = TestTools.CreateRequest<LocationsController>(HttpMethod.Get);

            //Testing getting one location
            var getResponse = controller.Get(_roleId, locationId, null, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing getting all locations for a client
            getResponse = controller.Get(_roleId, null, clientId, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing getting all depot locations for a business account
            getResponse = controller.Get(_roleId, null, null, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Locations Controller Tests Passed");
        }

        [TestMethod]
        public void RoutesTests()
        {
            DetachAllEntities();
            var controller = TestTools.CreateRequest<RoutesController>(HttpMethod.Get);

            //Testing deep and assigned
            var getResponse = controller.Get(_roleId, DateTime.UtcNow.Date, true, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing not deep and not assigned
            getResponse = controller.Get(_roleId, DateTime.UtcNow.Date, false, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing not deep and assigned
            getResponse = controller.Get(_roleId, DateTime.UtcNow.Date, false, true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Testing deep and not assigned
            getResponse = controller.Get(_roleId, DateTime.UtcNow.Date, true, false);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Routes Controller Tests Passed");
        }

        [TestMethod]
        public void SessionsTests()
        {
            DetachAllEntities();
            var headers = new Dictionary<string, string> {{"ops-details", "true"}};
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
            var service = CoreEntitiesContainer.Services.Include(s => s.ServiceTemplate).Include(s => s.ServiceTemplate.Fields).First();
            var recurringService = CoreEntitiesContainer.RecurringServices.Include(rs => rs.Repeat).First();
            var serviceDate = recurringService.Repeat.StartDate;
            var serviceTemplate = CoreEntitiesContainer.ServiceTemplates.First(st => st.OwnerServiceProviderId != null);

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
            var clientId = CoreEntitiesContainer.Clients.First().Id;
            var recurringServiceId = CoreEntitiesContainer.RecurringServices.First().Id;

            var controller = TestTools.CreateRequest<ServiceHoldersController>(HttpMethod.Get);

            //Tests no service type and no context
            var getResponse = controller.Get(_roleId, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with no context
            getResponse = controller.Get(_roleId, "WVO Collection", null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests no service type with client context
            getResponse = controller.Get(_roleId, null, clientId, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with client context
            getResponse = controller.Get(_roleId, "WVO Collection", null, clientId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests no service type with recurring service context
            getResponse = controller.Get(_roleId, null, null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests service type with recurring service context
            getResponse = controller.Get(_roleId, "WVO Collection", null, recurringServiceId, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31));
            Assert.IsNotNull(getResponse.FirstOrDefault());

            //Tests just returning the column headers
            getResponse = controller.Get(_roleId, null, null, null, new DateTime(2012, 9, 1), new DateTime(2012, 10, 31), true);
            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Service Holders Controller Tests Passed");
        }

        [TestMethod]
        public void ServiceTemplatesTests()
        {
            var controller = TestTools.CreateRequest<ServiceTemplatesController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleId);

            Assert.IsNotNull(getResponse.FirstOrDefault());

            Debug.WriteLine("All Service Templates Controller Tests Passed");
        }

        [TestMethod]
        public void ResourceWithLastPointsTests()
        {
            var controller = TestTools.CreateRequest<ResourceWithLastPointsController>(HttpMethod.Get);

            var getResponse = controller.Get(_roleId);
            Assert.IsNotNull(getResponse.FirstOrDefault());
        }

        [TestMethod]
        public void RouteTasksTest()
        {
            DetachAllEntities();

            //update a route task's status to one that remove's it from a route
            var routeTask = CoreEntitiesContainer.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var unroutedStatus = CoreEntitiesContainer.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && ts.RemoveFromRoute);
            routeTask.TaskStatus = unroutedStatus;

            var controller = TestTools.CreateRequest<RouteTasksController>(HttpMethod.Get);

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
            var routeId = CoreEntitiesContainer.Routes.First(r => r.Date == date).Id;

            var getResponse = controller.Get(_roleId, routeId);

            Assert.IsNotNull(getResponse);

            var trackPoints = new List<TrackPoint>();
            var fakeRouteId = Guid.NewGuid();

            //Create the test TrackPoints with random numbers as properties
            for (int i = 0; i < 10; i++)
            {
                var random = new Random();

                var trackPoint = new TrackPoint
                {
                    Id = Guid.NewGuid(),
                    Heading = random.Next(0, 359),
                    Latitude = random.Next(-90, 90),
                    Longitude = random.Next(-180, 180),
                    CollectedTimeStamp = DateTime.Now.AddSeconds(10 * i),
                    Speed = random.Next(20, 60),
                    RouteId = fakeRouteId,
                    Source = "Testing",
                    Accuracy = 1
                };

                trackPoints.Add(trackPoint);
            }

            controller = TestTools.CreateRequest<TrackPointsController>(HttpMethod.Post);

            controller.Post(_roleId, trackPoints.ToArray());
        }
    }
}
