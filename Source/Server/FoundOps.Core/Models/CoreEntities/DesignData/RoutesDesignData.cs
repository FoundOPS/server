using FoundOps.Common.Composite.Tools;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models.CoreEntities.Extensions.Services;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FoundOps.Core.Models.CoreEntities.DesignData
{
    public class RoutesDesignData
    {
        public IList<Route> DesignRoutes { get; private set; }

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

            InitializeRoutes(ownerBusinessAccount);
        }

        /// <summary>
        /// Create routes from yesterday until tomorrow for the current service provider.
        /// </summary>
        private void InitializeRoutes(BusinessAccount ownerBusinessAccount)
        {
            DesignRoutes = new List<Route>();

            var serviceTemplates = _ownerBusinessAccount.ServiceTemplates.Where(st => st.ServiceTemplateLevel == ServiceTemplateLevel.ServiceProviderDefined).ToArray();

            //Create three routes for yesterday
            for (int i = 0; i <= 3; i++)
                CreateRoute(DateTime.UtcNow.AddDays(-1), serviceTemplates.RandomItem(), ownerBusinessAccount, i);

            //Create three routes for today
            for (int i = 0; i <= 3; i++)
                CreateRoute(DateTime.UtcNow, serviceTemplates.RandomItem(), ownerBusinessAccount, i);

            //Create three routes for tomorrow
            for (int i = 0; i <= 3; i++)
                CreateRoute(DateTime.UtcNow.AddDays(1), serviceTemplates.RandomItem(), ownerBusinessAccount, i);
        }


        /// <summary>
        /// Creates a route from a service template and date. Uses a random route name.
        /// </summary>
        private void CreateRoute(DateTime date, ServiceTemplate serviceTemplate, BusinessAccount ownerBusinessAccount, int clientIndex)
        {
            var newRoute = new Route
            {
                Id = Guid.NewGuid(),
                Name = _routeNames.RandomItem(),
                Date = date.Date,
                RouteType = serviceTemplate.Name,
                OwnerBusinessAccount = ownerBusinessAccount
            };

            //Add employees to the Route
            newRoute.Employees.Add(_employeesDesignData.DesignEmployees.RandomItem());

            //Add vehicles to the Route
            newRoute.Vehicles.Add(_vehiclesDesignData.DesignVehicles.RandomItem());

            var orderInRoute = 1;

            var startingClientIndex = clientIndex * 6;

            //Create Services and RouteTasks, add them to the Route
            for (var i = startingClientIndex; i <= (startingClientIndex + 5); i++)
            {
                //Prevents you from trying to add a client that doesnt exist in Design Data
                if (i > _clientsDesignData.DesignClients.Count())
                    break;

                var currentClient = _clientsDesignData.DesignClients.ElementAt(i);

                //Take the first location from the client
                var currentLocation = currentClient.Locations.ElementAt(0);

                var newService = new Service
                {
                    ServiceDate = newRoute.Date,
                    ServiceTemplate = serviceTemplate.MakeChild(ServiceTemplateLevel.ServiceDefined),
                    Client = currentClient,
                    ServiceProvider = ownerBusinessAccount
                };

                newService.ServiceTemplate.SetDestination(currentLocation);

                var routeTask = new RouteTask
                {
                    OrderInRouteDestination = 1,
                    Date = newRoute.Date,
                    Location = currentClient.Locations.ElementAt(0),
                    Client = currentClient,
                    Name = newRoute.RouteType,
                    EstimatedDuration = new TimeSpan(0, _random.Next(25), 0),
                    OwnerBusinessAccount = ownerBusinessAccount,
                    Service = newService,
                    TaskStatus = ownerBusinessAccount.TaskStatus.FirstOrDefault(ts => ts.StatusDetailInt.ToString().Contains('4'))
                };

                var routeDestination = new RouteDestination { OrderInRoute = orderInRoute };
                routeDestination.RouteTasks.Add(routeTask);
                newRoute.RouteDestinations.Add(routeDestination);

                orderInRoute++;
            }

            DesignRoutes.Add(newRoute);
        }
    }
}
