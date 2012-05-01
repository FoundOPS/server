using System.ServiceModel.DomainServices.Server;
using FoundOps.Common.Composite;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
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
                .Include("RouteDestinations").Include("RouteDestinations.Location").Include("RouteDestinations.RouteTasks").Include("RouteDestinations.RouteTasks.Client");

            return routes.OrderBy(r=>r.Name);
        }

        /// <summary>
        /// Gets the route details.
        /// It includes the Vehicles, Technicians and Technicians.OwnedPerson.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="routeId">The employee id.</param>
        public Route GetRouteDetails(Guid roleId, Guid routeId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var route = ObjectContext.Routes.Where(r => r.OwnerBusinessAccount.Id == businessForRole.Id && r.Id == routeId)
                    .Include("Vehicles").Include("Technicians").First();

            //Force load technician's OwnedPerson
            (from employee in route.Technicians
             join person in ObjectContext.Parties.OfType<Person>()
                 on employee.Id equals person.Id
             select new { employee, person }).ToArray();

            return route;
        }

        /// <summary>
        /// Get the route's RouteTask.Service.ServiceTemplates and Fields.
        /// It includes the OwnerClient, Fields, OptionsFields w Options, LocationFields w Location, (TODO) Invoices
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="routeId">The route id.</param>
        public IQueryable<ServiceTemplate> GetRouteServiceTemplates(Guid roleId, Guid routeId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var serviceTemplateIds = this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessForRole.Id && r.Id == routeId).Include("RouteDestinations.RouteTasks")
                                     .SelectMany(r => r.RouteDestinations.SelectMany(rd => rd.RouteTasks.Select(rt => rt.ServiceId))).Where(sid => sid != null).Select(sid => sid.Value).ToArray();

            var templatesWithDetails = from serviceTemplate in GetServiceTemplatesForServiceProvider(roleId, (int)ServiceTemplateLevel.ServiceDefined).Where(st => serviceTemplateIds.Contains(st.Id))
                                       from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                                       from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                                       select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations };

            return templatesWithDetails.Select(t => t.serviceTemplate).Distinct().OrderBy(st => st.Name);
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

            if (vehicleId != Guid.Empty)
                routes = routes.Where(r => r.Vehicles.Any(v => v.Id == vehicleId));

            if (employeeId != Guid.Empty)
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
            foreach (var routeDestination in route.RouteDestinations.ToArray())
                DeleteRouteDestination(routeDestination);

            this.ObjectContext.Routes.DeleteObject(route);
        }

        #endregion

        #region RouteDestination

        public IEnumerable<RouteDestination> GetRouteDestinations()
        {
            return this.ObjectContext.RouteDestinations;
        }

        /// <summary>
        /// Gets the RouteDestinations Details
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="destinationId">The destination id.</param>
        public RouteDestination GetRouteDestinationDetails(Guid roleId, Guid destinationId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var routeDestination = ObjectContext.RouteDestinations.Where(rd => rd.RouteTasks.FirstOrDefault().BusinessAccountId == businessForRole.Id && rd.Id == destinationId)
                .Include("Client.OwnedParty.ContactInfoSet").FirstOrDefault();

            return routeDestination;
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

        /// <summary>
        /// Returns the scheduled RouteTasks for the day based on the ServiceProvider that are not in a route.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="serviceDate">The service date.</param>
        [Query]
        public IQueryable<TaskHolder> GetUnroutedServices(Guid roleId, DateTime serviceDate)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var unroutedServicesForDate = ObjectContext.GetUnroutedServicesForDate(businessForRole.Id, serviceDate);

            return unroutedServicesForDate.AsQueryable();
        }

        /// <summary>
        /// Gets the route task details.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="routeTaskId">The route task id.</param>
        public RouteTask GetRouteTaskDetails(Guid roleId, Guid routeTaskId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var routeTask = ObjectContext.RouteTasks.Where(rt => rt.BusinessAccountId == businessForRole.Id && rt.Id == routeTaskId)
                .Include("Client").Include("Client.OwnedParty").Include("Client.OwnedParty.ContactInfoSet").Include("Location").Include("Service").FirstOrDefault();

            //Load the ServiceTemplate for the Service
            if (routeTask.ServiceId.HasValue)
                GetServiceTemplateDetailsForRole(roleId, routeTask.ServiceId.Value);

            return routeTask;
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
