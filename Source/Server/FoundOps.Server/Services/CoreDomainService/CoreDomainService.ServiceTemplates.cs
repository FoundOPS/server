using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ServiceModel.DomainServices.Server;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any template entities:
    /// Fields, Options, and ServiceTemplates
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

        public IQueryable<ServiceTemplate> GetServiceTemplatesForServiceProvider(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var recurringServicesForServiceProvider = GetRecurringServicesForServiceProvider(roleId);

            var serviceTemplatesForServiceProvider =
                this.ObjectContext.ServiceTemplates.Include("Fields").Include("OwnerClient").Where(
                        st => st.OwnerServiceProviderId == businessForRole.Id ||
                        (st.OwnerClient != null && st.OwnerClient.VendorId == businessForRole.Id) ||
                        recurringServicesForServiceProvider.Any(rs => rs.Id == st.OwnerRecurringService.Id));


            //Force Load Options
            serviceTemplatesForServiceProvider.SelectMany(st => st.Fields).OfType<OptionsField>().SelectMany(of => of.Options).ToArray();

            //Force Load LocationField's Location
            serviceTemplatesForServiceProvider.SelectMany(st => st.Fields).OfType<LocationField>().Select(lf => lf.Value).ToArray();

            //Force load Invoices
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            this.ObjectContext.Invoices.Join(serviceTemplatesForServiceProvider, i => i.Id, st => st.Id, (i, st) => i).ToArray();

            return serviceTemplatesForServiceProvider;
        }

        /// <summary>
        /// Deletes the service template and its invoice, fields, and children service templates
        /// </summary>
        /// <param name="serviceTemplate"></param>
        public void DeleteServiceTemplate(ServiceTemplate serviceTemplate)
        {
            var loadedServiceTemplate = this.ObjectContext.ServiceTemplates.FirstOrDefault(st => st.Id == serviceTemplate.Id);
            if (loadedServiceTemplate != null)
                this.ObjectContext.Detach(loadedServiceTemplate);

            if ((serviceTemplate.EntityState == EntityState.Detached))
                this.ObjectContext.ServiceTemplates.Attach(serviceTemplate);

            //Force delete all sub servicetemplates if you are deleting a business account
            var forceDelete = this.ChangeSet.ChangeSetEntries
                .Any(cse => cse.Entity is BusinessAccount && cse.Operation == DomainOperation.Delete &&
                            ((BusinessAccount)cse.Entity).ServiceTemplates.Contains(serviceTemplate));

            var decendants = GetDescendants(serviceTemplate);

            //If not force delete, make sure there are no RecurringServiceLevel or ServiceLevel decendants before deleting the service template
            if (!forceDelete)
            {
                //Find last descendant and check what level it is at
                var recurringServiceOrServiceLevelExist =
                    decendants.Any(d => d.ServiceTemplateLevel == ServiceTemplateLevel.ServiceDefined ||
                                        d.ServiceTemplateLevel == ServiceTemplateLevel.RecurringServiceDefined);

                //If the lowest level is either the RecurringService or Service level, do not delete
                if (recurringServiceOrServiceLevelExist)
                    return;
            }

            //Delete the ServiceTemplate and it's descendants
            var serviceTemplatesToDelete = decendants.Union(new[] { serviceTemplate });

            foreach (var serviceTemplateToDelete in serviceTemplatesToDelete)
            {
                //Delete each Field
                serviceTemplateToDelete.Fields.Load();
                var fieldsToDelete = serviceTemplateToDelete.Fields.ToArray();
                foreach (var fieldToDelete in fieldsToDelete)
                    this.DeleteField(fieldToDelete);

                //Delete the invoice reference
                serviceTemplateToDelete.InvoiceReference.Load();
                if (serviceTemplateToDelete.Invoice != null)
                    DeleteInvoice(serviceTemplateToDelete.Invoice);

                this.ObjectContext.ServiceTemplates.DeleteObject(serviceTemplateToDelete);
            }
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
