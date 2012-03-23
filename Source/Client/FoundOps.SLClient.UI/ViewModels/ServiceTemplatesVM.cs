using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model to manage adding, modifying, and deleting ServiceTemplates.
    /// </summary>
    [ExportViewModel("ServiceTemplatesVM")]
    public class ServiceTemplatesVM : InfiniteAccordionVM<ServiceTemplate>, IAddToDeleteFromSource<ServiceTemplate>
    {
        #region Public Properties

        #region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        public Func<string, ServiceTemplate> CreateNewItem { get; private set; }

        public IEqualityComparer<object> CustomComparer { get; set; }

        public string MemberPath { get; private set; }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions
        {
            get { return null; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplatesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServiceTemplatesVM()
            : base(null, //<-- TODO when setting up data loading
                false)
        {
            SetupDataLoading();

            #region IAddToDeleteFromSource<ServiceTemplate> Implementation

            MemberPath = "Name";

            CreateNewItem = name =>
            {
                NavigateToThis();
                var newServiceTemplate = CreateNewServiceTemplate(null, name);
                SelectedEntity = newServiceTemplate;
                return newServiceTemplate;
            };

            //TODO Make the comparer gets used by the SearchSuggestionsHelper
            CustomComparer = new ServiceTemplateIsAncestorOrDescendent();

            //ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
            //{
            //    //TODO Add Context
            //    SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.Context.SearchServiceTemplatesForRole(Manager.Context.RoleId, searchText));
            //};

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId =>
                                        {
                                            //Only load service templates of the current level
                                            var serviceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined;

                                            var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                                            if(businessAccountContext!=null && businessAccountContext.Id != BusinessAccountsConstants.FoundOpsId)
                                                serviceTemplateLevel = ServiceTemplateLevel.ServiceProviderDefined;

                                            var clientContext = ContextManager.GetContext<Client>();
                                            if (clientContext != null)
                                                serviceTemplateLevel = ServiceTemplateLevel.ClientDefined;

                                            return Context.GetServiceTemplatesForServiceProviderQuery(roleId, (int)serviceTemplateLevel);
                                        },
                                        new[]
                                        {
                                            new ContextRelationshipFilter
                                                {
                                                    EntityMember = "OwnerClientId",
                                                    FilterValueGenerator = v => ((Client) v).Id,
                                                    RelatedContextType = typeof (Client)
                                                },
                                                  new ContextRelationshipFilter
                                                {
                                                    EntityMember = "OwnerServiceProviderId",
                                                    FilterValueGenerator = v => ((BusinessAccount) v).Id,
                                                    RelatedContextType = typeof (BusinessAccount)
                                                }
                                        });

            SetupDetailsLoading(selectedEntity => Context.GetServiceTemplateDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        #endregion

        #region Logic


        public override void DeleteEntity(ServiceTemplate entityToDelete)
        {
            //TODO Check Deletes are possible
            var entityCollection = new List<ServiceTemplate> { entityToDelete };

            DataManager.RemoveEntities(entityCollection);
        }

        protected override ServiceTemplate AddNewEntity(object commandParameter)
        {
            //Reuse CreateNewServiceTemplate logic
            return CreateNewServiceTemplate((ServiceTemplate)commandParameter);
        }

        //TODO: If a parent existed before, then was deleted, now being added again: Figure out if you can reconnect children to parent ServiceTemplate 
        private ServiceTemplate CreateNewServiceTemplate(ServiceTemplate parentServiceTemplate, string name = "New Service")
        {
            if (string.IsNullOrEmpty(name))
                name = "New Service";

            //Find if there is a recurring service context
            var recurringServiceContext = ContextManager.GetContext<RecurringService>();

            //Setup RecurringServiceDefined ServiceTemplate
            if (recurringServiceContext != null)
            {
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
                serviceTemplateChild.Id = recurringServiceContext.Id;
                recurringServiceContext.ServiceTemplate = serviceTemplateChild;

                ////To raise selectServiceTemplateObservable changed
                //this.LoadedServiceTemplates.Add(serviceTemplateChild);

                return serviceTemplateChild;
            }

            //Find if there is a client context
            var clientContext = ContextManager.GetContext<Client>();

            //Setup ClientDefined ServiceTemplate
            if (clientContext != null)
            {
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                serviceTemplateChild.OwnerClient = clientContext;

                ////To raise selectServiceTemplateObservable changed
                //this.LoadedServiceTemplates.Add(serviceTemplateChild);

                return serviceTemplateChild;
            }

            //(In Administrative Console) Find if there is a BusinessAccount context
            var serviceProvider = ContextManager.GetContext<BusinessAccount>();
            if (serviceProvider != null)
            {
                if (serviceProvider.Id == BusinessAccountsDesignData.FoundOps.Id)
                {
                    //If FoundOPS: create a FoundOPS defined template
                    var newServiceTemplate = new ServiceTemplate { ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined, Name = name };
                    serviceProvider.ServiceTemplates.Add(newServiceTemplate);

                    ////To raise selectServiceTemplateObservable changed
                    //this.LoadedServiceTemplates.Add(newServiceTemplate);

                    return newServiceTemplate;
                }

                throw new NotImplementedException("New service templates should inherit from a FoundOPS template.");
            }

            return null;
        }

        #endregion
    }
}