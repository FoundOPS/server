using System;
using System.Collections;
using System.ComponentModel;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.Services;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities.DesignData;
using Microsoft.Windows.Data.DomainServices;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the ServiceTemplates
    /// </summary>
    [ExportViewModel("ServiceTemplatesVM")]
    public class ServiceTemplatesVM : CoreEntityCollectionInfiniteAccordionVM<ServiceTemplate>,
          IAddToDeleteFromSource<ServiceTemplate>
    {
        #region Public Properties

        #region ServiceTemplate contexts

        private readonly ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _serviceTemplatesForServiceProvider;
        /// <summary>
        /// Returns all service provider service templates
        /// </summary>
        public IEnumerable<ServiceTemplate> ServiceTemplatesForServiceProvider { get { return _serviceTemplatesForServiceProvider.Value; } }

        private readonly ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _additionalServiceTemplatesForServiceProvider;
        /// <summary>
        /// Returns any service templates for service provider the current context does not already have
        /// </summary>
        public IEnumerable<ServiceTemplate> AdditionalServiceTemplatesForServiceProvider { get { return _additionalServiceTemplatesForServiceProvider.Value; } }

        private readonly ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _serviceTemplatesForClient;
        /// <summary>
        /// Returns service templates the current client has
        /// </summary>
        public IEnumerable<ServiceTemplate> ServiceTemplatesForClient { get { return _serviceTemplatesForClient.Value; } }

        private readonly ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _foundOPSServiceTemplates;
        /// <summary>
        /// Returns FoundOPS service templates.
        /// </summary>
        public IEnumerable<ServiceTemplate> FoundOPSServiceTemplates { get { return _foundOPSServiceTemplates.Value; } }

        #endregion

        private readonly ObservableAsPropertyHelper<ObservableCollection<ServiceTemplate>> _loadedServiceTemplates;
        /// <summary>
        /// Gets the loaded service templates.
        /// </summary>
        public ObservableCollection<ServiceTemplate> LoadedServiceTemplates { get { return _loadedServiceTemplates.Value; } }

        #region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        public Func<string, ServiceTemplate> CreateNewItem { get; private set; }

        public IEqualityComparer<object> CustomComparer { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable> _foundopsServiceTemplates;
        public IEnumerable ExistingItemsSource { get { return _foundopsServiceTemplates.Value; } }

        public string MemberPath { get; private set; }

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplatesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public ServiceTemplatesVM(DataManager dataManager)
            : base(dataManager, false)
        {
            //Subscribe to the ServiceTemplates query
            IsLoadingObservable = DataManager.Subscribe<ServiceTemplate>(DataManager.Query.ServiceTemplates, ObservationState, null);

            #region Setup ServiceTemplate context Data Properties

            var loadedServiceTemplates = DataManager.GetEntityListObservable<ServiceTemplate>(DataManager.Query.ServiceTemplates);
            //Setup LoadedServiceTemplates property
            _loadedServiceTemplates = loadedServiceTemplates.ToProperty(this, x => x.LoadedServiceTemplates, new ObservableCollection<ServiceTemplate>());

            //_collectionView = loadedServiceTemplates.Select(lst => new CollectionViewSource { Source = lst }.View)
            //    .ToProperty(this, x => x.CollectionView);

            //_collectionView.DistinctUntilChanged().ObserveOnDispatcher()
            //    .Subscribe(cv => cv.Filter += obj => EntityIsPartOfView((ServiceTemplate) obj, IsAddingNew));

            //Select the ServiceTemplates whenevever:
            //a) they are changed or set
            //b) the current context changes
            //c) the OwnerAccount changes
            //Condition: the ServiceTemplates are loaded and the OwnerAccount is loaded
            var selectServiceTemplateObservable =
                //a) they are changed or set
                loadedServiceTemplates.FromCollectionChangedOrSet()
                //b) the current context changes
                .Merge(this.ContextManager.ContextChangedObservable.Select(cc => LoadedServiceTemplates))
                //c) the OwnerAccount changes
                .Merge(this.ContextManager.OwnerAccountObservable.Select(oa => LoadedServiceTemplates))
                //Condition: the ServiceTemplates are loaded and the OwnerAccount is loaded
                .Where(sts => sts != null && ContextManager.OwnerAccount != null).Throttle(new TimeSpan(0, 0, 0, 0, 300));

            //Setup FoundOPSServiceTemplates

            var foundOPSServiceTemplates =
                selectServiceTemplateObservable.Select(lsts =>
                {
                    var serviceTemplatesForProvider = lsts.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined);
                    return serviceTemplatesForProvider.OrderBy(st => st.Name).AsEnumerable();
                });

            _foundOPSServiceTemplates = foundOPSServiceTemplates.ToProperty(this, x => x.FoundOPSServiceTemplates, new ObservableCollection<ServiceTemplate>());

            //Setup ServiceTemplatesForServiceProvider
            var serviceTemplatesForServiceProvider =
                selectServiceTemplateObservable.Select(lsts =>
                {
                    var serviceTemplatesForProvider = lsts.Where(st =>
                            st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined &&
                            st.OwnerServiceProviderId == ContextManager.OwnerAccount.Id);

                    return serviceTemplatesForProvider.OrderBy(st => st.Name).AsEnumerable();
                });

            _serviceTemplatesForServiceProvider = serviceTemplatesForServiceProvider.ToProperty(this, x => x.ServiceTemplatesForServiceProvider, new ObservableCollection<ServiceTemplate>());

            //Setup AdditionalServiceTemplatesForServiceProvider property

            var additionalServiceTemplatesForServiceProvider =
                serviceTemplatesForServiceProvider.Merge(AddCommand.Merge(this.DeleteCommand).Select(_ => LoadedServiceTemplates)).Select(lsts =>
                {
                    var serviceTemplatesForProvider = lsts.Where(st =>
                            st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined &&
                            st.OwnerServiceProviderId == ContextManager.OwnerAccount.Id);

                    Func<ServiceTemplate, bool> serviceTemplateExistsUnderContext = st => false;

                    var clientContext = ContextManager.GetContext<Client>();

                    if (clientContext != null)
                    {
                        //Check if there are any service templates with the current client's context
                        serviceTemplateExistsUnderContext =
                            serviceProviderServiceTemplate =>
                            serviceProviderServiceTemplate.ChildrenServiceTemplates.Any(st =>
                                st.ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined && st.OwnerClientId == clientContext.Id);
                    }

                    //Return any FoundOPS service templates that are not already part of the ServiceProvider's service templates
                    var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                    if (businessAccountContext != null)
                    {
                        //Check if there are any service templates with the current BusinessAccount's context
                        serviceTemplateExistsUnderContext =
                          serviceProviderServiceTemplate => serviceProviderServiceTemplate.ChildrenServiceTemplates.Any(st =>
                              st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined && st.OwnerServiceProviderId == businessAccountContext.Id);

                        return FoundOPSServiceTemplates.Where(st => !serviceTemplateExistsUnderContext(st)).OrderBy(st => st.Name).AsEnumerable();
                    }

                    //Return the ServiceTemplates that do not exist under the context
                    return serviceTemplatesForProvider.Where(st => !serviceTemplateExistsUnderContext(st)).OrderBy(st => st.Name).AsEnumerable();
                });

            _additionalServiceTemplatesForServiceProvider =
                additionalServiceTemplatesForServiceProvider.ToProperty(this, x => x.AdditionalServiceTemplatesForServiceProvider, new ObservableCollection<ServiceTemplate>());

            //Setup ServiceTemplatesForClient property (whenever selectServiceTemplateObservable or the Client's ServiceTemplates changes)

            var serviceTemplatesForClient =
                selectServiceTemplateObservable.Merge(ContextManager.GetContextObservable<Client>().Where(c => c != null)
                .SelectMany(c => c.ServiceTemplates.FromCollectionChanged()).Select(_ => LoadedServiceTemplates)).Throttle(new TimeSpan(0, 0, 0, 0, 250))
                .Select(lsts =>
                {
                    var clientContext = ContextManager.GetContext<Client>();
                    return clientContext == null ? null : clientContext.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined)
                        .OrderBy(st => st.Name).AsEnumerable();
                }).Where(sts => sts != null);

            _serviceTemplatesForClient = serviceTemplatesForClient.ToProperty(this, x => x.ServiceTemplatesForClient, new ObservableCollection<ServiceTemplate>());

            #endregion

            #region Setup DomainCollectionView

            //Setup DomainCollectionView based on the current client context
            this.ContextManager.GetContextObservable<Client>().ObserveOnDispatcher().Subscribe(_ =>
            {
                //If there is a Client context
                var clientContext = ContextManager.GetContext<Client>();
                if (clientContext != null)
                {
                    var clientContextDCV = DomainCollectionViewFactory<ServiceTemplate>.GetDomainCollectionView(clientContext.ServiceTemplates);
                    clientContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined;
                    DomainCollectionViewObservable.OnNext(clientContextDCV);
                    return;
                }

                //if not, find the current ServiceProvider's ServiceTemplates
                if (ContextManager.OwnerAccount != null && ContextManager.OwnerAccount is BusinessAccount)
                {
                    var serviceProviderContextDCV = DomainCollectionViewFactory<ServiceTemplate>
                        .GetDomainCollectionView(new EntityList<ServiceTemplate>(Context.ServiceTemplates, ServiceTemplatesForServiceProvider));
                    serviceProviderContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined;
                    DomainCollectionViewObservable.OnNext(serviceProviderContextDCV);
                    return;
                }
            });

            //Setup DomainCollectionView based on the BusinessAccount context (for the Administrative console)
            this.ContextManager.GetContextObservable<BusinessAccount>().ObserveOnDispatcher().Subscribe(_ =>
            {
                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                if (businessAccountContext == null) return;

                var serviceProviderContextDCV = DomainCollectionViewFactory<ServiceTemplate>.GetDomainCollectionView(businessAccountContext.ServiceTemplates);
                serviceProviderContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined
                    || ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined;
                DomainCollectionViewObservable.OnNext(serviceProviderContextDCV);
            });


            //Whenever the DCV changes, sort by Name
            this.DomainCollectionViewObservable.Subscribe(dcv => dcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending)));

            #endregion

            //Whenever the _loadedUserAccounts changes notify ExistingItemsSource changed
            _foundopsServiceTemplates = foundOPSServiceTemplates.ToProperty(this, x => x.ExistingItemsSource);

            MemberPath = "Name";

            CreateNewItem = name => CreateNewServiceTemplate(null, name);

            CustomComparer = new ServiceTemplateIsAncestorOrDescendent();
        }

        #region Logic

        //Do not use default filter. This filters by context (ServiceTemplateLevel)
        public override void UpdateFilter() { }

        protected override ServiceTemplate AddNewEntity(object commandParameter)
        {
            return CreateNewServiceTemplate((ServiceTemplate)commandParameter);
        }

        //TODO: If a parent existed before, then was deleted, now being added again: Figure out if you can reconnect children to parent ServiceTemplate 
        private ServiceTemplate CreateNewServiceTemplate(ServiceTemplate parentServiceTemplate, string name = "New Service")
        {
            //Find if there is a recurring service context
            var recurringServiceContext = ContextManager.GetContext<RecurringService>();

            //Setup RecurringServiceDefined ServiceTemplate
            if (recurringServiceContext != null)
            {
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
                serviceTemplateChild.Id = recurringServiceContext.Id;
                recurringServiceContext.ServiceTemplate = serviceTemplateChild;

                //TODO: Check if necessary
                this.Context.ServiceTemplates.Add(serviceTemplateChild);

                return serviceTemplateChild;
            }

            //Find if there is a client context
            var clientContext = ContextManager.GetContext<Client>();

            //Setup ClientDefined ServiceTemplate
            if (clientContext != null)
            {
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                serviceTemplateChild.OwnerClient = clientContext;

                //TODO: Check if necessary
                this.Context.ServiceTemplates.Add(serviceTemplateChild);

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
                    return newServiceTemplate;
                }

                throw new NotImplementedException("New service templates should inherit from a FoundOPS template.");
            }

            return null;
        }

        #endregion
    }
}