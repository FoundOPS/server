﻿using System;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Collections.Generic;
using FoundOps.Common.Composite;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using Route = FoundOps.Core.Models.CoreEntities.Route;
using System.ServiceModel.DomainServices.EntityFramework;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using RouteTask = FoundOps.Core.Models.CoreEntities.RouteTask;
using RouteDestination = FoundOps.Core.Models.CoreEntities.RouteDestination;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any dispatching entities:
    /// Routes, RouteDestinations and RouteTasks
    /// </summary>
    public partial class CoreDomainService
    {
        #region Route

        public IQueryable<Route> GetRoutesForServiceProviderOnDay(Guid roleId, DateTime dateOfRoutes)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            var routesDateOnly = dateOfRoutes.Date;

            var routes =
                ((ObjectQuery<Route>)this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessForRole.Id && r.Date == routesDateOnly))
                .Include("RouteDestinations").Include("RouteDestinations.Location").Include("RouteDestinations.RouteTasks")
                .Include("RouteDestinations.RouteTasks.Location").Include("RouteDestinations.RouteTasks.Client").Include("RouteDestinations.RouteTasks.Service.ServiceTemplate")
                .Include("Technicians").Include("Vehicles");

            return routes;

        }

        public IQueryable<Route> GetRouteLogForServiceProvider(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessForRole(roleId);

            var routes = ((ObjectQuery<Route>)
                          this.ObjectContext.Routes.Where(r => r.OwnerBusinessAccountId == businessForRole.Id))
                          .Include("Vehicles").Include("Technicians").Include("RouteDestinations");

            //Force load OwnedPerson
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            (from t in routes.SelectMany(r => r.Technicians)
             join p in this.ObjectContext.Parties
                 on t.Id equals p.Id
             select p).ToArray();

            return routes;
        }

        public void InsertRoute(Route route)
        {
            route.Date = route.Date.Date; //Remove any time
            route.StartTime = route.StartTime.SetDate(route.Date); //Set to same date as route.Date
            route.EndTime = route.EndTime.SetDate(route.Date); //Set to same date as route.Date

            if ((route.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(route, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Routes.AddObject(route);
            }
        }

        public void UpdateRoute(Route currentRoute)
        {
            this.ObjectContext.Routes.AttachAsModified(currentRoute);
        }

        public void DeleteRoute(Route route)
        {
            if ((route.EntityState == EntityState.Detached))
                this.ObjectContext.Routes.Attach(route);

            route.Technicians.Load();
            route.Technicians.Clear();
            route.Vehicles.Load();
            route.Vehicles.Clear();

            route.RouteDestinations.Load();

            foreach (var routeDestination in route.RouteDestinations.ToArray())
                this.DeleteRouteDestination(routeDestination);

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
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeDestination, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RouteDestinations.AddObject(routeDestination);
            }
        }

        public void UpdateRouteDestination(RouteDestination currentRouteDestination)
        {
            this.ObjectContext.RouteDestinations.AttachAsModified(currentRouteDestination);
        }

        public void DeleteRouteDestination(RouteDestination routeDestination)
        {
            if (routeDestination.EntityState == EntityState.Detached)
            {
                var loadedRouteDestination =
                     this.ObjectContext.RouteDestinations.FirstOrDefault(rd => rd.Id == routeDestination.Id);

                if (loadedRouteDestination != null)
                    this.ObjectContext.Detach(loadedRouteDestination);

                this.ObjectContext.RouteDestinations.Attach(routeDestination);
            }

            routeDestination.RouteReference.Load();
            if (routeDestination.Route != null)
                routeDestination.Route.RouteDestinations.Remove(routeDestination);

            routeDestination.ClientReference.Load();
            if (routeDestination.Client != null)
                routeDestination.Client.RouteDestinations.Remove(routeDestination);

            routeDestination.LocationReference.Load();
            if (routeDestination.Location != null)
                routeDestination.Location.RouteDestinations.Remove(routeDestination);

            //Do not delete RouteTasks, just clear the reference
            routeDestination.RouteTasks.Load();
            routeDestination.RouteTasks.Clear();

            this.ObjectContext.RouteDestinations.DeleteObject(routeDestination);
        }

        #endregion

        #region RouteTask

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

            var businessAccount = ObjectContext.BusinessAccountForRole(roleId);

            //Get all the existing services for the day => later to be unioned with generatedServices
            IEnumerable<Service> existingServices = ((ObjectQuery<Service>)this.ObjectContext.Services.Where(service => service.ServiceDate == serviceDate && service.ServiceProviderId == businessAccount.Id)).
                Include("ServiceTemplate").Include("RouteTasks").ToList();

            //Force Load ServiceTemplates.Fields and LocationsFields (for Destination), and Invoices
            (from es in existingServices
             join st in this.ObjectContext.ServiceTemplates
                 on es.Id equals st.Id
             from f in st.Fields
             from lf in st.Fields.OfType<LocationField>()
             select new { st, f, lf.Value, st.Invoice })
            .ToArray();

            //Force load invoices
            (from st in existingServices.Select(es => es.ServiceTemplate)
             join i in this.ObjectContext.Invoices
                 on st.Id equals i.Id
             select i).ToArray();

            var recurringServicesForDate = RecurringServicesForDate(serviceDate, businessAccount);

            //Generate services for the day from RecurringServices (that do not have generated services)
            var generatedServices = recurringServicesForDate.Where(rs => !existingServices.Any(s => s.RecurringServiceId == rs.Id)).Select(rs =>
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
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeTask, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RouteTasks.AddObject(routeTask);
            }
        }

        public void UpdateRouteTask(RouteTask currentRouteTask)
        {
            this.ObjectContext.RouteTasks.AttachAsModified(currentRouteTask);
        }

        public void DeleteRouteTask(RouteTask routeTask)
        {
            if ((routeTask.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(routeTask, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.RouteTasks.Attach(routeTask);
                this.ObjectContext.RouteTasks.DeleteObject(routeTask);
            }
        }

        #endregion
    }
}
