using System;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using ReactiveUI;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A VM for filtering various parts of the dispatcher
    /// </summary>
    [ExportViewModel("DispatcherFilterVM")]
    public class DispatcherFilterVM : DataFedVM
    {
        #region Public Properties

        private ObservableCollection<ServiceTemplate> _loadedServiceProviderServiceTemplates;
        /// <summary>
        /// Collection of loaded ServiceTemplates for the ServiceProvider.
        /// </summary>
        public ObservableCollection<ServiceTemplate> LoadedServiceProviderServiceTemplates
        {
            get { return _loadedServiceProviderServiceTemplates; }
            private set
            {
                _loadedServiceProviderServiceTemplates = value;
                this.RaisePropertyChanged("LoadedServiceProviderServiceTemplates");
            }
        }

        private ObservableCollection<Region> _loadedServiceProviderRegions;
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<Region> LoadedServiceProviderRegions
        {
            get { return _loadedServiceProviderRegions; }
            private set
            {
                _loadedServiceProviderRegions = value;
                this.RaisePropertyChanged("LoadedServiceProviderRegions");
            }
        }

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

        private readonly ObservableAsPropertyHelper<int> _forceFilterCountUpdate;

        /// <summary>
        /// Used to force the bindings on the Dispatching filter counts. 
        /// </summary>
        public int ForceFilterCountUpdate { get { return _forceFilterCountUpdate.Value; } }


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsVM"/> class.
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
                var serviceTemplatesQuery = Manager.Data.Context.GetServiceTemplatesForServiceProviderQuery(Manager.Context.RoleId, (int)ServiceTemplateLevel.ServiceProviderDefined);

                Manager.Data.Context.Load(serviceTemplatesQuery, lo =>
                {
                    LoadedServiceProviderServiceTemplates = new ObservableCollection<ServiceTemplate>(lo.Entities);
                }, null);

                #endregion
            });

            //Setup an observable for whenever the Dispatcher is entered/reentered
            var enteredDispatcher = MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher"));
            enteredDispatcher.ObserveOnDispatcher().Subscribe(_ =>
            {
                var regionsQuery = Manager.Data.Context.GetRegionsForServiceProviderQuery(Manager.Context.RoleId);

                Manager.Data.Context.Load(regionsQuery, lo =>
                {
                    LoadedServiceProviderRegions = new ObservableCollection<Region>(lo.Entities);
                }, null);
            });
        }

        #endregion
    }
}
