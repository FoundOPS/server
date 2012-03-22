using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the ServiceTemplates
    /// </summary>
    [ExportViewModel("ServiceTemplatesVM")]
    public class ServiceTemplatesVM : InfiniteAccordionVM<ServiceTemplate> //, IAddToDeleteFromSource<ServiceTemplate>
    {
        #region Public Properties

        //#region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        //public Func<string, ServiceTemplate> CreateNewItem { get; private set; }

        //public IEqualityComparer<object> CustomComparer { get; set; }

        //private readonly ObservableAsPropertyHelper<IEnumerable> _foundopsServiceTemplates;
        //public IEnumerable ExistingItemsSource { get { return _foundopsServiceTemplates.Value; } }

        //public string MemberPath { get; private set; }

        ////TODO
        //public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions
        //{
        //    get { return null; }
        //}

        //#endregion

        //#endregion

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

            //#region Setup DomainCollectionView

            ////Setup DomainCollectionView based on the current client context
            //this.ContextManager.GetContextObservable<Client>().ObserveOnDispatcher().Subscribe(_ =>
            //{
            //    //If there is a Client context
            //    var clientContext = ContextManager.GetContext<Client>();
            //    if (clientContext != null)
            //    {
            //        var clientContextDCV = DomainCollectionViewFactory<ServiceTemplate>.GetDomainCollectionView(clientContext.ServiceTemplates);
            //        clientContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined;
            //        CollectionViewObservable.OnNext(clientContextDCV);
            //        return;
            //    }

            //    //if not, find the current ServiceProvider's ServiceTemplates
            //    if (ContextManager.OwnerAccount != null && ContextManager.OwnerAccount is BusinessAccount)
            //    {
            //        var serviceProviderContextDCV = DomainCollectionViewFactory<ServiceTemplate>
            //            .GetDomainCollectionView(new EntityList<ServiceTemplate>(Context.ServiceTemplates, ServiceTemplatesForServiceProvider));
            //        serviceProviderContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined;
            //        CollectionViewObservable.OnNext(serviceProviderContextDCV);
            //        return;
            //    }
            //});

            ////Setup DomainCollectionView based on the BusinessAccount context (for the Administrative console)
            //this.ContextManager.GetContextObservable<BusinessAccount>().ObserveOnDispatcher().Subscribe(_ =>
            //{
            //    var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
            //    if (businessAccountContext == null) return;

            //    var serviceProviderContextDCV = DomainCollectionViewFactory<ServiceTemplate>.GetDomainCollectionView(businessAccountContext.ServiceTemplates);
            //    serviceProviderContextDCV.Filter += st => ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined
            //        || ((ServiceTemplate)st).ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined;
            //    CollectionViewObservable.OnNext(serviceProviderContextDCV);
            //});

            //#region IAddToDeleteFromSource<ServiceTemplate> Implementation

            ////TODO Optimization
            //////Whenever the _loadedUserAccounts changes notify ExistingItemsSource changed
            ////_foundopsServiceTemplates = foundOPSServiceTemplates.ToProperty(this, x => x.ExistingItemsSource);

            //MemberPath = "Name";

            //CreateNewItem = name =>
            //{
            //    var newServiceTemplate = CreateNewServiceTemplate(null, name);
            //    SelectedEntity = newServiceTemplate;
            //    NavigateToThis();
            //    return newServiceTemplate;
            //};

            //CustomComparer = new ServiceTemplateIsAncestorOrDescendent();

            //#endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            //Force load the entities when in a related types view
            //this is because VDCV will only normally load when a virtual item is loaded onto the screen
            //virtual items will not always load because in clients context the gridview does not always show (sometimes it is in single view)
            SetupContextDataLoading(roleId => Context.GetServiceTemplatesForServiceProviderQuery(roleId),
                                    new[]
                                        {
                                            new ContextRelationshipFilter
                                                {
                                                    EntityMember = "PartyId",
                                                    FilterValueGenerator = v => ((Client) v).Id,
                                                    RelatedContextType = typeof (Client)
                                                },
                                                  new ContextRelationshipFilter
                                                {
                                                    EntityMember = "RegionId",
                                                    FilterValueGenerator = v => ((Region) v).Id,
                                                    RelatedContextType = typeof (Region)
                                                }
                                        }, true);

        }

        #endregion

        #region Logic

        public override void DeleteEntity(ServiceTemplate entityToDelete)
        {
            var entityCollection = new List<ServiceTemplate> { entityToDelete };

            DataManager.RemoveEntities(entityCollection);
        }

        ////Use CreateNewServiceTemplate logic
        //protected override ServiceTemplate AddNewEntity(object commandParameter)
        //{
        //    return CreateNewServiceTemplate((ServiceTemplate)commandParameter);
        //}

        ////TODO: If a parent existed before, then was deleted, now being added again: Figure out if you can reconnect children to parent ServiceTemplate 
        //private ServiceTemplate CreateNewServiceTemplate(ServiceTemplate parentServiceTemplate, string name = "New Service")
        //{
        //    if (string.IsNullOrEmpty(name))
        //        name = "New Service";

        //    //Find if there is a recurring service context
        //    var recurringServiceContext = ContextManager.GetContext<RecurringService>();

        //    //Setup RecurringServiceDefined ServiceTemplate
        //    if (recurringServiceContext != null)
        //    {
        //        var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
        //        serviceTemplateChild.Id = recurringServiceContext.Id;
        //        recurringServiceContext.ServiceTemplate = serviceTemplateChild;

        //        //To raise selectServiceTemplateObservable changed
        //        this.LoadedServiceTemplates.Add(serviceTemplateChild);

        //        return serviceTemplateChild;
        //    }

        //    //Find if there is a client context
        //    var clientContext = ContextManager.GetContext<Client>();

        //    //Setup ClientDefined ServiceTemplate
        //    if (clientContext != null)
        //    {
        //        var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
        //        serviceTemplateChild.OwnerClient = clientContext;

        //        //To raise selectServiceTemplateObservable changed
        //        this.LoadedServiceTemplates.Add(serviceTemplateChild);

        //        return serviceTemplateChild;
        //    }

        //    //(In Administrative Console) Find if there is a BusinessAccount context
        //    var serviceProvider = ContextManager.GetContext<BusinessAccount>();
        //    if (serviceProvider != null)
        //    {
        //        if (serviceProvider.Id == BusinessAccountsDesignData.FoundOps.Id)
        //        {
        //            //If FoundOPS: create a FoundOPS defined template
        //            var newServiceTemplate = new ServiceTemplate { ServiceTemplateLevel = ServiceTemplateLevel.FoundOpsDefined, Name = name };
        //            serviceProvider.ServiceTemplates.Add(newServiceTemplate);

        //            //To raise selectServiceTemplateObservable changed
        //            this.LoadedServiceTemplates.Add(newServiceTemplate);

        //            return newServiceTemplate;
        //        }

        //        throw new NotImplementedException("New service templates should inherit from a FoundOPS template.");
        //    }

        //    return null;
        //}

        #endregion
    }
}