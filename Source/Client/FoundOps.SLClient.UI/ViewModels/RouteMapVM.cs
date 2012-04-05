using System;
using FoundOps.SLClient.UI.Controls.Dispatcher;
using ReactiveUI;
using MEFedMVVM.ViewModelLocator;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Context.Services.Interface;

namespace FoundOps.SLClient.UI.ViewModels
{
    [ExportViewModel("RouteMapVM")]
    public class RouteMapVM : CoreEntityCollectionVM<RouteDestination>
    {
        //Public Properties

        public RelayCommand<Tuple<decimal, decimal>> ManuallySetLatitudeLongitude { get; private set; }

        private ObservableCollection<Route> _shownRoutes;
        /// <summary>
        /// Gets the shown routes.
        /// </summary>
        public ObservableCollection<Route> ShownRoutes
        {
            get { return _shownRoutes; }
            private set
            {
                _shownRoutes = value;
                this.RaisePropertyChanged("ShownRoutes");
            }
        }

        // Local Variables

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteMapVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RouteMapVM() : base(false)
        {
            //Register Commands
            ManuallySetLatitudeLongitude = new RelayCommand<Tuple<decimal, decimal>>(OnManuallySetLatitudeLongitude, (latitudeLongitude) => true);

            //TODO: I do not think this is correct

            //_routeDataService.PropertyChanged += (sender, e) =>
            //{
            //    SaveCommand.RaiseCanExecuteChanged();
            //    DiscardCommand.RaiseCanExecuteChanged();
            //};

            //TODO: I do not think this is correct

            //_routeDataService.GetRoutesForServiceProviderOnDay(ContextManager.RoleId, DateTime.Now, getRoutesCallback =>
            //{
            //    ShownRoutes = getRoutesCallback;
            //    MessageBus.Current.SendMessage(new RefreshRouteMapView());
            //});
        }

        #region RouteMapVM's Logic

        private void OnManuallySetLatitudeLongitude(Tuple<decimal, decimal> latitudeLongitude)
        {
            if (SelectedEntity == null) return;


            SelectedEntity.Location.Latitude = Convert.ToDecimal(latitudeLongitude.Item1);
            SelectedEntity.Location.Longitude = Convert.ToDecimal(latitudeLongitude.Item2);

            if (SelectedEntity.Location.TelerikLocation != null)
                MessageBus.Current.SendMessage(new RouteDestinationSetMessage(SelectedEntity.Location.TelerikLocation.Value));
            
            this.RaisePropertyChanged("ShownRoutes");
        }

        protected override void OnSelectedEntityChanged(RouteDestination oldValue, RouteDestination newValue)
        {
            ShownRoutes = new ObservableCollection<Route>();

            if (newValue == null) return;

            if (newValue.Location.Latitude != null && newValue.Location.Longitude != null)
            {
                if (newValue.Location.TelerikLocation != null)
                    MessageBus.Current.SendMessage(new RouteDestinationSetMessage(newValue.Location.TelerikLocation.Value));
            }
        }

        public class RouteDestinationSetMessage
        {
            /// <summary>
            /// Gets the set route destination.
            /// </summary>
            public Telerik.Windows.Controls.Map.Location SetRouteDestination { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="RouteDestinationSetMessage"/> class.
            /// </summary>
            /// <param name="routeDestination">The route destination.</param>
            public RouteDestinationSetMessage(Telerik.Windows.Controls.Map.Location routeDestination)
            {
                SetRouteDestination = routeDestination;
            }
        }

        #endregion
    }

    public class RefreshRouteMapView
    {
    }
}