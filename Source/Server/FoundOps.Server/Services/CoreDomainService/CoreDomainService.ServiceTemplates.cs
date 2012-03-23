using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any template entities:
    /// Fields, Options, and ServiceTemplates
    /// TODO: Secure
    /// </summary>
    public partial class CoreDomainService
    {
        #region Fields

        public IQueryable<Field> GetFields()
        {
            return this.ObjectContext.Fields;
        }

        public IQueryable<OptionsField> GetOptionsField()
        {
            return this.ObjectContext.Fields.OfType<OptionsField>();
        }

        public IQueryable<OptionsField> GetOptionsFields()
        {
            return this.ObjectContext.Fields.OfType<OptionsField>();
        }

        public IQueryable<LocationField> GetLocationFields()
        {
            return this.ObjectContext.Fields.OfType<LocationField>();
        }

        public IQueryable<TextBoxField> GetTextBoxFields()
        {
            return this.ObjectContext.Fields.OfType<TextBoxField>();
        }

        public void InsertField(Field field)
        {
            if ((field.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(field, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Fields.AddObject(field);
            }
        }

        public void UpdateField(Field currentField)
        {
            this.ObjectContext.Fields.AttachAsModified(currentField);
        }

        public void DeleteOptionsField(OptionsField optionsField)
        {
            this.ObjectContext.DetachExistingAndAttach(optionsField);
            optionsField.Options.Load();
            var optionsToDelete = optionsField.Options.ToArray();
            foreach (var option in optionsToDelete)
                this.DeleteOption(option);

            this.ObjectContext.Fields.DeleteObject(optionsField);
        }

        public void DeleteField(Field field)
        {
            var loadedField = this.ObjectContext.Fields.FirstOrDefault(f => f.Id == field.Id);
            if (loadedField != null) this.ObjectContext.Detach(loadedField);

            if ((field.EntityState == EntityState.Detached))
                this.ObjectContext.Fields.Attach(field);

            if (field as OptionsField != null)
            {
                this.DeleteOptionsField((OptionsField)field);
                return;
            }

            field.ChildrenFields.Load();
            //Delete children
            foreach (var child in field.ChildrenFields.ToArray())
                DeleteField(child);

            this.ObjectContext.Fields.DeleteObject(field);
        }

        #endregion

        #region Option

        public IQueryable<Option> GetOptions()
        {
            return this.ObjectContext.Options;
        }

        public void InsertOption(Option option)
        {
            if ((option.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(option, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Options.AddObject(option);
            }
        }

        public void UpdateOption(Option currentOption)
        {
            this.ObjectContext.Options.AttachAsModified(currentOption);
        }

        public void DeleteOption(Option option)
        {
            var loadedOption = this.ObjectContext.Options.FirstOrDefault(o => o.Id == option.Id);

            if (loadedOption != null)
                this.ObjectContext.Detach(loadedOption);

            if (option.EntityState == EntityState.Detached)
                this.ObjectContext.Options.Attach(option);

            this.ObjectContext.Options.DeleteObject(option);
        }

        #endregion

        #region ServiceTemplate

        /// <summary>
        /// Gets the ServiceTemplates the current role has access to.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceTemplateLevel">(Optional/recommended) The service template level to filter by.</param>
        public IQueryable<ServiceTemplate> GetServiceTemplatesForServiceProvider(Guid roleId, int? serviceTemplateLevel)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            IQueryable<ServiceTemplate> serviceTemplatesToReturn = this.ObjectContext.ServiceTemplates;

            //Filter by serviceTemplateLevel if it is not null
            if (serviceTemplateLevel != null)
                serviceTemplatesToReturn = serviceTemplatesToReturn.Where(st => st.LevelInt == serviceTemplateLevel);

            //If the current role is FoundOPS, filter by what service templates can be accessed
            if (businessForRole.Id != BusinessAccountsConstants.FoundOpsId)
            {
                //Filter by the service templates for the service provider (Vendor)
                serviceTemplatesToReturn = from serviceTemplate in serviceTemplatesToReturn
                                           join stv in ObjectContext.ServiceTemplateWithVendorIds
                                               on serviceTemplate.Id equals stv.ServiceTemplateId
                                           where stv.VendorId == businessForRole.Id
                                           select serviceTemplate;
            }

            //TODO? Limit by 1000
            return serviceTemplatesToReturn.OrderBy(st => st.Name);
        }

        /// <summary>
        /// Searches the FoundOPS ServiceTemplates the current role's ServiceProvider does not have yet.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceProviderId">The service provider you are searching templates for.</param>
        /// <param name="searchText">The search text.</param>
        public IQueryable<ServiceTemplate> SearchServiceTemplatesForServiceProvider(Guid roleId, Guid serviceProviderId, string searchText)
        {
            var foundOpsServiceTemplates = GetServiceTemplatesForServiceProvider(roleId, (int)ServiceTemplateLevel.FoundOpsDefined);

            if (!String.IsNullOrEmpty(searchText))
                foundOpsServiceTemplates = foundOpsServiceTemplates.Where(st => st.Name.StartsWith(searchText));

            var existingChildren = GetServiceTemplatesForServiceProvider(roleId, (int)ServiceTemplateLevel.ServiceProviderDefined)
                .Where(sp => sp.OwnerServiceProviderId == serviceProviderId);

            //Remove existing children service templates
            foundOpsServiceTemplates = foundOpsServiceTemplates.Where(foundOpsTemplate =>
                !existingChildren.Any(ec => ec.OwnerServiceTemplateId == foundOpsTemplate.Id));

            return foundOpsServiceTemplates.OrderBy(st => st.Name);
        }

        /// <summary>
        /// Gets the service template details.
        /// It includes the OwnerClient, Fields, OptionsFields w Options, LocationFields w Location, (TODO) Invoices
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceTemplateId">The ServiceTemplate id.</param>
        public ServiceTemplate GetServiceTemplateDetailsForRole(Guid roleId, Guid serviceTemplateId)
        {
            var serviceTemplateTuples =
                from serviceTemplate in GetServiceTemplatesForServiceProvider(roleId, null).Where(st => st.Id == serviceTemplateId)
                from options in serviceTemplate.Fields.OfType<OptionsField>().Select(of => of.Options).DefaultIfEmpty()
                from locations in serviceTemplate.Fields.OfType<LocationField>().Select(lf => lf.Value).DefaultIfEmpty()
                select new { serviceTemplate, serviceTemplate.OwnerClient, serviceTemplate.Fields, options, locations };

            var serviceTemplateWithDetails = serviceTemplateTuples.First().serviceTemplate;

            //TODO w QuickBooks
            ////Force load Invoices
            ////Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            //this.ObjectContext.Invoices.Join(serviceTemplatesForServiceProvider, i => i.Id, st => st.Id, (i, st) => i).ToArray();

            return serviceTemplateWithDetails;
        }

        /// <summary>
        /// Deletes the service template and its invoice, fields, and children service templates
        /// </summary>
        /// <param name="serviceTemplate"></param>
        public void DeleteServiceTemplate(ServiceTemplate serviceTemplate)
        {
            //TODO cannot delete if it has children
            //Stored procedure that will find all the children of this ServiceTemplate
            //Then it will delete all of them
            //Cascades will take care of all associations
            //ObjectContext.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId(serviceTemplate.Id);
        }

        private IEnumerable<ServiceTemplate> GetDescendants(ServiceTemplate serviceTemplate)
        {
            serviceTemplate.ChildrenServiceTemplates.Load();

            if (!serviceTemplate.ChildrenServiceTemplates.Any())
                return new List<ServiceTemplate>();

            var descendants = new List<ServiceTemplate>();

            foreach (var child in serviceTemplate.ChildrenServiceTemplates)
            {
                descendants.Add(child);
                GetDescendants(child);
            }

            return descendants;
        }

        public void InsertServiceTemplate(ServiceTemplate serviceTemplate)
        {
            if ((serviceTemplate.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(serviceTemplate, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ServiceTemplates.AddObject(serviceTemplate);
            }
        }

        public void UpdateServiceTemplate(ServiceTemplate currentServiceTemplate)
        {
            this.ObjectContext.ServiceTemplates.AttachAsModified(currentServiceTemplate);
        }

        #endregion
    }
}
