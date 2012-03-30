using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Silverlight.UI.Interfaces;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model to manage adding, modifying, and deleting ServiceTemplates.
    /// </summary>
    [ExportViewModel("ServiceTemplatesVM")]
    public class ServiceTemplatesVM : InfiniteAccordionVM<ServiceTemplate>, IAddToDeleteFromSource<ServiceTemplate>
    {
        #region Public Properties

        private readonly Subject<IEnumerable<ServiceTemplate>> _serviceTemplatesForContext = new Subject<IEnumerable<ServiceTemplate>>();
        private ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _serviceTemplatesForContextHelper;
        /// <summary>
        /// This is for use by the ServiceProvider blocks. The QueryableCollectionView is used by the FoundOPS admin console.
        /// In the future these VMs should probably be split up.
        /// This will return the ClientContext's ServiceTemplates or the ServiceProvider's ServiceTemplates.
        /// </summary>
        public IEnumerable<ServiceTemplate> ServiceTemplatesForContext { get { return _serviceTemplatesForContextHelper.Value; } }

        #region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        public Func<string, ServiceTemplate> CreateNewItem { get; private set; }

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
                                        throw new NotImplementedException("New service templates should inherit from a FoundOPS template.");

                                    var newServiceTemplate = new ServiceTemplate
                                    {
                                        ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined,
                                        Name = name
                                    };
                                    serviceProvider.ServiceTemplates.Add(newServiceTemplate);

                                    SelectedEntity = newServiceTemplate;

                                    return newServiceTemplate;
                                };

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
            {
                //If there is no service provider context return nothing
                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                if (businessAccountContext == null || businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId)
                    return;

                //Search the FoundOPS ServiceTemplates
                SearchSuggestionsHelper(autoCompleteBox, () =>
                     Manager.Data.DomainContext.SearchServiceTemplatesForServiceProviderQuery(Manager.Context.RoleId, businessAccountContext.Id, searchText));
            };

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            #region Setup ServiceProvider users data loading

            //Setup the _serviceTemplatesForContextHelper. Set the initial value to the ServiceProvider's ServiceTemplates
            _serviceTemplatesForContextHelper = _serviceTemplatesForContext.ToProperty(this, x => x.ServiceTemplatesForContext, ContextManager.CurrentServiceTemplates);

            //The ServiceTemplatesForContext will either be the ClientContext.ServiceTemplates or the current ServiceProvider's ServiceTemplates

            //Update when the ServiceProvider's ServiceTemplates are loaded and when the ClientContext changes
            //a) if there is no ClientContext, push the ServiceProvider's SerivceTemplates
            //b) if there is a ClientContext, push the ClientContext's ServiceTemplates
            ContextManager.CurrentServiceTemplatesObservable.AsGeneric()
            .Merge(ContextManager.GetContextObservable<Client>().AsGeneric())
                //Throttle to allow time for the values to propogate
            .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
            .Subscribe(_ =>
            {
                var clientContext = ContextManager.GetContext<Client>();
                _serviceTemplatesForContext.OnNext(clientContext == null ? ContextManager.CurrentServiceTemplates : clientContext.ServiceTemplates);
            });

            LoadOperation<ServiceTemplate> clientServiceTemplatesLoadOperation = null;
            //Whenever the ClientContext changes load the it's ServiceTemplates with details
            ContextManager.GetContextObservable<Client>().Where(c => c != null).Subscribe(client =>
            {
                //Cancel the last load
                if (clientServiceTemplatesLoadOperation != null && clientServiceTemplatesLoadOperation.CanCancel)
                    clientServiceTemplatesLoadOperation.Cancel();

                //Do not try to load ServiceTemplates for a Client that does not exist yet
                if (client.EntityState == EntityState.New)
                    return;

                var query = DomainContext.GetClientServiceTemplatesQuery(ContextManager.RoleId, client.Id);
                clientServiceTemplatesLoadOperation = DomainContext.Load(query);
            });

            #endregion

            #region Setup Admin Console data loading

            SetupContextDataLoading(roleId =>
            {
                //Only load service templates of the current level

                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                //If the current role is not a FoundOPS role do not load anything
                //because the current ServiceProvider's service templates are automatically loaded
                if (businessAccountContext == null || ContextManager.OwnerAccount.Id != BusinessAccountsConstants.FoundOpsId)
                    return null;

                var serviceTemplateLevel = businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId
                                               ? (int)ServiceTemplateLevel.FoundOpsDefined
                                               : (int)ServiceTemplateLevel.ServiceProviderDefined;

                return DomainContext.GetServiceTemplatesForServiceProviderQuery(roleId, serviceTemplateLevel);
            },
            new[]{
                new ContextRelationshipFilter
                {
                    EntityMember = "OwnerServiceProviderId",
                    FilterValueGenerator = v => ((BusinessAccount) v).Id,
                    RelatedContextType = typeof (BusinessAccount)
               }});

            SetupDetailsLoading(selectedEntity =>
            {
                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                //If the current role is not a FoundOPS role do not load anything
                //because the current ServiceProvider's service templates are automatically loaded
                if (businessAccountContext == null ||
                    ContextManager.OwnerAccount.Id != BusinessAccountsConstants.FoundOpsId)
                    return null;

                return DomainContext.GetServiceTemplateDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id);
            });

            #endregion
        }

        #endregion

        #region Logic

        public override void DeleteEntity(ServiceTemplate entityToDelete)
        {
            DomainContext.ServiceTemplates.Remove(entityToDelete);
        }

        protected override ServiceTemplate AddNewEntity(object commandParameter)
        {
            var parentServiceTemplate = commandParameter as ServiceTemplate;

            //Creating a new service template (on the FoundOPS level)
            if (parentServiceTemplate == null)
                return CreateNewItem(null);

            //Reuse CreateChildServiceTemplate logic
            CreateChildServiceTemplate((ServiceTemplate)commandParameter, newServiceTemplate => SelectedEntity = newServiceTemplate);

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
            DomainContext.Load(DomainContext.GetServiceTemplateDetailsForRoleQuery(ContextManager.RoleId, parentServiceTemplate.Id),
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