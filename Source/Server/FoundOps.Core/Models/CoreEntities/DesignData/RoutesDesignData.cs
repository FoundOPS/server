using System;
using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RoutesDesignData
    {
        public Route DesignRoute { get; private set; }
        public Route DesignRouteTwo { get; private set; }
        public Route DesignRouteThree { get; private set; }
        public Route DesignRouteFour { get; private set; }
        public Route DesignRouteFive { get; private set; }

        public RouteTask DesignRouteTask { get; private set; }
        public RouteTask DesignRouteTaskTwo { get; private set; }
        public RouteTask DesignRouteTaskThree { get; private set; }

        public IEnumerable<Route> DesignRoutes { get; private set; }
        private readonly BusinessAccount _ownerBusinessAccount;
        private readonly ClientsDesignData _clientsDesignData;
        private readonly VehiclesDesignData _vehiclesDesignData;
        private readonly EmployeesDesignData _employeesDesignData;

        public RoutesDesignData()
            : this(new BusinessAccountsDesignData().GotGrease)
        {
        }

        public RoutesDesignData(BusinessAccount ownerBusinessAccount)
            : this(ownerBusinessAccount, new ClientsDesignData(), new VehiclesDesignData(), new EmployeesDesignData())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesDesignData"/> class.
        /// </summary>
        /// <param name="ownerBusinessAccount">The owner business account.</param>
        /// <param name="clientsDesignData">The clients design data.</param>
        /// <param name="vehiclesDesignData">The vehicles design data.</param>
        public RoutesDesignData(BusinessAccount ownerBusinessAccount, ClientsDesignData clientsDesignData, VehiclesDesignData vehiclesDesignData, EmployeesDesignData employeesDesignData)
        {
            _ownerBusinessAccount = ownerBusinessAccount;
            _clientsDesignData = clientsDesignData;
            _vehiclesDesignData = vehiclesDesignData;
            _employeesDesignData = employeesDesignData;

            InitializeRoute();

            foreach (var route in DesignRoutes)
                route.OwnerBusinessAccount = ownerBusinessAccount;
        }

        private void InitializeRoute()
        {
            DesignRoute = new Route
                              {
                                  Name = "SF Bay Area",
                                  Date = DateTime.UtcNow.Date,
                                  RouteType = _ownerBusinessAccount.ServiceTemplates
                                  .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                                  .ElementAt(0).Name
                              };
            AddRouteDestinations(DesignRoute);
            DesignRoute.Technicians.Add(_employeesDesignData.DesignEmployee);
            DesignRoute.Vehicles.Add(_vehiclesDesignData.DesignVehicle);

            DesignRouteTwo = new Route
                                 {
                                     Name = "Shelter Island",
                                     Date = DateTime.UtcNow.Date,
                                     RouteType = _ownerBusinessAccount.ServiceTemplates
                                  .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                                  .ElementAt(1).Name
                                 };

            AddRouteDestinations(DesignRouteTwo);
            DesignRouteTwo.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            DesignRouteTwo.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            DesignRouteThree = new Route
                                   {
                                       Name = "Purdue",
                                       Date = DateTime.UtcNow.Date.AddDays(1),
                                       RouteType = _ownerBusinessAccount.ServiceTemplates
                                 .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                                 .ElementAt(2).Name
                                   };
            AddRouteDestinations(DesignRouteThree);
            DesignRouteThree.Technicians.Add(_employeesDesignData.DesignEmployeeThree);
            DesignRouteThree.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);

            DesignRouteFour = new Route
            {
                Name = "North Side",
                Date = DateTime.UtcNow.Date,
                RouteType = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(0).Name
            };
            AddRouteDestinations(DesignRouteFour);
            DesignRouteFour.Technicians.Add(_employeesDesignData.DesignEmployee);
            DesignRouteFour.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            DesignRouteFive = new Route
            {
                Name = "South Side",
                Date = DateTime.UtcNow.Date,
                RouteType = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(1).Name
            };
            AddRouteDestinations(DesignRouteFive);
            DesignRouteFive.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            DesignRouteFive.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);


            DesignRoutes = new List<Route> { DesignRoute, DesignRouteTwo, DesignRouteThree, DesignRouteFour, DesignRouteFive };
        }

        private void AddRouteDestinations(Route route)
        {
            #region Route Destination One

            DesignRouteTask = new RouteTask
            {
                OrderInRouteDestination = 1,
                Date = route.Date,
                Location = _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClient,
                Name = "Collect Oil",
                EstimatedDuration = new TimeSpan(0, 15, 0),
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            DesignRouteTaskTwo = new RouteTask
            {
                OrderInRouteDestination = 2,
                Date = route.Date,
                Location = _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientTwo,
                Name = "Get Oil Paperwork Signed",
                EstimatedDuration = new TimeSpan(0, 1, 0),
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            routeDestination.RouteTasks.Add(DesignRouteTaskTwo);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = new RouteTask
            {
                OrderInRouteDestination = 1,
                Date = route.Date,
                Location = _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientTwo,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination { OrderInRoute = 2 };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = new RouteTask
            {
                OrderInRouteDestination = 1,
                Date = route.Date,
                Location = _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(0),
                Client = _clientsDesignData.DesignClientThree,
                EstimatedDuration = new TimeSpan(0, 10, 0),
                Name = "Drop off Paperwork",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination { OrderInRoute = 3 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = new RouteTask
            {
                OrderInRouteDestination = 1,
                Date = route.Date,
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
                OrderInRouteDestination = 1,
                Date = route.Date,
                Location = _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(1),
                Client = _clientsDesignData.DesignClientTwo,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = new RouteTask
            {
                OrderInRouteDestination = 1,
                Date = route.Date,
                Location = _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(1),
                Client = _clientsDesignData.DesignClientThree,
                EstimatedDuration = new TimeSpan(0, 25, 0),
                Name = "Clean Grease Trap",
                OwnerBusinessAccount = _ownerBusinessAccount
            };

            routeDestination = new RouteDestination { OrderInRoute = 6 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }
    }
}
