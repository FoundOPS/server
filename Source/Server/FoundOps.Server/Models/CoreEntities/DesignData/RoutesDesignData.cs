using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Server.Models.CoreEntities.DesignData
{
    public class RoutesDesignData
    {
        public Route DesignRoute { get; private set; }
        public Route DesignRouteTwo { get; private set; }
        public Route DesignRouteThree { get; private set; }

        public RouteTask DesignRouteTask { get; private set; }
        public RouteTask DesignRouteTaskTwo { get; private set; }
        public RouteTask DesignRouteTaskThree { get; private set; }

        public IEnumerable<Route> DesignRoutes { get; private set; }
        private readonly BusinessAccount _ownerBusinessAccount;
        private ClientsDesignData _clientsDesignData;

        public RoutesDesignData()
            : this(new BusinessAccountsDesignData().GotGrease)
        {
        }

        public RoutesDesignData(BusinessAccount ownerBusinessAccount)
            : this(ownerBusinessAccount, new ClientsDesignData())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesDesignData"/> class.
        /// </summary>
        /// <param name="ownerBusinessAccount">The owner business account.</param>
        /// <param name="clientsDesignData">The clients design data.</param>
        public RoutesDesignData(BusinessAccount ownerBusinessAccount, ClientsDesignData clientsDesignData)
        {
            _ownerBusinessAccount = ownerBusinessAccount;
            _clientsDesignData = clientsDesignData;

            InitializeRoute();

            foreach (var route in DesignRoutes)
                route.OwnerBusinessAccount = ownerBusinessAccount;
        }

        private void InitializeRoute()
        {
            DesignRoute = new Route
                              {
                                  Name = "SF Bay Area",
                                  Date = DateTime.Now.Date,
                                  Color = "FF0071B8",
                                  RouteType = "Oil"
                              };
            AddRouteDestinations(DesignRoute);
            
            DesignRouteTwo = new Route
                                 {
                                     Name = "Shelter Island",
                                     Date = DateTime.Now.Date,
                                     Color = "FFFF5200",
                                     RouteType = "Environmental Biotech"
                                 };

            AddRouteDestinations(DesignRouteTwo);

            DesignRouteThree = new Route
                                   {
                                       Name = "Purdue",
                                       Date = DateTime.Now.Date.AddDays(1),
                                       Color = "FF009B2D",
                                       RouteType = "Oil"
                                   };
            AddRouteDestinations(DesignRouteThree);

            DesignRoutes = new List<Route> { DesignRoute, DesignRouteTwo, DesignRouteThree };
        }

        private void AddRouteDestinations(Route route)
        {
            #region Route Destination One

            DesignRouteTask = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClient,
                Name = "Collect Oil",
                EstimatedDuration = new TimeSpan(0, 15, 0),
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            DesignRouteTaskTwo = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientTwo,
                Name = "Get Oil Paperwork Signed",
                EstimatedDuration = new TimeSpan(0, 1, 0),
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            var routeDestination = new RouteDestination
              {
                  OrderInRoute = 1
              };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            routeDestination.RouteTasks.Add(DesignRouteTaskTwo);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientTwo,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination
            {
                OrderInRoute = 2
            };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientThree,
                EstimatedDuration = new TimeSpan(0, 10, 0),
                Name = "Drop off Paperwork",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination
            {
                OrderInRoute = 3
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(1),
                Client = _clientsDesignData.DesignClient,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(1),
                Client = _clientsDesignData.DesignClientTwo,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination
            {
                OrderInRoute = 5
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = new RouteTask
            {
                Date = DateTime.Now.Date,
                Location = _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(1),
                Client = _clientsDesignData.DesignClientThree,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination
            {
                OrderInRoute = 6
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }
    }
}
