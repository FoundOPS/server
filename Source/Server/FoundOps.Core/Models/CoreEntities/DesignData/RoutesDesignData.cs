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

            DesignRoute = new Route
                              {
                                  Name = "SF Bay Area",
                                  Date = DateTime.UtcNow.Date,
                                  RouteType = serviceTemplate.Name
                              };
            AddRouteDestinations(DesignRoute, serviceTemplate);
            DesignRoute.Technicians.Add(_employeesDesignData.DesignEmployee);
            DesignRoute.Vehicles.Add(_vehiclesDesignData.DesignVehicle);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(1);
            DesignRouteTwo = new Route
                                 {
                                     Name = "Shelter Island",
                                     Date = DateTime.UtcNow.Date,
                                     RouteType = serviceTemplate.Name
                                 };

            AddRouteDestinations(DesignRouteTwo, serviceTemplate);
            DesignRouteTwo.Technicians.Add(_employeesDesignData.DesignEmployeeTwo);
            DesignRouteTwo.Vehicles.Add(_vehiclesDesignData.DesignVehicleTwo);

            serviceTemplate = _ownerBusinessAccount.ServiceTemplates
                .Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined)
                .ElementAt(2);

            DesignRouteThree = new Route
                                   {
                                       Name = "Purdue",
                                       Date = DateTime.UtcNow.Date.AddDays(1),
                                       RouteType = serviceTemplate.Name
                                   };

            AddRouteDestinations(DesignRouteThree, serviceTemplate);
            DesignRouteThree.Technicians.Add(_employeesDesignData.DesignEmployeeThree);
            DesignRouteThree.Vehicles.Add(_vehiclesDesignData.DesignVehicleThree);
            DesignRoutes = new List<Route> { DesignRoute, DesignRouteTwo, DesignRouteThree };
        }

        private void AddRouteDestinations(Route route, ServiceTemplate serviceTemplate)
        {
            #region Route Destination One


            DesignRouteTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClient, route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate);

            DesignRouteTaskTwo = CreateRouteTaskAndService(2, route.Date, _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(0),
                _clientsDesignData.DesignClientTwo, route.RouteType, new TimeSpan(0, 1, 0), _ownerBusinessAccount, serviceTemplate);

            var routeDestination = new RouteDestination { OrderInRoute = 1 };
            routeDestination.RouteTasks.Add(DesignRouteTask);
            routeDestination.RouteTasks.Add(DesignRouteTaskTwo);
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

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClient.OwnedParty.Locations.ElementAt(1),
                _clientsDesignData.DesignClient, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination
            {
                OrderInRoute = 4
            };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Five

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientTwo.OwnedParty.Locations.ElementAt(1),
                _clientsDesignData.DesignClientTwo, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

            routeDestination = new RouteDestination { OrderInRoute = 5 };
            routeDestination.RouteTasks.Add(routeTask);
            route.RouteDestinations.Add(routeDestination);

            #endregion

            #region Route Destination Six

            routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClientThree.OwnedParty.Locations.ElementAt(1),
                _clientsDesignData.DesignClientThree, route.RouteType, new TimeSpan(0, 25, 0), _ownerBusinessAccount, serviceTemplate);

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
