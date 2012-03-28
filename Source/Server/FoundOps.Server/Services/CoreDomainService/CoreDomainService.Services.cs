using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using FoundOps.Server.Authentication;
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
                if (!GetRecurringServicesForServiceProviderOptimized(roleId).Any(rs => rs.Id == recurringServiceContext))
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
        /// Gets the service details.
        /// It includes the Client and the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceId">The service id.</param>
        public Service GetServiceDetailsForRole(Guid roleId, Guid serviceId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var service = this.ObjectContext.Services.Include("Client")
                .FirstOrDefault(s => s.Id == serviceId && s.ServiceProviderId == businessForRole.Id);

            //Load the ServiceTemplate for the Service
            GetServiceTemplateDetailsForRole(roleId, serviceId);

            return service;
        }

        public IEnumerable<Service> GetServicesForRole(Guid roleId)
        {
            return GetExistingServicesForRole(roleId, Guid.Empty, Guid.Empty, Guid.Empty);
        }

        private IEnumerable<Service> GetExistingServicesForRole(Guid roleId, Guid clientIdFilter, Guid locationIdFilter, Guid recurringServiceIdFilter)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null) return null;

            var servicesForRole = this.ObjectContext.Services.Where(service => service.ServiceProviderId == businessForRole.Id);

            if (clientIdFilter != Guid.Empty)
                servicesForRole = servicesForRole.Where(s => s.ClientId == clientIdFilter);

            if (recurringServiceIdFilter != Guid.Empty)
                servicesForRole = servicesForRole.Where(s => s.RecurringServiceId == recurringServiceIdFilter);

            servicesForRole = ((ObjectQuery<Service>)servicesForRole).Include("Client").Include("Client.OwnedParty").Include("RecurringServiceParent").Include("ServiceTemplate").Include("ServiceTemplate.Fields");

            //Force Load LocationField's Location
            servicesForRole.SelectMany(s => s.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            if (locationIdFilter != Guid.Empty)
                return servicesForRole.ToList().Where(s => s.ServiceTemplate.GetDestination() != null && s.ServiceTemplate.GetDestination().Id == locationIdFilter);

            return servicesForRole;
        }

        public void InsertService(Service service)
        {
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
            this.ObjectContext.Services.AttachAsModified(currentService);
        }

        public void DeleteService(Service service)
        {
            if (service.Generated)
                return;

            if ((service.EntityState == EntityState.Detached))
                this.ObjectContext.Services.Attach(service);

            if (service.ServiceTemplate != null)
                this.DeleteServiceTemplate(service.ServiceTemplate);

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

        #region RecurringService Optimized

        /// <summary>
        /// Gets the RecurringServices for the roleId.
        /// </summary>
        /// <param name="roleId">The role to determine the business account from.</param>
        public IEnumerable<RecurringService> GetRecurringServicesForServiceProviderOptimized(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var recurringServices = this.ObjectContext.RecurringServices.Include("Client").Where(v => v.Client.VendorId == businessForRole.Id);
            return recurringServices;
        }

        /// <summary>
        /// Gets the RecurringService details.
        /// It includes the Client and the ServiceTemplate and Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="recurringServiceId">The recurring service id.</param>
        public RecurringService GetRecurringServiceDetailsForRole(Guid roleId, Guid recurringServiceId)
        {
            var recurringService = GetRecurringServicesForServiceProviderOptimized(roleId).FirstOrDefault(rs => rs.Id == recurringServiceId);

            //Load the ServiceTemplate for the RecurringService
            GetServiceTemplateDetailsForRole(roleId, recurringServiceId);

            return recurringService;
        }

        #endregion

        #region RecurringService  //TODO Optimize then Delete

        public IEnumerable<RecurringService> GetRecurringServicesForServiceProvider(Guid roleId)
        {
            return GetRecurringServicesForServiceProvider(roleId, Guid.Empty, Guid.Empty, Guid.Empty);
        }

        private IEnumerable<RecurringService> GetRecurringServicesForServiceProvider(Guid roleId, Guid clientIdFilter, Guid locationIdFilter, Guid recurringServiceIdFilter)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var recurringServicesToReturn = this.ObjectContext.RecurringServices.Include("Client").Where(v => v.Client.VendorId == businessForRole.Id);

            if (clientIdFilter != Guid.Empty)
                recurringServicesToReturn = recurringServicesToReturn.Where(rs => rs.ClientId == clientIdFilter);

            if (recurringServiceIdFilter != Guid.Empty)
                recurringServicesToReturn = recurringServicesToReturn.Where(rs => rs.Id == recurringServiceIdFilter);

            recurringServicesToReturn = ((ObjectQuery<RecurringService>)recurringServicesToReturn).Include("Repeat").Include("ServiceTemplate").Include("ServiceTemplate.Fields").Include("Client.OwnedParty");

            //Force Load LocationField's Location
            recurringServicesToReturn.SelectMany(rs => rs.ServiceTemplate.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            //Force Load Options
            recurringServicesToReturn.SelectMany(rs => rs.ServiceTemplate.Fields).OfType<OptionsField>().SelectMany(of => of.Options).ToArray();

            //Force Load ServiceTemplate.Invoice
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            this.ObjectContext.Invoices.Join(recurringServicesToReturn.Select(rs => rs.ServiceTemplate), i => i.Id, st => st.Id, (i, st) => i).ToArray();

            if (locationIdFilter != Guid.Empty)
                return recurringServicesToReturn.ToList().Where(rs => rs.ServiceTemplate.GetDestination() != null && rs.ServiceTemplate.GetDestination().Id == locationIdFilter);

            return recurringServicesToReturn;
        }

        private IEnumerable<RecurringService> RecurringServicesForDate(DateTime selectedDate, BusinessAccount businessAccount)
        {
            var recurringServicesForBusinessAccount =
                (from client in this.ObjectContext.Clients.Where(c => c.VendorId == businessAccount.Id)
                 from recurringService in this.ObjectContext.RecurringServices.Where(rs => rs.ClientId == client.Id)
                 select recurringService);

            //Force load Repeats
            recurringServicesForBusinessAccount.Select(rs => rs.Repeat).ToArray();

            //Force load Clients
            recurringServicesForBusinessAccount.Select(rs => rs.Client).ToArray();

            //Force Load ServiceTemplates.Fields (for MakeChild), and LocationsFields (for Destination)
            var serviceTemplatesAndDestinations = (from rs in recurringServicesForBusinessAccount
                                                   join st in this.ObjectContext.ServiceTemplates
                                                       on rs.Id equals st.Id
                                                   from f in st.Fields
                                                   from lf in st.Fields.OfType<LocationField>()
                                                   select new { st, f, lf.Value }).ToArray();

            //Force Load ServiceTemplate.Invoice
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            this.ObjectContext.Invoices.Join(recurringServicesForBusinessAccount.Select(rs => rs.ServiceTemplate), i => i.Id, st => st.Id, (i, st) => i).ToArray();

            //Filter only recurring services on date (and not excluded on that date)
            return recurringServicesForBusinessAccount.ToList().Where(rs => !rs.ExcludedDates.Contains(selectedDate) && rs.Repeat.NextRepeatDateOnOrAfterDate(selectedDate.Date) == selectedDate.Date);
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
            this.ObjectContext.RecurringServices.AttachAsModified(currentRecurringService);
        }

        public void DeleteRecurringService(RecurringService recurringService)
        {
            if ((recurringService.EntityState == EntityState.Detached))
                this.ObjectContext.RecurringServices.Attach(recurringService);

            recurringService.GeneratedServices.Load();
            recurringService.GeneratedServices.Clear(); //Sever link to generated services

            recurringService.ServiceTemplateReference.Load();

            if (recurringService.ServiceTemplate != null)
                this.DeleteServiceTemplate(recurringService.ServiceTemplate);

            var repeat = this.ObjectContext.Repeats.FirstOrDefault(r => r.Id == recurringService.Id);

            if (repeat != null)
                this.DeleteRepeat(repeat);

            this.ObjectContext.RecurringServices.DeleteObject(recurringService);
        }

        #endregion
    }
}
