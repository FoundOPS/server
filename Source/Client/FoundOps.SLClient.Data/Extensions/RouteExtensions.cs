using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Common.Silverlight.UI.Interfaces;
using RiaServicesContrib;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Route : ILoadDetails
    {
        #region Implementation of ILoadDetails

        private bool _detailsLoaded;
        /// <summary>
        /// Gets or sets a value indicating whether [details loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoaded
        {
            get { return _detailsLoaded; }
            set
            {
                _detailsLoaded = value;
                this.RaisePropertyChanged("DetailsLoaded");
            }
        }

        private bool _manifestDetailsLoaded;
        /// <summary>
        /// This means the Clients, Locations, and ServiceTemplates and Fields of the RouteDestinations and Tasks have been loaded.
        /// </summary>
        public bool ManifestDetailsLoaded
        {
            get { return _manifestDetailsLoaded; }
            set
            {
                _manifestDetailsLoaded = value;
                this.RaisePropertyChanged("ManifestDetailsLoaded");
            }
        }

        #endregion

        /// <summary>
        /// Gets the entity graph of Client to detach on reload.
        /// </summary>
        public EntityGraph<Entity> EntityGraphToDetach
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Route, RouteDestination>(r => r.RouteDestinations).Edge<RouteDestination, RouteTask>(rd => rd.RouteTasks);

                return new EntityGraph<Entity>(this, graphShape);
            }
        }

        private OrderedEntityCollection<RouteDestination> _routeDestinationsListWrapper;
        public OrderedEntityCollection<RouteDestination> RouteDestinationsListWrapper
        {
            get { return _routeDestinationsListWrapper; }
            private set
            {
                _routeDestinationsListWrapper = value;
                this.RaisePropertyChanged("RouteDestinationsListWrapper");
            }
        }

        /// <summary>
        /// Returns the route's locations.
        /// </summary>
        public IEnumerable<Location> RouteLocations
        {
            get
            {
                return this.RouteDestinations.SelectMany(rd => rd.RouteTasks.Select(rt => rt.Location)).Where(l => l != null);
            }
        }

        partial void OnCreation()
        {
            RouteDestinationsListWrapper = new OrderedEntityCollection<RouteDestination>(this.RouteDestinations, "OrderInRoute", false);
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
            {
                RouteDestinationsListWrapper = new OrderedEntityCollection<RouteDestination>(this.RouteDestinations, "OrderInRoute", false);
            }
        }
    }
}