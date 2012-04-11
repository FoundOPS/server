using System.Data.Entity;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.Core.Tools;
using System;
using System.Data;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Server;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any service entities:
    /// Services and RecurringServices
    /// </summary>
    public partial class CoreDomainService
    {
        #region Service

        /// <summary>
        /// Gets the service details.
        /// It includes the Client/Client.OwnedParty and the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceId">The service id.</param>
        public Service GetServiceDetailsForRole(Guid roleId, Guid serviceId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var service = this.ObjectContext.Services.Include(s => s.Client.OwnedParty)
                .FirstOrDefault(s => s.Id == serviceId && s.ServiceProviderId == businessForRole.Id);

            //Load the ServiceTemplate for the Service
            GetServiceTemplateDetailsForRole(roleId, serviceId);

            return service;
        }

        /// <summary>
        /// Gets the service details for the current role.
        /// It includes the Client/Client.OwnedParty, RecurringServiceParent, the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="serviceIds">The ids of the Services to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Service> GetServicesDetailsForRole(Guid roleId, IEnumerable<Guid> serviceIds)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null) return null;

            var servicesForRole = this.ObjectContext.Services.Where(service => service.ServiceProviderId == businessForRole.Id).Where(s => serviceIds.Contains(s.Id));

            servicesForRole = servicesForRole.Include(s => s.Client).Include(s => s.Client.OwnedParty).Include(s => s.RecurringServiceParent)
                .Include(s => s.ServiceTemplate).Include(s => s.ServiceTemplate.Fields);

            //Force Load LocationField's Location
            //TODO: Optimize by IQueryable
            servicesForRole.SelectMany(s => s.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return servicesForRole;
        }

        public void InsertService(Service service)
        {
            //If the service being inserted has a RecurringServiceId
            //Find any RouteTasks that have the same RecurringServiceId and Date
            //Update the ServiceId on the RouteTask with the Id of the newly created Service
            if (service.RecurringServiceId != null)
            {
                var routeTasks = this.ObjectContext.RouteTasks.Where(rt => rt.RecurringServiceId == service.RecurringServiceId && rt.Date == service.ServiceDate).ToArray();

                foreach (var routeTask in routeTasks)
                    routeTask.ServiceId = service.Id;
            }

            if ((service.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(service, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Services.AddObject(service);
            }
        }

        public void UpdateService(Service currentService)
        {
            var originalService = ChangeSet.GetOriginal(currentService);

            //If the original date was prior to today, throw an exception
            if (originalService.ServiceDate < DateTime.UtcNow.Date)
                throw new Exception("Cannot change the date of a Service in the past.");

            //Delete any associated route tasks (today/in the future) if the service date changed
            if (originalService.ServiceDate != currentService.ServiceDate)
            {
                var routeTasksToDelete = this.ObjectContext.RouteTasks.Where(rt =>
                    rt.ServiceId == currentService.Id || (rt.RecurringServiceId == originalService.RecurringServiceId && rt.Date == originalService.ServiceDate)).ToArray();

                DeleteRouteTasks(routeTasksToDelete);
            }

            this.ObjectContext.Services.AttachAsModified(currentService);
        }

        /// <summary>
        /// Goes through each routeTask and removes it from route destinations
        /// Delete any route destinations without remaining tasks.
        /// </summary>
        /// <param name="routeTasksToDelete"></param>
        private void DeleteRouteTasks(RouteTask[] routeTasksToDelete)
        {
            //If no RouteTasks exist, return
            if (!routeTasksToDelete.Any()) return;

            foreach (var routeTask in routeTasksToDelete)
            {
                routeTask.RouteDestinationReference.Load();
                var routeDestination = routeTask.RouteDestination;

                //Remove the RouteTask from the RouteDestination
                if (routeDestination != null)
                {
                    routeDestination.RouteTasks.Load();
                    routeTask.RouteDestination = null;

                    //If the RouteDestination has no remaining tasks, delete it
                    if (routeDestination.RouteTasks.Count == 0)
                        DeleteRouteDestination(routeDestination);
                }

                //Delete the route task
                this.ObjectContext.DetachExistingAndAttach(routeTask);
            }
        }

        public void DeleteService(Service service)
        {
            if (service.Generated)
                return;

            if ((service.EntityState == EntityState.Detached))
                this.ObjectContext.Services.Attach(service);

            if (service.ServiceTemplate != null)
                this.DeleteServiceTemplate(service.ServiceTemplate);

            var routeTasksToDelete = this.ObjectContext.RouteTasks.Where(rt =>
                 rt.ServiceId == service.Id || (rt.RecurringServiceId == service.RecurringServiceId && rt.Date == service.ServiceDate)).ToArray();

            DeleteRouteTasks(routeTasksToDelete);

            if ((service.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(service, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Services.DeleteObject(service);
            }
        }

        #endregion

        #region Service Holders

        /// <summary>
        /// Gets the ServiceHolders based off the current ServiceProvider and optional contexts.
        /// It will return all the services on the seed date.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="clientContext">(Optional) If this is the only context set, it will generate ServiceHolders only for this client.</param>
        /// <param name="recurringServiceContext">(Optional) If this is the only context set, it will generate ServiceHolders only for this RecurringService.</param>
        /// <param name="seedDate">The date to start generating ServiceHolders from.</param>
        /// <param name="numberOfOccurrences">The (soft) minimum number of occurrences to return. If there are not enough services it will not meet the minimum.</param>
        /// <param name="getPrevious">Return all the services from at least one date before the seed date (if there are any).</param>
        /// <param name="getNext">Return all the services from at least one date after the seed date (if there are any).</param>
        /// <returns></returns>
        [Query]
        public IQueryable<ServiceHolder> GetServiceHolders(Guid roleId, Guid? clientContext, Guid? recurringServiceContext,
            DateTime seedDate, int numberOfOccurrences, bool getPrevious, bool getNext)
        {
            Guid? recurringServiceContextId = null;
            Guid? clientContextId = null;
            Guid? serviceProviderContextId = null;

            if (recurringServiceContext.HasValue)
            {
                //Check the user has access to the RecurringService
                if (!RecurringServicesForServiceProviderOptimized(roleId).Any(rs => rs.Id == recurringServiceContext))
                    return null;

                recurringServiceContextId = recurringServiceContext;
            }
            else if (clientContext.HasValue)
            {
                //Check the user has access to the Client
                if (!GetClientsForRole(roleId).Any(c => c.Id == clientContext))
                    return null;

                clientContextId = clientContext;
            }
            else
            {
                var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

                //Because there is no other context, assume the context should be the serviceProvider
                //The businessForRole returns null if the user does not have access to the ServiceProvider
                serviceProviderContextId = businessForRole.Id;
            }

            //Order by OccurDate and Service Name for UI
            //Order by ServiceId and RecurringServiceId to ensure they are always returned in the same order
            return ObjectContext.GetServiceHolders(serviceProviderContextId, clientContextId, recurringServiceContextId,
                seedDate, numberOfOccurrences, getPrevious, getNext)
                .OrderBy(sh => sh.OccurDate).ThenBy(sh => sh.ServiceName).ThenBy(sh => sh.ServiceId).ThenBy(sh => sh.RecurringServiceId)
                .AsQueryable();
        }

        /// <summary>
        /// An empty method to allow creating ServiceHolders on the client.
        /// </summary>
        public void InsertServiceHolder(ServiceHolder serviceHolder)
        {
            throw new Exception("Cannot save ServiceHolders in the database.");
        }

        #endregion

        #region Recurring Services

        /// <summary>
        /// Gets the RecurringServices for the roleId.
        /// It includes the bare minimum Client.
        /// </summary>
        /// <param name="roleId">The role to determine the business account from.</param>
        private IQueryable<RecurringService> RecurringServicesForServiceProviderOptimized(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var recurringServices = this.ObjectContext.RecurringServices.Include(rs => rs.Client)
                .Where(v => v.Client.VendorId == businessForRole.Id);

            return recurringServices.OrderBy(rs => rs.ClientId);
        }

        /// <summary>
        /// Gets the RecurringService details.
        /// It includes the Client.OwnedParty, ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="recurringServiceId">The recurring service id.</param>
        public RecurringService GetRecurringServiceDetailsForRole(Guid roleId, Guid recurringServiceId)
        {
            var recurringServices = RecurringServicesForServiceProviderOptimized(roleId).Where(rs => rs.Id == recurringServiceId)
                .Include(c => c.Client.OwnedParty).Include(rs => rs.ServiceTemplate).Include(rs => rs.ServiceTemplate.Fields);

            //Force Load LocationField's Location
            recurringServices.SelectMany(s => s.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return recurringServices.FirstOrDefault();
        }

        /// <summary>
        /// Gets the RecurringService details.
        /// It includes the Client.OwnedParty, ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="recurringServiceIds">The ids of the RecurringServices to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<RecurringService> GetRecurringServicesWithDetailsForRole(Guid roleId, IEnumerable<Guid> recurringServiceIds)
        {
            var recurringServices = RecurringServicesForServiceProviderOptimized(roleId)
                .Where(rs => recurringServiceIds.Contains(rs.Id)).Include(c => c.Client.OwnedParty).Include(rs => rs.ServiceTemplate).Include(rs => rs.ServiceTemplate.Fields);

            //Force Load LocationField's Location
            //TODO: Optimize by IQueryable
            recurringServices.SelectMany(rs => rs.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            return recurringServices;
        }

        public void InsertRecurringService(RecurringService recurringService)
        {
            if ((recurringService.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(recurringService, EntityState.Added);
            }
            else
            {
                this.ObjectContext.RecurringServices.AddObject(recurringService);
            }
        }

        public void UpdateRecurringService(RecurringService currentRecurringService)
        {
            //If there is a newly added excluded date today or in the future. Remove any associated route tasks for that date
            var originalRecurringService = ChangeSet.GetOriginal(currentRecurringService);
            var newlyAddedFutureDatesToExclude = currentRecurringService.ExcludedDates.Except(originalRecurringService.ExcludedDates).Where(d => d >= DateTime.UtcNow.Date).ToArray();
            if (newlyAddedFutureDatesToExclude.Any())
            {
                var routeTasksToDelete = this.ObjectContext.RouteTasks.Where(rt =>
                    rt.RecurringServiceId == currentRecurringService.Id && newlyAddedFutureDatesToExclude.Contains(rt.Date)).ToArray();

                DeleteRouteTasks(routeTasksToDelete);
            }

            this.ObjectContext.RecurringServices.AttachAsModified(currentRecurringService);
        }

        public void DeleteRecurringService(RecurringService recurringService)
        {
            ObjectContext.DeleteRecurringService(recurringService.Id);
        }

        #endregion
    }
}
