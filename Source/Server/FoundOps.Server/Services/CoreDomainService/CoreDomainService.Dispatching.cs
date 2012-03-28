using System.ServiceModel.DomainServices.Server;
using FoundOps.Common.Composite;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.Server.Authentication;
using Route = FoundOps.Core.Models.CoreEntities.Route;
using RouteDestination = FoundOps.Core.Models.CoreEntities.RouteDestination;
using RouteTask = FoundOps.Core.Models.CoreEntities.RouteTask;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any dispatching entities:
    /// Routes, RouteDestinations and RouteTasks
    /// </summary>
    public partial class CoreDomainService
    {
        #region Route

        /// <summary>
        /// Gets the routes for a service provider on a date.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="dateOfRoutes">The date of the routes.</param>
        public IQueryable<Route> GetRoutesForServiceProviderOnDay(Guid roleId, DateTime dateOfRoutes)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var routesDateOnly = dateOfRoutes.Date;

            var routes =
                ((ObjectQuery<Route>)this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessForRole.Id && r.Date == routesDateOnly))
                .Include("RouteDestinations").Include("RouteDestinations.Location").Include("RouteDestinations.RouteTasks")
                .Include("RouteDestinations.RouteTasks.Location").Include("RouteDestinations.RouteTasks.Client").Include("RouteDestinations.RouteTasks.Service.ServiceTemplate")
                .Include("Technicians").Include("Vehicles");

            return routes;

        }

        /// <summary>
        /// Gets the routes for a service provider.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="employeeId">(Optional) filter routes that have this employee.</param>
        /// <param name="vehicleId">(Optional) filter routes that have this vehicle.</param>
        public IQueryable<Route> GetRouteLogForServiceProvider(Guid roleId, Guid employeeId, Guid vehicleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var routes = this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessForRole.Id)
                          .Include("Vehicles").Include("Technicians").Include("RouteDestinations");

            if(vehicleId!=Guid.Empty)
                routes = routes.Where(r => r.Vehicles.Any(v => v.Id == vehicleId));

            if(employeeId!=Guid.Empty)
                routes = routes.Where(r => r.Technicians.Any(t => t.Id == employeeId));

            //TODO: optimize this
            //See http://stackoverflow.com/questions/5699583/when-how-does-a-ria-services-query-get-added-to-ef-expression-tree
            //http://stackoverflow.com/questions/8358681/ria-services-domainservice-query-with-ef-projection-that-calls-method-and-still
            //Force load OwnedPerson
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            (from t in routes.SelectMany(r => r.Technicians)
             join p in this.ObjectContext.Parties
                 on t.Id equals p.Id
             select p).ToArray();

            return routes.OrderBy(r => r.Date);
        }

        public void InsertRoute(Route route)
        {
            route.Date = route.Date.Date; //Remove any time
            route.StartTime = route.StartTime.SetDate(route.Date); //Set to same date as route.Date
            route.EndTime = route.EndTime.SetDate(route.Date); //Set to same date as route.Date

            if ((route.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(route, EntityState.Added);
            else
                this.ObjectContext.Routes.AddObject(route);
        }

        public void UpdateRoute(Route route)
        {
            this.ObjectContext.Routes.AttachAsModified(route);
        }

        /// <summary>
        /// Deletes the Route and it's RouteDestinations.
        /// It does not delete the route destination's tasks.
        /// Although they are automatically removed through cascading.
        /// </summary>
        /// <param name="route"></param>
        public void DeleteRoute(Route route)
        {
            this.ObjectContext.DetachExistingAndAttach(route);

            route.RouteDestinations.Load();
            foreach(var routeDestination in route.RouteDestinations.ToArray())
                DeleteRouteDestination(routeDestination);

            this.ObjectContext.Routes.DeleteObject(route);
        }

        #endregion

        #region RouteDestination

        public IEnumerable<RouteDestination> GetRouteDestinations()
        {
            return this.ObjectContext.RouteDestinations;
        }

        public void InsertRouteDestination(RouteDestination routeDestination)
        {
            if ((routeDestination.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeDestination, EntityState.Added);
            else
                this.ObjectContext.RouteDestinations.AddObject(routeDestination);
        }

        public void UpdateRouteDestination(RouteDestination currentRouteDestination)
        {
            this.ObjectContext.RouteDestinations.AttachAsModified(currentRouteDestination);
        }

        /// <summary>
        /// Deletes the route destination. It does not delete the route tasks.
        /// </summary>
        /// <param name="routeDestination">The route destination to delete.</param>
        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            this.ObjectContext.DetachExistingAndAttach(routeDestination);

            routeDestination.RouteTasks.Load();
            routeDestination.RouteTasks.Clear();

            this.ObjectContext.RouteDestinations.DeleteObject(routeDestination);
        }

        #endregion

        #region RouteTask
        [Query]
        public IQueryable<TaskHolder>  GetUnroutedServices (Guid roleId, DateTime serviceDate) 
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var serviceProviderContextId = businessForRole.Id; 
 
            return ObjectContext.GetUnroutedServicesForDate(serviceProviderContextId, serviceDate).AsQueryable();
        }

        private static IEnumerable<RouteTask> GenerateRouteTasksFromServices(IEnumerable<Service> services, BusinessAccount businessAccount)
        {
            var newRouteTasks = services.Select(service =>
            {
                var individualGeneratedRouteTaskForService = new RouteTask
                {
                    GeneratedOnServer = true,
                    Date = service.ServiceDate,
                    Service = service,
                    Client = service.Client,
                    BusinessAccountId = businessAccount.Id,
                    Name = service.ServiceTemplate.Name,
                    Location = service.ServiceTemplate.GetDestination()
                };
                return individualGeneratedRouteTaskForService;
            });

            return newRouteTasks;
        }

        //do logic that gets all the services for the day based on recurring services, then for each create new task. Then do union with what is currently done.
        public IEnumerable<RouteTask> GetUnroutedRouteTasks(Guid roleId, DateTime selectedDate)
        {
            var serviceDate = selectedDate.Date; //Remove time aspect

            var businessAccount = ObjectContext.BusinessAccountOwnerOfRole(roleId);

            //Get all the existing services for the day => later to be unioned with generatedServices
            var existingServicesQuery = ((ObjectQuery<Service>)this.ObjectContext.Services.Where(service => service.ServiceDate == serviceDate && service.ServiceProviderId == businessAccount.Id))
                .Include("ServiceTemplate").Include("ServiceTemplate.Fields").Include("RouteTasks").ToList();

            //Force load existingServices' LocationsFields's value (for Destination)
            (from es in existingServicesQuery
             join st in this.ObjectContext.ServiceTemplates
                 on es.Id equals st.Id
             from f in st.Fields
             from lf in st.Fields.OfType<LocationField>()
             select new { st, f, lf.Value })
            .ToArray();

            IEnumerable<Service> existingServices = existingServicesQuery.ToList();

            var recurringServicesForDate = RecurringServicesForDate(serviceDate, businessAccount);

            //Generate services for the day from RecurringServices for all recurring services without existing services
            var generatedServices = recurringServicesForDate.Where(rs => existingServices.All(s => s.RecurringServiceId != rs.Id)).Select(rs =>
            {
                var individualGeneratedService = new Service
                {
                    Generated = true,
                    Client = rs.Client,
                    ServiceTemplate = rs.ServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                    ServiceProviderId = businessAccount.Id,
                    ServiceDate = serviceDate,
                    RecurringServiceParent = rs
                };
                return individualGeneratedService;
            }).ToList();

            var unroutedExistingServices = existingServices.Where(s => s.RouteTasks.All(rt => rt.RouteDestinationId == null));

            var unroutedServices = unroutedExistingServices.Union(generatedServices);

            //Get the existing unrouted route tasks for the day
            var existingUnroutedRouteTasks = ((ObjectQuery<RouteTask>)this.ObjectContext.RouteTasks.Where(
                    routeTask =>
                    routeTask.BusinessAccountId == businessAccount.Id && routeTask.Date == serviceDate &&
                    (routeTask.RouteDestinationId == null || routeTask.RouteDestinationId == Guid.Empty))).Include("Client").Include("Location").Include("Service");

            //For each service in unrouted services for the day, create a route task 
            var generatedRouteTasks = GenerateRouteTasksFromServices(unroutedServices, businessAccount);

            //Remove preexisting route tasks
            generatedRouteTasks = generatedRouteTasks.Where(rt => !existingUnroutedRouteTasks.Any(unrt => rt.ServiceId == unrt.ServiceId));

            //Combine unroutedRouteTasks and generatedRouteTasksFromRecurringServices
            var allUnroutedRouteTasks = Enumerable.Union(existingUnroutedRouteTasks, generatedRouteTasks);

            return allUnroutedRouteTasks;
        }

        public void InsertRouteTask(RouteTask routeTask)
        {
            routeTask.Date = routeTask.Date.Date; //Remove any time

            if ((routeTask.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeTask, EntityState.Added);
            else
                this.ObjectContext.RouteTasks.AddObject(routeTask);
        }

        public void UpdateRouteTask(RouteTask currentRouteTask)
        {
            this.ObjectContext.RouteTasks.AttachAsModified(currentRouteTask);
        }

        public void DeleteRouteTask(RouteTask routeTask)
        {
            this.ObjectContext.DetachExistingAndAttach(routeTask);
            this.ObjectContext.RouteTasks.DeleteObject(routeTask);
        }

        #endregion
    }
}
