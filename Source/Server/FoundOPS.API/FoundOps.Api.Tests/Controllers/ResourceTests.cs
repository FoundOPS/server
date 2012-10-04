using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Models;
using System.Data;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace FoundOps.Api.Tests.Controllers
{
    [TestClass]
    public class ResourceTests
    {
        protected readonly Core.Models.CoreEntities.CoreEntitiesContainer CoreEntitiesContainer;
        private readonly Guid _roleId;
        private readonly Guid _gotGreaseId = new Guid("8528E50D-E2B9-4779-9B29-759DBEA53B61");

        public ResourceTests()
        {
            CoreEntitiesContainer = new Core.Models.CoreEntities.CoreEntitiesContainer();
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
        public void ErrorsTests()
        {
            DetachAllEntities();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");

            var controller = new ErrorsController { Request = request };

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

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new LocationsController { Request = request };

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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new RoutesController { Request = request };

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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Add("ops-details", "true");
            //this is necessary to work http://stackoverflow.com/questions/11053598/how-to-mock-the-createresponset-extension-method-on-httprequestmessage
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var controller = new SessionsController { Request = request };

            var getResponseWithHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, getResponseWithHeader.StatusCode);

            //now test simple response
            request.Headers.Remove("ops-details");

            controller.Request = request;

            var getResponseWithoutHeader = controller.Get();

            Assert.AreEqual(HttpStatusCode.Accepted, getResponseWithoutHeader.StatusCode);

            Debug.WriteLine("All Sessions Controller Tests Passed");
        }

        [TestMethod]
        public void RouteTasksTest()
        {
            DetachAllEntities();

            //update a route task's status to one that remove's it from a route
            var routeTask = CoreEntitiesContainer.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var unroutedStatus = CoreEntitiesContainer.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && ts.RemoveFromRoute);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var controller = new RouteTasksController { Request = request };
            routeTask.TaskStatus = unroutedStatus;

            var putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is no longer in a route
            DetachAllEntities();
            var updatedRouteTask = CoreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNull(updatedRouteTask.RouteDestinationId);

            routeTask = CoreEntitiesContainer.RouteTasks.First(rt => rt.RouteDestinationId.HasValue && rt.BusinessAccountId == _gotGreaseId);
            var routedStatus = CoreEntitiesContainer.TaskStatuses.First(ts => ts.BusinessAccountId == _gotGreaseId && !ts.RemoveFromRoute);

            putResponse = controller.Put(RouteTask.ConvertModel(routeTask));
            //check it worked
            Assert.AreEqual(HttpStatusCode.Accepted, putResponse.StatusCode);

            //make sure the route task is no longer in a route
            DetachAllEntities();
            updatedRouteTask = CoreEntitiesContainer.RouteTasks.Where(rt => rt.Id == routeTask.Id).Include(rt => rt.RouteDestination).First();
            Assert.IsNotNull(updatedRouteTask.RouteDestinationId);

            Debug.WriteLine("All RouteTasks Controller Tests Passed");
        }
    }
}
