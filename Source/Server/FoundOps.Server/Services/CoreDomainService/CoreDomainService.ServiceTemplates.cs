using System.Data.Entity;
using System.Windows;
using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.Core.Tools;
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

        /// <summary>
        /// Searches the parent FoundOPS ServiceTemplate for Fields.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="foundOpsLevelServiceTemplateId">The service template you are searching fields for.</param>
        /// <param name="serviceProviderServiceTemplateId">The service template you are adding fields to. </param>
        /// <param name="searchText">The search text.</param>
        public IQueryable<Field> SearchFieldsForServiceProvider(Guid roleId, Guid foundOpsLevelServiceTemplateId, Guid serviceProviderServiceTemplateId, string searchText)
        {
            //Gets all fields from the specified service template
            var foundOpsFields = HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, foundOpsLevelServiceTemplateId, null, null, null).SelectMany(st => st.Fields);

            var serviceProviderFields = HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, serviceProviderServiceTemplateId, null, null, null).SelectMany(st => st.Fields);

            var fields = foundOpsFields.Where(f => !serviceProviderFields.Select(spf => spf.Name).Contains(f.Name));

            if (!String.IsNullOrEmpty(searchText))
                fields = fields.Where(f => f.Name.StartsWith(searchText));

            return fields.AsQueryable().OrderBy(f => f.Name);
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

            var serviceTemplate = ObjectContext.ServiceTemplates.FirstOrDefault(st => st.Id == field.OwnerServiceTemplate.Id);

            //When adding a FoundOPS level service template to a service provider propogate the template/fields to all the Clients
            if (serviceTemplate != null && serviceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
            {
                //In order to propagate the original Template added must already be saved
                ObjectContext.SaveChanges();

                ObjectContext.PropagateNewFields(field.Id);
            }
        }

        public void UpdateField(Field currentField)
        {
            this.ObjectContext.Fields.AttachAsModified(currentField);
        }

        public void DeleteField(Field field)
        {
            var serviceTemplate = ObjectContext.ServiceTemplates.FirstOrDefault(st => st.Id == field.ServiceTemplateId);

            //If the associated ServiceTemplate is on the ServiceProvider level, delete the field and all children
            if (serviceTemplate != null && serviceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
            {
                ObjectContext.DeleteFieldAndChildrenBasedOnFieldId(field.Id);
                return;
            }

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

        //private so it is not automatically called
        private void DeleteOptionsField(OptionsField optionsField)
        {
            this.ObjectContext.DetachExistingAndAttach(optionsField);
            optionsField.Options.Load();
            var optionsToDelete = optionsField.Options.ToArray();
            foreach (var option in optionsToDelete)
                this.DeleteOption(option);

            this.ObjectContext.Fields.DeleteObject(optionsField);
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
            var businessAccount = ObjectContext.Owner(roleId).First();

            IQueryable<ServiceTemplate> serviceTemplatesToReturn = this.ObjectContext.ServiceTemplates;

            //Filter by serviceTemplateLevel if it is not null
            if (serviceTemplateLevel != null)
                serviceTemplatesToReturn = serviceTemplatesToReturn.Where(st => st.LevelInt == serviceTemplateLevel);

            //If the current role is FoundOPS, filter by what service templates can be accessed
            if (businessAccount.Id != BusinessAccountsConstants.FoundOpsId)
            {
                //Filter by the service templates for the service provider (Vendor)
                serviceTemplatesToReturn = (from serviceTemplate in serviceTemplatesToReturn
                                            join stv in ObjectContext.ServiceTemplateWithVendorIds
                                                on serviceTemplate.Id equals stv.ServiceTemplateId
                                            where stv.BusinessAccountId == businessAccount.Id
                                            select serviceTemplate).Distinct();
            }

            //TODO? Limit by 1000
            return serviceTemplatesToReturn.Distinct().OrderBy(st => st.Name);
        }

        /// <summary>
        /// Gets the current ServiceProvider's ServiceProvider level ServiceTemplates and details.
        /// It includes the OwnerClient, Fields, OptionsFields w Options, LocationFields w Location, (TODO) Invoices
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        public IEnumerable<ServiceTemplate> GetServiceProviderServiceTemplates(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            //If FoundOPS account => Filter only FoundOPS templates
            //Else => Filter by the service templates for the service provider (Vendor)
            var serviceProviderTemplates = businessAccount.Id == BusinessAccountsConstants.FoundOpsId 
                ? HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, null, null, businessAccount.Id, (int)ServiceTemplateLevel.FoundOpsDefined).AsQueryable() 
                : HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, null, null, businessAccount.Id, (int)ServiceTemplateLevel.ServiceProviderDefined).AsQueryable();

            return serviceProviderTemplates.OrderBy(st => st.Name);
        }

        /// <summary>
        /// Gets the Client's ServiceTemplates and details.
        /// It includes the OwnerClient, Fields, OptionsFields w Options, LocationFields w Location, (TODO) Invoices
        /// </summary>
        /// <param name="vendorId">The current role's vendor id.</param>
        /// <param name="clientId">The clientId to load service templates for.</param>
        private IEnumerable<ServiceTemplate> GetClientServiceTemplates(Guid vendorId, Guid clientId)
        {
            //Filter by the service templates for the Client
            //also filter be service provider (Vendor) for security
            var serviceTemplates = HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, null, clientId, null, null).Where(st => st.OwnerServiceProviderId == vendorId);

            return serviceTemplates.OrderBy(st => st.Name);
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

            return foundOpsServiceTemplates.Distinct().OrderBy(st => st.Name);
        }

        /// <summary>
        /// Gets the service template details. Should only use when loading one ServiceTemplate.
        /// It includes the OwnerClient, Fields, OptionsFields w Options, LocationFields w Location, (TODO) Invoices
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="serviceTemplateId">The ServiceTemplate id.</param>
        public ServiceTemplate GetServiceTemplateDetailsForRole(Guid roleId, Guid serviceTemplateId)
        {
            return HardCodedLoaders.LoadServiceTemplateWithDetails(this.ObjectContext, serviceTemplateId, null, null, null).First();

            //TODO w QuickBooks
            ////Force load Invoices
            ////Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            //this.ObjectContext.Invoices.Join(serviceTemplatesForServiceProvider, i => i.Id, st => st.Id, (i, st) => i).ToArray();
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
            ObjectContext.DeleteServiceTemplateAndChildrenBasedOnServiceTemplateId(serviceTemplate.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceTemplate"></param>
        public void InsertServiceTemplate(ServiceTemplate serviceTemplate)
        {
            if (serviceTemplate.ServiceTemplateLevel != ServiceTemplateLevel.FoundOpsDefined && serviceTemplate.OwnerServiceTemplateId == null)
                throw new Exception("All service templates (besides FoundOPS levels) should have a parent.");

            if ((serviceTemplate.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(serviceTemplate, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ServiceTemplates.AddObject(serviceTemplate);
            }

            //When adding a FoundOPS level service template to a service provider propogate the template/fields to all the Clients
            if (serviceTemplate.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
            {
                //In order to propagate the original Template added must already be saved
                ObjectContext.SaveChanges();

                ObjectContext.PropagateNewServiceTemplateToClients(serviceTemplate.Id);
            }
        }

        /// <summary>
        /// This will only be used for FoundOPS level service templates. Editing name is disabled for lower service templates, for now.
        /// Name changes on FoundOPS level service templates should propogate to all children.
        /// </summary>
        /// <param name="currentServiceTemplate"></param>
        public void UpdateServiceTemplate(ServiceTemplate currentServiceTemplate)
        {
            if (currentServiceTemplate.ServiceTemplateLevel != ServiceTemplateLevel.FoundOpsDefined)
                throw new Exception("Should this ever happen?");

            this.ObjectContext.ServiceTemplates.AttachAsModified(currentServiceTemplate);

            ObjectContext.SaveChanges();

            ObjectContext.PropagateNameChange(currentServiceTemplate.Id);
        }

        #endregion
    }
}
