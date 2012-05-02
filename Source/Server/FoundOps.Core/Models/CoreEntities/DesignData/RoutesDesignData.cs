using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RoutesDesignData
    {
        public IEnumerable<Route> DesignRoutes { get; private set; }

        private readonly BusinessAccount _ownerBusinessAccount;
        private readonly ClientsDesignData _clientsDesignData;
        private readonly VehiclesDesignData _vehiclesDesignData;
        private readonly EmployeesDesignData _employeesDesignData;

        private readonly Random _random = new Random();
        private readonly string[] _routeNames = { "SF Bay Area", "Shelter Island", "North Side", "South Side" };

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
        /// <param name="employeesDesignData">The employees design data.</param>
        public RoutesDesignData(BusinessAccount ownerBusinessAccount, ClientsDesignData clientsDesignData, VehiclesDesignData vehiclesDesignData, 
            EmployeesDesignData employeesDesignData)
        {
            _ownerBusinessAccount = ownerBusinessAccount;
            _clientsDesignData = clientsDesignData;
            _vehiclesDesignData = vehiclesDesignData;
            _employeesDesignData = employeesDesignData;

            InitializeRoutes();

            foreach (var route in DesignRoutes)
                route.OwnerBusinessAccount = ownerBusinessAccount;
        }


        /// <summary>
        /// Creates a route from a service template and date. Uses a random route name.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="serviceTemplate"></param>
        /// <returns></returns>
        private Route CreateRoute(DateTime date, ServiceTemplate serviceTemplate)
        {
            var newRoute =new Route
            {
                Id = Guid.NewGuid(),
                Name = _routeNames.ElementAt(_random.Next(4)),
                Date = date,
                RouteType = serviceTemplate.Name
            };

            newRoute.Technicians.Add(_employeesDesignData.DesignEmployees.ElementAt(_random.Next(3)));
            newRoute.Vehicles.Add(_vehiclesDesignData.DesignVehicles.ElementAt(_random.Next(3)));


        }

        private void InitializeRoutes()
        {
            var serviceTemplates = _ownerBusinessAccount.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined);

            //Create three routes for yesterday
            for (int i = 0; i < 3; i++)
                CreateRoute(DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), serviceTemplates.ElementAt(_random.Next(3)));

            //Create three routes for today
            for (int i = 0; i < 3; i++)
                CreateRoute(DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), serviceTemplates.ElementAt(_random.Next(3)));

            #region Yesterday's Routes

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

        private void AddRouteDestinations(Route route, ServiceTemplate serviceTemplate, int minClient, int maxClient)
        {
            var orderInRoute = 1;

            for (var i = minClient; i <= maxClient; i++)
            {
                //Prevents you from trying to add a client that doesnt exist in Design Data
                if (i > _clientsDesignData.DesignClients.Count())
                    continue;

                var routeTask = CreateRouteTaskAndService(1, route.Date, _clientsDesignData.DesignClients.ElementAt(i).OwnedParty.Locations.ElementAt(0),
                    _clientsDesignData.DesignClients.ElementAt(i), route.RouteType, new TimeSpan(0, 15, 0), _ownerBusinessAccount, serviceTemplate, Status.Created);

                var routeDestination = new RouteDestination { OrderInRoute = orderInRoute };
                routeDestination.RouteTasks.Add(routeTask);
                route.RouteDestinations.Add(routeDestination);

                orderInRoute++;
            }
        }

        private RouteTask CreateRouteTaskAndService(int orderInRouteDestination, DateTime date, Location location, Client designClient, string routeType,
                                                    TimeSpan timeSpan, BusinessAccount ownerBusinessAccount, ServiceTemplate serviceTemplate, Status status)
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
                Service = newService,
                Status = status
            };

            return task;
        }
    }
}
