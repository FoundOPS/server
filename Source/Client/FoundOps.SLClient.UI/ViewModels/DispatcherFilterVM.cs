using System.Linq;
using System.Reactive.Subjects;
using System.ServiceModel.DomainServices.Client;
using System.Threading.Tasks;
using FoundOps.Common.Silverlight.MVVM.Messages;
using FoundOps.Common.Silverlight.MVVM.Models;
using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using FoundOps.Common.Silverlight.UI.Messages;
using FoundOps.Common.Tools;
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

        private readonly ObservableCollection<EntityOption> _serviceTemplateOptions = new ObservableCollection<EntityOption>();
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<EntityOption> ServiceTemplateOptions { get { return _serviceTemplateOptions; } }

        private readonly ObservableCollection<EntityOption> _regionOptions = new ObservableCollection<EntityOption>();
        /// <summary>
        /// Collection of loaded Regions for the ServiceProvider.
        /// </summary>
        public ObservableCollection<EntityOption> RegionOptions { get { return _regionOptions; } }

        private readonly Subject<bool> _filterUpdatedSubject = new Subject<bool>();
        /// <summary>
        /// An observable that pushes whenever the filter is updated.
        /// </summary>
        public IObservable<bool> FilterUpdatedObservable
        {
            get
            {
                //Throttle by 250 milliseconds to prevent duplicates
                return _filterUpdatedSubject.Throttle(TimeSpan.FromMilliseconds(250)).AsObservable();
            }
        }

        #endregion

        #region Locals

        // Used to cancel the previous RoutesLoad.
        private readonly Subject<bool> _cancelLastFilterRegionsLoad = new Subject<bool>();

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
            //Setup the ServiceTemplateOptions whenever the CurrentServiceTemplatesObservable updates
            Manager.Context.CurrentServiceTemplatesObservable.ObserveOnDispatcher().Subscribe(loadedServiceTemplates =>
            {
                ServiceTemplateOptions.Clear();
                foreach (var serviceTemplate in loadedServiceTemplates)
                    ServiceTemplateOptions.Add(new EntityOption(serviceTemplate));
            });

            //Load the regions whenever the Dispatcher is entered
            MessageBus.Current.Listen<NavigateToMessage>().Where(m => m.UriToNavigateTo.ToString().Contains("Dispatcher")).AsGeneric()
            .ObserveOnDispatcher().Subscribe(_ =>
           {
               _cancelLastFilterRegionsLoad.OnNext(true);

               Manager.CoreDomainContext.LoadAsync(Manager.CoreDomainContext.GetRegionsForServiceProviderQuery(Manager.Context.RoleId), _cancelLastFilterRegionsLoad)
               .ContinueWith(task =>
               {
                   if (task.IsCanceled || !task.Result.Any())
                       return;

                   //Notify the RoutesVM has completed loading Routes
                   IsLoadingSubject.OnNext(false);

                   LoadedRegions = new ObservableCollection<Region>(task.Result);

                   RegionOptions.Clear();

                   foreach (var region in LoadedRegions)
                       RegionOptions.Add(new EntityOption(region));

               }, TaskScheduler.FromCurrentSynchronizationContext());
           });
        }

        #endregion

        #region Logic

        #region Public

        /// <summary>
        /// Returns whether the taskHolder is included in the current filter.
        /// </summary>
        /// <param name="taskHolder">The taskHolder to check.</param>
        /// <returns></returns>
        public bool TaskHolderIncludedInFilter(TaskHolder taskHolder)
        {
            var meetsRouteTypeFilter = ServiceTemplateOptions.Any(option => option.IsSelected && ((ServiceTemplate)option.Entity).Name == taskHolder.ServiceName);
            var meetsRegionsFilter = RegionOptions.Any(option => (option.IsSelected && ((Region)option.Entity).Name == taskHolder.RegionName)) || 
                taskHolder.RegionName == null || !RegionOptions.Any();

            return meetsRouteTypeFilter && meetsRegionsFilter;
        }

        /// <summary>
        /// Returns whether the route is included in the current filter.
        /// </summary>
        /// <param name="route">The route to check.</param>
        /// <returns></returns>
        public bool RouteIncludedInFilter(Route route)
        {
            //Selects all the Region's names from the RouteTasks in the Route
            var regionsForRoute = route.RouteDestinations.SelectMany(rd => rd.RouteTasks.Where(rt => rt.Location != null && rt.Location.Region != null)
                .Select(rt => rt.Location.Region.Name)).ToArray();

            var meetsRouteTypeFilter = ServiceTemplateOptions.Any(option => option.IsSelected && ((ServiceTemplate)option.Entity).Name == route.RouteType);

            //Only filter by region if there are locations or locations with regions in the route
            var meetsRegionsFilter = !regionsForRoute.Any()
                || RegionOptions.Any(option => option.IsSelected && regionsForRoute.Contains(((Region)option.Entity).Name)) || !RegionOptions.Any();

            return meetsRouteTypeFilter && meetsRegionsFilter;
        }

        /// <summary>
        /// Used to manually trigger the filter updated.
        /// </summary>
        public void TriggerFilterUpdated()
        {
            _filterUpdatedSubject.OnNext(true);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// An option to select or not select a ServiceTemplate.
    /// </summary>
    public class EntityOption : ThreadableNotifiableObject
    {
        #region Public Properties

        private readonly Entity _entity;
        /// <summary>
        /// Gets the selected regions of routes to display.
        /// </summary>
        public Entity Entity { get { return _entity; } }

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
        /// Initializes a new instance of the <see cref="EntityOption"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityOption(Entity entity)
        {
            _entity = entity;
        }

        #endregion
    }
}