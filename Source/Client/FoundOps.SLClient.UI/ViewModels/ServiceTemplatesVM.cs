using System.Collections;
using System.Windows;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model to manage adding, modifying, and deleting ServiceTemplates.
    /// </summary>
    [ExportViewModel("ServiceTemplatesVM")]
    public class ServiceTemplatesVM : InfiniteAccordionVM<ServiceTemplate, ServiceTemplate>, IAddToDeleteFromSource<ServiceTemplate>,
        IAddToDeleteFromDestination<Field>, IAddNewExisting<Field>, IRemoveDelete<Field>
    {
        #region Public Properties

        #region Service Provider users

        private readonly Subject<IEnumerable<ServiceTemplate>> _serviceTemplatesForContext = new Subject<IEnumerable<ServiceTemplate>>();

        private ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _serviceTemplatesForContextHelper;
        /// <summary>
        /// This is for use by the ServiceProvider blocks.
        /// In the future these VMs should probably be split up.
        /// This will return the ClientContext's ServiceTemplates or the ServiceProvider's ServiceTemplates.
        /// </summary>
        public IEnumerable<ServiceTemplate> ServiceTemplatesForContext { get { return _serviceTemplatesForContextHelper.Value; } }

        private ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _additionalServiceTemplatesForClientHelper;
        /// <summary>
        /// This is for use by the AvailableServices part of Clients. 
        /// It will return the ServiceProviders ServiceTemplates that the current client context does not have yet.
        /// </summary>
        public IEnumerable<ServiceTemplate> AdditionalServiceTemplatesForClient { get { return _additionalServiceTemplatesForClientHelper.Value; } }

        #endregion

        #region Admin Console users

        private readonly Subject<IEnumerable<ServiceTemplate>> _businessAccountContextServiceTemplates = new Subject<IEnumerable<ServiceTemplate>>();

        private ObservableAsPropertyHelper<IEnumerable<ServiceTemplate>> _businessAccountContextServiceTemplatesHelper;
        /// <summary>
        /// This is for use by the Admin Console blocks.
        /// In the future these VMs should probably be split up.
        /// This will return the BusinessAccountContext's ServiceTemplates.
        /// </summary>
        public IEnumerable<ServiceTemplate> BusinessAccountContextServiceTemplates { get { return _businessAccountContextServiceTemplatesHelper.Value; } }

        #endregion

        #region Implementation of IAddToDeleteFromDestination

        private readonly List<IDisposable> _addToDeleteFromSubscriptions = new List<IDisposable>();
        /// <summary>
        /// Subscribes to the add delete from control observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFrom(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Field))
            {
                _addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemField(existingItem)));
                _addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemField()));
            }
        }

        /// <summary>
        /// Disposes the subscriptions to the add delete from control observables.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="type">Type of the source.</param>
        public void UnlinkAddToDeleteFrom(AddToDeleteFrom c, Type type)
        {
            foreach (var subscription in _addToDeleteFromSubscriptions)
                subscription.Dispose();

            _addToDeleteFromSubscriptions.Clear();
        }

        /// <summary>
        /// Gets the fields destination items source.
        /// </summary>
        public IEnumerable FieldsDestinationItemsSource
        {
            get { return SelectedEntity == null ? null : SelectedEntity.Fields; }
        }

        #endregion


        #region Implementation of IAddToDeleteFromSource<ServiceTemplate>

        public Func<string, ServiceTemplate> CreateNewItem { get; private set; }

        public string MemberPath { get; private set; }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        #endregion

        #region Implementation of IAddNewExisting<Field> & IRemoveDelete<Field>

        /// <summary>
        /// An action to add an existing Field to the current BusinessAccount.
        /// </summary>
        public Action<object> AddExistingItemField { get; private set; }
        Action<object> IAddNewExisting<Field>.AddExistingItem { get { return AddExistingItemField; } }

        /// <summary>
        /// An action to add a new Field to the current BusinessAccount.
        /// </summary>
        public Func<string, Field> AddNewItemField { get; private set; }
        Func<string, Field> IAddNew<Field>.AddNewItem { get { return AddNewItemField; } }

        /// <summary>
        /// An action to remove a Field from the current BusinessAccount.
        /// </summary>
        public Func<Field> RemoveItemField { get; private set; }
        Func<Field> IRemove<Field>.RemoveItem { get { return RemoveItemField; } }

        /// <summary>
        /// An action to remove a Field from the current BusinessAccount and delete it.
        /// </summary>
        public Func<Field> DeleteItemField { get; private set; }
        Func<Field> IRemoveDelete<Field>.DeleteItem { get { return DeleteItemField; } }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplatesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ServiceTemplatesVM()
            : base(new[] { typeof(BusinessAccount) }, true, true)
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
                    serviceProvider.Id != BusinessAccountsConstants.FoundOpsId)
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

            ManuallyUpdateSuggestions = autoCompleteBox =>
            {
                //If there is no service provider context return nothing
                var businessAccountContext = ContextManager.GetContext<BusinessAccount>();
                if (businessAccountContext == null || businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId)
                    return;

                //Search the FoundOPS ServiceTemplates
                SearchSuggestionsHelper(autoCompleteBox, () =>
                     Manager.Data.DomainContext.SearchServiceTemplatesForServiceProviderQuery(Manager.Context.RoleId, businessAccountContext.Id, autoCompleteBox.SearchText));
            };

            #endregion

            #region Implementation of IAddNewExisting<Field> & IRemoveDelete<Field>

            AddNewItemField = name =>
            {
                throw new Exception("Should not add new items through AddToDeleteFrom control. Still using old Add/Delete");
            };

            AddExistingItemField = existingItem =>
            {
                var parentField = (Field)existingItem;
                var childField = parentField.MakeChild();

                SelectedEntity.Fields.Add(childField);
                VM.Fields.SelectedEntity = childField;
            };

            RemoveItemField = () =>
            {
                throw new Exception("Should not remove fields. Only delete them.");
            };

            DeleteItemField = () =>
            {
                var selectedField = VM.Fields.SelectedEntity;
                if (selectedField != null)
                    VM.Fields.DeleteEntity(selectedField);

                return selectedField;
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

            //Setup ServiceTemplatesForContext
            //Whenever the ClientContext changes
            //a) if there is no ClientContext, push the ServiceProvider's ServiceTemplates
            //b) if there is a ClientContext, push the ClientContext's ServiceTemplates
            ContextManager.GetContextObservable<Client>().SelectLatest(clientContext =>
            {
                if (clientContext == null)
                    return ContextManager.CurrentServiceTemplatesObservable;

                //Return the ClientDefined level service templates
                //They will change whenever the details are loaded and the clientContext ServiceTemplates change
                return Observable2.FromPropertyChangedPattern(clientContext, x => x.DetailsLoaded).AsGeneric()
                       .Merge(clientContext.ServiceTemplates.FromCollectionChangedAndNow().AsGeneric())
                       .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
                       .Select(_ => clientContext.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ClientDefined));
            }).Subscribe(_serviceTemplatesForContext);

            //Setup AdditionalServiceTemplatesForClient
            var additionalServiceTemplatesForClient = ContextManager.GetContextObservable<Client>()
                .SelectLatest(clientContext =>
                {
                    if (clientContext == null)
                        return Observable.Empty<IEnumerable<ServiceTemplate>>();

                    //Whenever the clientContext's service templates change and when the ServiceProvider's ServiceTemplates change
                    //update what are the remaining service templates for the client
                    var updateAdditionalServiceTemplates = clientContext.ServiceTemplates.FromCollectionChangedAndNow().AsGeneric()
                        .Merge(ContextManager.CurrentServiceTemplatesObservable.AsGeneric()).Throttle(TimeSpan.FromMilliseconds(100))
                        .ObserveOnDispatcher();

                    return updateAdditionalServiceTemplates.Select(_ =>
                    {
                        if (ContextManager.CurrentServiceTemplates == null)
                            return new ServiceTemplate[] { };

                        //Remove already existing ServiceTemplates
                        var availableServiceTemplatesForClient = ContextManager.CurrentServiceTemplates.Where(spst => !clientContext.ServiceTemplates.Any(cst => cst.OwnerServiceTemplateId == spst.Id));

                        return availableServiceTemplatesForClient;
                    });
                });

            _additionalServiceTemplatesForClientHelper = additionalServiceTemplatesForClient.ToProperty(this, x => x.AdditionalServiceTemplatesForClient);

            #endregion

            #region Setup Admin Console data loading

            _businessAccountContextServiceTemplatesHelper = _businessAccountContextServiceTemplates.ToProperty(this, x => x.BusinessAccountContextServiceTemplates);

            //Setup BusinessAccountContextServiceTemplates
            ContextManager.GetContextObservable<BusinessAccount>().SelectLatest(businessAccountContext =>
            {
                //If the current role is not a FoundOPS role do not load anything. This is only for the admin console
                if (ContextManager.OwnerAccount == null || ContextManager.OwnerAccount.Id != BusinessAccountsConstants.FoundOpsId)
                    return Observable.Empty<IEnumerable<ServiceTemplate>>();

                if (businessAccountContext == null)
                    return Observable.Empty<IEnumerable<ServiceTemplate>>();

                //Return the ServiceProviderLevel or FoundOPS level service templates
                //They will change whenever the details are loaded and the businessAccountContext.ServiceTemplates change
                return Observable2.FromPropertyChangedPattern(businessAccountContext, x => x.DetailsLoaded).AsGeneric()
                       .Merge(businessAccountContext.ServiceTemplates.FromCollectionChangedAndNow().AsGeneric())
                       .Throttle(TimeSpan.FromMilliseconds(100)).ObserveOnDispatcher()
                       .Select(_ =>
                       {
                           if (businessAccountContext.Id == BusinessAccountsConstants.FoundOpsId)
                               return businessAccountContext.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.FoundOpsDefined);
                           return businessAccountContext.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined);
                       });
            }).Subscribe(_businessAccountContextServiceTemplates);

            #endregion
        }

        #endregion

        #region Logic

        public override void DeleteEntity(ServiceTemplate entityToDelete)
        {
            if (entityToDelete.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
            {
                var result =
                    MessageBox.Show(
                        "Doing this will delete this Service Template and all of its children templates as well as any data assiciated with them. Are you sure this is what you want to do?",
                        "Stop and Think!", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.Cancel)
                    return;
            }
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
                //If this is in the FoundOPS admin console
                if (ContextManager.GetContext<BusinessAccount>() != null)
                    NavigateToThis();

                SelectedEntity = childServiceTemplate;
                if (callback != null)
                    callback(childServiceTemplate);
            };

            //Make sure this is in a proper context to be creating a service template

            //Find if there is a RecurringService, Client, or ServiceProvider context
            //var recurringServiceContext = ContextManager.GetContext<RecurringService>();
            var clientContext = ContextManager.GetContext<Client>();
            var serviceProviderContext = ContextManager.GetContext<BusinessAccount>();

            //NOTE: Recurring services are currently disabled
            //recurringServiceContext == null && 
            if (clientContext == null && serviceProviderContext == null)
                throw new Exception("Cannot create Service Template outside of a context.");
            if (clientContext != null)
            {
                //Setup ClientDefined ServiceTemplate
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);
                serviceTemplateChild.OwnerClient = clientContext;
                completed(serviceTemplateChild);
            }
            ////Setup RecurringServiceDefined ServiceTemplate
            //else if (recurringServiceContext != null)
            //{
            //    var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.RecurringServiceDefined);
            //    serviceTemplateChild.Id = recurringServiceContext.Id;
            //    recurringServiceContext.ServiceTemplate = serviceTemplateChild;

                    //    completed(serviceTemplateChild);
            //    return;
            //}
            //If in Admin Console, setup ServiceProvider defined context
            else if (serviceProviderContext != null)
            {
                //Setup BusinessAccountDefined ServiceTemplate
                var serviceTemplateChild = parentServiceTemplate.MakeChild(ServiceTemplateLevel.ServiceProviderDefined);
                serviceTemplateChild.OwnerServiceProvider = serviceProviderContext;
                completed(serviceTemplateChild);
            }
        }

        #endregion
    }
}