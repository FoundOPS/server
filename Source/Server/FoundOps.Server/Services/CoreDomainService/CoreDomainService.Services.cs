using System;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Collections.Generic;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.EntityFramework;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any service entities:
    /// Services and RecurringServices
    /// </summary>
    public partial class CoreDomainService
    {
        #region Service

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

        #region RecurringService  //TODO Optimize

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

    public enum CrawlingPosition
    {
        Front,
        StoppedInMiddle,
        Back
    }
}
