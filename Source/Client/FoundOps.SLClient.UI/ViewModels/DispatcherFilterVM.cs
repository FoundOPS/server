using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.MVVM.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A VM for filtering various parts of the dispatcher
    /// </summary>
    [ExportViewModel("DispatcherFilterVM")]
    public class DispatcherFilterVM : DataFedVM
    {
        #region Public Properties

        private ObservableCollection<ServiceTemplate> _loadedServiceTemplates;
        /// <summary>
        /// Collection of loaded ServiceTemplates for the ServiceProvider.
        /// </summary>
        public ObservableCollection<ServiceTemplate> LoadedServiceTemplates
        {
            get { return _loadedServiceTemplates; }
            private set
            {
                _loadedServiceTemplates = value;
                this.RaisePropertyChanged("LoadedServiceTemplates");
            }
        }

        private ObservableCollection<Region> _loadedRegions;
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<Region> LoadedRegions
        {
            get { return _loadedRegions; }
            private set
            {
                _loadedRegions = value;
                this.RaisePropertyChanged("LoadedRegions");
            }
        }

        private readonly ObservableCollection<ServiceTemplateOption> _serviceTemplateOptions = new ObservableCollection<ServiceTemplateOption>();
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<ServiceTemplateOption> ServiceTemplateOptions { get { return _serviceTemplateOptions; } }

        private readonly ObservableCollection<string> _selectedRegions = new ObservableCollection<string>();
        /// <summary>
        /// Gets the selected regions of routes to display.
        /// </summary>
        public ObservableCollection<string> SelectedRegions { get { return _selectedRegions; } }

        private readonly ObservableCollection<string> _selectedRouteTypes = new ObservableCollection<string>();
        /// <summary>
        /// Gets the selected route types of routes to display.
        /// </summary>
        public ObservableCollection<string> SelectedRouteTypes { get { return _selectedRouteTypes; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherFilterVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public DispatcherFilterVM()
        {
            SetupDataLoading();
        }

        private void SetupDataLoading()
        {
            //Whenever the RoleId updates load the service templates required for adding new Clients
            Manager.Context.RoleIdObservable.ObserveOnDispatcher().Subscribe(roleId =>
            {
                #region load the service templates required for adding new Clients

                //Load the service templates
                var serviceTemplatesQuery = Manager.Data.DomainContext.GetServiceTemplatesForServiceProviderQuery(Manager.Context.RoleId, (int)ServiceTemplateLevel.ServiceProviderDefined);

                Manager.Data.DomainContext.Load(serviceTemplatesQuery, lo =>
                {
                    LoadedServiceTemplates = new ObservableCollection<ServiceTemplate>(lo.Entities);

                    ServiceTemplateOptions.Clear();
                    foreach (var serviceTemplate in LoadedServiceTemplates)
                        ServiceTemplateOptions.Add(new ServiceTemplateOption(serviceTemplate));
                }, null);

                #endregion
            });

            //Setup an observable for whenever the Dispatcher is entered/reentered
            var enteredDispatcher = MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher"));
            enteredDispatcher.ObserveOnDispatcher().Subscribe(_ =>
            {
                var regionsQuery = Manager.Data.DomainContext.GetRegionsForServiceProviderQuery(Manager.Context.RoleId);

                Manager.Data.DomainContext.Load(regionsQuery, lo =>
                {
                    LoadedRegions = new ObservableCollection<Region>(lo.Entities);
                }, null);
            });
        }

        #endregion
    }

    /// <summary>
    /// An option to select or not select a ServiceTemplate.
    /// </summary>
    public class ServiceTemplateOption : ThreadableNotifiableObject
    {
        #region Public Properties

        private readonly ServiceTemplate _serviceTemplate;
        /// <summary>
        /// Gets the selected regions of routes to display.
        /// </summary>
        public ServiceTemplate ServiceTemplate { get { return _serviceTemplate; } }

        private bool _isSelected = true;
        /// <summary>
        /// Whether or not this is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTemplateOption"/> class.
        /// </summary>
        /// <param name="serviceTemplate">The service template.</param>
        public ServiceTemplateOption(ServiceTemplate serviceTemplate)
        {
            _serviceTemplate = serviceTemplate;
        }

        #endregion
    }
}
