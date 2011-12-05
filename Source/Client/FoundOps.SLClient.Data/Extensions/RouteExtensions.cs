using FoundOps.Common.Silverlight.Tools;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Route
    {
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