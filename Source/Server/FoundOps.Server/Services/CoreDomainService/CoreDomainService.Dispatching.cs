using System.Data.SqlClient;
using System.ServiceModel.DomainServices.Server;
using Dapper;
using FoundOps.Common.Composite;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Route = FoundOps.Core.Models.CoreEntities.Route;
using RouteDestination = FoundOps.Core.Models.CoreEntities.RouteDestination;
using RouteTask = FoundOps.Core.Models.CoreEntities.RouteTask;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        /// <param name="date">The date of the routes.</param>
        public IQueryable<Route> GetRoutesForServiceProviderOnDay(Guid roleId, DateTime date)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            date = date.Date;

            var routes =
                this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Date == date)
                .Include(r => r.RouteDestinations).Include("RouteDestinations.Location").Include("RouteDestinations.RouteTasks").Include("RouteDestinations.RouteTasks.Client");

            return routes.OrderBy(r => r.Name);
        }

        /// <summary>
        /// Gets the route details.
        /// It includes the Vehicles, Employees and Employees.OwnedPerson.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="routeId">The employee id.</param>
        public Route GetRouteDetails(Guid roleId, Guid routeId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var route = ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Id == routeId)
                    .Include(r => r.Vehicles).Include(r => r.Employees).FirstOrDefault();

            if (route == null)
                return null;

            //Force load technician's OwnedPerson
            (from employee in route.Employees
             join user in ObjectContext.Parties.OfType<UserAccount>()
                 on employee.Id equals user.Id
             select new { employee, user }).ToArray();

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
            var businessAccount = ObjectContext.Owner(roleId).First();

            var serviceTemplateIds = this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessAccount.Id && r.Id == routeId).Include("RouteDestinations.RouteTasks")
                                     .SelectMany(r => r.RouteDestinations.SelectMany(rd => rd.RouteTasks.Select(rt => rt.ServiceId))).Where(sid => sid != null).Select(sid => sid.Value).ToArray();

            var serviceTemplates =
                serviceTemplateIds.Select(
                    serviceTemplateId =>
                    HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, serviceTemplateId, null, null, null).First()).ToList();

            return serviceTemplates.AsQueryable().OrderBy(st => st.Name);
        }

        /// <summary>
        /// Gets the routes for a service provider.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="employeeId">(Optional) filter routes that have this employee.</param>
        /// <param name="vehicleId">(Optional) filter routes that have this vehicle.</param>
        public IQueryable<Route> GetRouteLogForServiceProvider(Guid roleId, Guid employeeId, Guid vehicleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var routes = this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessAccount.Id)
                          .Include(r => r.Vehicles).Include(r => r.Employees).Include(r => r.RouteDestinations);

            if (vehicleId != Guid.Empty)
                routes = routes.Where(r => r.Vehicles.Any(v => v.Id == vehicleId));

            if (employeeId != Guid.Empty)
                routes = routes.Where(r => r.Employees.Any(t => t.Id == employeeId));

            //TODO: optimize this
            //See http://stackoverflow.com/questions/5699583/when-how-does-a-ria-services-query-get-added-to-ef-expression-tree
            //http://stackoverflow.com/questions/8358681/ria-services-domainservice-query-with-ef-projection-that-calls-method-and-still
            //Force load OwnedPerson
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            (from t in routes.SelectMany(r => r.Employees)
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

            //route.Employees.Load();
            //route.Employees.Clear();

            //route.Vehicles.Load();
            //route.Vehicles.Clear();

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
            var businessAccount = ObjectContext.Owner(roleId).First();

            var routeDestination = ObjectContext.RouteDestinations.Where(rd => rd.RouteTasks.FirstOrDefault().BusinessAccountId == businessAccount.Id && rd.Id == destinationId)
                .Include(rd => rd.Client.ContactInfoSet).FirstOrDefault();

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

            if (routeDestination.RouteTasks.Any())
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
        public IQueryable<RouteTask> GetUnroutedServices(Guid roleId, DateTime serviceDate)
        {
            var businessForRole = ObjectContext.Owner(roleId).FirstOrDefault();

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@serviceProviderIdContext", businessForRole.Id);
                parameters.Add("@serviceDate", serviceDate);

                //Calls a stored procedure that will find any Services scheduled for today and create a routetask for them if one doesnt exist
                //Then it will return all RouteTasks that are not in a route joined with their Locations, Location.Regions and Clients
                //Dapper will then map the output table to RouteTasks, RouteTasks.Location, RouteTasks.Location.Region and RouteTasks.Client
                //While that is being mapped we also attach the Client, Location and Region to the objectContext
                var data = conn.Query<RouteTask, Location, Region, Client, TaskStatus, RouteTask>("sp_GetUnroutedServicesForDate", (routeTask, location, region, client, taskStatus) =>
                {
                    if (location != null)
                    {
                        this.ObjectContext.DetachExistingAndAttach(location);
                        routeTask.Location = location;

                        if (region != null)
                        {
                            this.ObjectContext.DetachExistingAndAttach(region);
                            routeTask.Location.Region = region;
                        }
                    }

                    if (client != null)
                    {
                        this.ObjectContext.DetachExistingAndAttach(client);
                        routeTask.Client = client;
                    }

                    if (taskStatus != null)
                    {
                        this.ObjectContext.DetachExistingAndAttach(taskStatus);
                        routeTask.TaskStatus = taskStatus;
                    }

                    return routeTask;
                }, parameters, commandType: CommandType.StoredProcedure);

                conn.Close();

                return data.AsQueryable();
            }
        }

        /// <summary>
        /// Gets the route task details.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="routeTaskId">The route task id.</param>
        public RouteTask GetRouteTaskDetails(Guid roleId, Guid routeTaskId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var routeTask = ObjectContext.RouteTasks.Where(rt => rt.BusinessAccountId == businessAccount.Id && rt.Id == routeTaskId)
                .Include(rt => rt.Client).Include(rt => rt.Client.ContactInfoSet).Include(rt => rt.Location).Include(rt => rt.Service).FirstOrDefault();

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

        #region TaskStatus

        public IQueryable<TaskStatus> GetTaskStatus()
        {
            throw new NotSupportedException("Exists solely to generate TaskStatus' in the clients data project");
        }

        public void InsertTaskStatus(TaskStatus taskStatus)
        {
            if ((taskStatus.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(taskStatus, EntityState.Added);
            else
                this.ObjectContext.TaskStatuses.AddObject(taskStatus);
        }

        public void UpdateTaskStatus(TaskStatus taskStatus)
        {
            this.ObjectContext.TaskStatuses.AttachAsModified(taskStatus);
        }

        public void DeleteTaskStatus(TaskStatus taskStatus)
        {
            this.ObjectContext.DetachExistingAndAttach(taskStatus);
            this.ObjectContext.TaskStatuses.DeleteObject(taskStatus);
        }

        #endregion
    }
}
