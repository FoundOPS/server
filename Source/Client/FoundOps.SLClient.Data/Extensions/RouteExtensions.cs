﻿using System.ServiceModel.DomainServices.Client;
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