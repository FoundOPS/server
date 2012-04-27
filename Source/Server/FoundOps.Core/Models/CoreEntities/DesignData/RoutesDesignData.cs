using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RoutesDesignData
    {
        public Route DesignRoute { get; private set; }
        public Route DesignRouteTwo { get; private set; }
        public Route DesignRouteThree { get; private set; }
        public Route DesignRouteFour { get; private set; }
        public Route DesignRouteFive { get; private set; }
        public Route YesterdayDesignRouteOne { get; private set; }
        public Route YesterdayDesignRouteTwo { get; private set; }
        public Route YesterdayDesignRouteThree { get; private set; }
        public Route YesterdayDesignRouteFour { get; private set; }

        

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
            var serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(0);
            #region Yesterday's Routes

            YesterdayDesignRouteOne = new Route
            {
                Id = Guid.NewGuid(),
                Name = "SF Bay Area",
                Date = DateTime.UtcNow.AddDays(-1).Date,
                RouteType = serviceTemplate.Name
            };
            AddRouteDestinationsRouteOne(YesterdayDesignRouteOne, serviceTemplate);
            YesterdayDesignRouteOne.Technicians.Add(_employeesDesignData.DesignEmployee);
            YesterdayDesignRouteOne.Vehicles.Add(_vehiclesDesignData.DesignVehicle);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(1);

            YesterdayDesignRouteTwo = new Route
            {
                Id = Guid.NewGuid(),
                Name = "Shelter Island",
                Date = DateTime.UtcNow.AddDays(-1).Date,
                RouteType = serviceTemplate.Name
            };

            AddRouteDestinationsRouteTwo(YesterdayDesignRouteTwo, serviceTemplate);
            YesterdayDesignRouteTwo.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            YesterdayDesignRouteTwo.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(2);

            YesterdayDesignRouteThree = new Route
            {
                Id = Guid.NewGuid(),
                Name = "North Side",
                Date = DateTime.UtcNow.AddDays(-1).Date,
                RouteType = serviceTemplate.Name
            };
            AddRouteDestinationsRouteThree(YesterdayDesignRouteThree, serviceTemplate);
            YesterdayDesignRouteThree.Technicians.Add(_employeesDesignData.DesignEmployee);
            YesterdayDesignRouteThree.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(1);

            YesterdayDesignRouteFour = new Route
            {
                Id = Guid.NewGuid(),
                Name = "South Side",
                Date = DateTime.UtcNow.AddDays(-1).Date,
                RouteType = serviceTemplate.Name
            };
            AddRouteDestinationsRouteFour(YesterdayDesignRouteFour, serviceTemplate);
            YesterdayDesignRouteFour.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            YesterdayDesignRouteFour.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);

            #endregion

            #region Today's Routes

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(0);

            DesignRoute = new Route
                              {
                                  Id = Guid.NewGuid(),
                                  Name = "SF Bay Area",
                                  Date = DateTime.UtcNow.Date,
                                  RouteType = serviceTemplate.Name
                              };
            AddRouteDestinationsRouteOne(DesignRoute, serviceTemplate);
            DesignRoute.Technicians.Add(_employeesDesignData.DesignEmployee);
            DesignRoute.Vehicles.Add(_vehiclesDesignData.DesignVehicle);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(1);

            DesignRouteTwo = new Route
                                 {
                                     Id = Guid.NewGuid(),
                                     Name = "Shelter Island",
                                     Date = DateTime.UtcNow.Date,
                                     RouteType = serviceTemplate.Name
                                 };

            AddRouteDestinationsRouteTwo(DesignRouteTwo, serviceTemplate);
            DesignRouteTwo.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            DesignRouteTwo.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(2);

            DesignRouteFour = new Route
            {
                Id = Guid.NewGuid(),
                Name = "North Side",
                Date = DateTime.UtcNow.Date,
                RouteType = serviceTemplate.Name
            };
            AddRouteDestinationsRouteThree(DesignRouteFour, serviceTemplate);
            DesignRouteFour.Technicians.Add(_employeesDesignData.DesignEmployee);
            DesignRouteFour.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(1);

            DesignRouteFive = new Route
            {
                Id = Guid.NewGuid(),
                Name = "South Side", 
                Date = DateTime.UtcNow.Date,
                RouteType = serviceTemplate.Name
            };
            AddRouteDestinationsRouteFour(DesignRouteFive, serviceTemplate);
            DesignRouteFive.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            DesignRouteFive.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);

            #endregion

            #region Tomorrow's Routes 

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                            .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                            .ElementAt(2);

            DesignRouteThree = new Route
                                   {
                                       Id = Guid.NewGuid(),
                                       Name = "Purdue",
                                       Date = DateTime.UtcNow.Date.AddDays(1),
                                       RouteType = serviceTemplate.Name
                                   };

            AddRouteDestinationsRouteOne(DesignRouteThree, serviceTemplate);
            DesignRouteThree.Technicians.Add(_employeesDesignData.DesignEmployeeThree);
            DesignRouteThree.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);

            #endregion

            DesignRoutes = new List<Route> { DesignRoute, DesignRouteTwo, DesignRouteThree, DesignRouteFour, DesignRouteFive, 
                                             YesterdayDesignRouteOne, YesterdayDesignRouteTwo, YesterdayDesignRouteThree, YesterdayDesignRouteFour}; 
        }

        private void AddRouteDestinationsRouteOne(Route route, ServiceTemplate serviceTemplate)
        {
            #region Route Destination One


            DesignRouteTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClient, route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate);

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwo, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 2 };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientThree, route.RouteType, new TimeSpan(0, 10, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 3 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientFour.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientFour, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientFive.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientFive, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientSix.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientSix, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 6 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }

        private void AddRouteDestinationsRouteTwo (Route route, ServiceTemplate serviceTemplate)
        {
            #region Route Destination One


            DesignRouteTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientSeven.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientSeven, route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate);

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientEight.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientEight, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 2 };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientNine.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientNine, route.RouteType, new TimeSpan(0, 10, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 3 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTen, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientEleven.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientEleven, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwelve.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwelve, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 6 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }

        private void AddRouteDestinationsRouteThree (Route route, ServiceTemplate serviceTemplate)
        {
            #region Route Destination One


            DesignRouteTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientThirteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientThirteen, route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate);

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientFourteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientFourteen, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 2 };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientFifteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientFifteen, route.RouteType, new TimeSpan(0, 10, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 3 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientSixteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientSixteen, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientSeventeen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientSeventeen, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientEighteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientEighteen, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 6 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }

        private void AddRouteDestinationsRouteFour (Route route, ServiceTemplate serviceTemplate)
        {
            #region Route Destination One


            DesignRouteTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientNineteen.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientNineteen, route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate);

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Two

            DesignRouteTaskThree = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwenty.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwenty, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 2 };

            routeDestination.RouteTasks.Add(DesignRouteTaskThree);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Three

            var routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwentyOne.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwentyOne, route.RouteType, new TimeSpan(0, 10, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 3 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Four

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwentyTwo.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwentyTwo, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwentyThree.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwentyThree, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwentyFour.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwentyFour, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 6 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion
        }

        private RouteTask CreateRouteTaskAndService(int orderInRouteDestination, DateTime date, Location location, Client designClient, string routeType, TimeSpan timeSpan, BusinessAccount ownerBusinessAccount, ServiceTemplate serviceTemplate)
        {
            var newService = new Service
            {
                ServiceDate = date,
                ServiceTemplate = serviceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                Client = designClient,
                ServiceProvider = ownerBusinessAccount
            };

            newService.ServiceTemplate.SetDestination(location);

            var task = new RouteTask
            {
                OrderInRouteDestination = orderInRouteDestination,
                Date = date,
                Location = location,
                Client = designClient,
                Name = routeType,
                EstimatedDuration = timeSpan,
                OwnerBusinessAccount = ownerBusinessAccount,
                Service = newService
            };

            return task;
        }
    }
}
