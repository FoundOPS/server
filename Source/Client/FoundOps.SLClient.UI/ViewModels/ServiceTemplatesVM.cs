using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.Services;
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
        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplatesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServiceTemplatesVM()
            : base(new[] { typeof(BusinessAccount), typeof(Client) }, true, true)
        {
            SetupDataLoading();

            #region IAddToDeleteFromSource<ServiceTemplate> Implementation

            MemberPath = "Name";

            //In the Administrative Console, creating a new FoundOPS template
            CreateNewItem = name =>
                                {
                                    NavigateToThis();

                                    if (string.IsNullOrEmpty(name))
                                        name = "New Service";

                                    //Find if the BusinessAccount context is FoundOPS
                                    var serviceProvider = ContextManager.GetContext<BusinessAccount>();
                                    if (serviceProvider == null ||
                                        serviceProvider.Id != BusinessAccountsDesignData.FoundOps.Id)
                                        throw new NotImplementedException(
                                            "New service templates should inherit from a FoundOPS template.");

                                    var newServiceTemplate = new ServiceTemplate
                                                                 {
                                                                     ServiceTemplateLevel =
                                                                         ServiceTemplateLevel.FoundOpsDefined,
                                                                     Name = name
                                                                 };
                                    serviceProvider.ServiceTemplates.Add(newServiceTemplate);

                                    SelectedEntity = newServiceTemplate;

                                    return newServiceTemplate;
                                };

            //TODO Make the comparer gets used by the SearchSuggestionsHelper
            CustomComparer = new ServiceTemplateIsAncestorOrDescendent();

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
                                            {
                                                //Filter the suggestions based on the current BusinessAccount context
                                                var businessAccountContext =
                                                    ContextManager.GetContext<BusinessAccount>();

                                                //If there is no context return nothing
                                                if (businessAccountContext == null) return;

                                                //You will only add new service templates to FoundOPS. This methods is used for adding existing service templates.
                                                //So if the current account is FoundOPS return nothing.
                                                if (businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId)
                                                    return;

                                                //Search the FoundOPS ServiceTemplates
                                                SearchSuggestionsHelper(autoCompleteBox, () =>
                                                                                         Manager.Data.Context.
                                                                                             SearchServiceTemplatesForServiceProviderQuery
                                                                                             (Manager.Context.RoleId,
                                                                                              searchText,
                                                                                              (int)
                                                                                              ServiceTemplateLevel.
                                                                                                  FoundOpsDefined));
                                            };

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
                                            if (businessAccountContext != null &&
                                                businessAccountContext.Id != BusinessAccountsConstants.FoundOpsId)
                                                serviceTemplateLevel = ServiceTemplateLevel.ServiceProviderDefined;

                                            var clientContext = ContextManager.GetContext<Client>();
                                            if (clientContext != null)
                                                serviceTemplateLevel = ServiceTemplateLevel.ClientDefined;

                                            return Context.GetServiceTemplatesForServiceProviderQuery(roleId,
                                                                                                      (int)
                                                                                                      serviceTemplateLevel);
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

            SetupDetailsLoading(
                selectedEntity =>
                Context.GetServiceTemplateDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
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
            var parentServiceTemplate = commandParameter as ServiceTemplate;

            //Creating a new service template (on the FoundOPS level)
            if (parentServiceTemplate == null)
                return CreateNewItem(null);

            //Reuse CreateChildServiceTemplate logic
            CreateChildServiceTemplate((ServiceTemplate)commandParameter,
                                       newServiceTemplate => SelectedEntity = newServiceTemplate);

            return null;
        }

        /// <summary>
        /// Creates a child of the ServiceTemplate.
        /// </summary>
        /// <param name="parentServiceTemplate">The parentServiceTemplate.</param>
        /// <param name="callback">An optional callback.</param>
        /// <param name="name"></param>
        public void CreateChildServiceTemplate(ServiceTemplate parentServiceTemplate, Action<ServiceTemplate> callback = null, string name = null)
        {
            if (parentServiceTemplate == null)
                throw new Exception("Cannot create a child service template without a parent");

            //When this is completed it will call this method
            Action<ServiceTemplate> completed = childServiceTemplate =>
            {
                NavigateToThis();
                SelectedEntity = childServiceTemplate;
                if (callback != null)
                    callback(childServiceTemplate);
            };

            //Make sure this is in a proper context to be creating a service template

            //Find if there is a RecurringService, Client, or ServiceProvider context
            var recurringServiceContext = ContextManager.GetContext<RecurringService>();
            var clientContext = ContextManager.GetContext<Client>();
            var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();

            if (recurringServiceContext == null && clientContext == null && serviceProviderContext == null)
                throw new Exception("Cannot create Service Template outside of a context.");

            //Load the parent's ServiceTemplate's details, then create the child
            Context.Load(Context.GetServiceTemplateDetailsForRoleQuery(ContextManager.RoleId, parentServiceTemplate.Id),
            loadOp =>
            {
                if (loadOp.HasError)
                    throw new Exception("Could not create Service Template. Please try again.");

                parentServiceTemplate.DetailsLoaded = true;

                //Setup RecurringServiceDefined ServiceTemplate
                if (recurringServiceContext != null)
                {
                    var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
                    serviceTemplateChild.Id = recurringServiceContext.Id;
                    recurringServiceContext.ServiceTemplate = serviceTemplateChild;

                    completed(serviceTemplateChild);
                    return;
                }

                //Setup ClientDefined ServiceTemplate
                if (clientContext != null)
                {
                    var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                    serviceTemplateChild.OwnerClient = clientContext;
                    completed(serviceTemplateChild);
                    return;
                }

                //Setup ServiceProvider ServiceTemplate
                if (serviceProviderContext != null)
                {
                    var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
                    serviceTemplateChild.OwnerServiceProvider = serviceProviderContext;
                    completed(serviceTemplateChild);
                    return;
                }

                throw new Exception("Could not create Service Template. Please try again.");
            }, null);
        }

        #endregion
    }
}