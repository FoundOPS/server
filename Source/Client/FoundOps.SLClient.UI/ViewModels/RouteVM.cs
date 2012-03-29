using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for editing a single Route
    /// </summary>
    public class RouteVM : CoreEntityVM, IDisposable, IAddToDeleteFromDestination<Employee>, IAddNewExisting<Employee>, IRemoveDelete<Employee>,
        IAddToDeleteFromDestination<Vehicle>, IAddNewExisting<Vehicle>, IRemoveDelete<Vehicle>
    {
        #region Public Properties

        /// <summary>
        /// The current route.
        /// </summary>
        public Route Route { get; private set; }

        #region Selected Items

        private Employee _selectedEmployee;
        /// <summary>
        /// The selected employee.
        /// </summary>
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                this.RaisePropertyChanged("SelectedEmployee");
            }
        }

        private Vehicle _selectedVehicle;
        /// <summary>
        /// The selected vehicle.
        /// </summary>
        public Vehicle SelectedVehicle
        {
            get { return _selectedVehicle; }
            set
            {
                _selectedVehicle = value;
                this.RaisePropertyChanged("SelectedVehicle");
            }
        }

        #endregion

        #region Implementation of IAddToDeleteFromDestination

        /// <summary>
        /// Links to the LinkToAddToDeleteFromControl events.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFromEvents(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Employee))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemEmployee((Employee)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemEmployee(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemEmployee();
                control.DeleteItem += (s, e) => this.DeleteItemEmployee();
            }

            if (sourceType == typeof(Vehicle))
            {
                control.AddExistingItem += (s, existingItem) => this.AddExistingItemVehicle((Vehicle)existingItem);
                control.AddNewItem += (s, newItemText) => this.AddNewItemVehicle(newItemText);
                control.RemoveItem += (s, e) => this.RemoveItemVehicle();
                control.DeleteItem += (s, e) => this.DeleteItemVehicle();
            }
        }

        /// <summary>
        /// Gets the user account destination items source.
        /// ToArray creates a new IEnumerable, part of a workaround for the RadGridView displaying incorrect items when an item is removed.
        /// </summary>
        public IEnumerable EmployeesDestinationItemsSource { get { return Route == null ? null : Route.Technicians.ToArray(); } }

        /// <summary>
        /// Gets the service templates destination items source.
        /// ToArray creates a new IEnumerable, part of a workaround for the RadGridView displaying incorrect items when an item is removed.
        /// </summary>
        public IEnumerable VehiclesDestinationItemsSource { get { return Route == null ? null : Route.Vehicles.ToArray(); } }

        #endregion

        #region Implementation of IAddNewExisting<Employee> & IRemoveDelete<Employee>

        /// <summary>
        /// An action to add a new Employee to the current Employee.
        /// </summary>
        public Func<string, Employee> AddNewItemEmployee { get; private set; }
        Func<string, Employee> IAddNew<Employee>.AddNewItem { get { return AddNewItemEmployee; } }

        /// <summary>
        /// An action to add an existing Employee to the current Employee.
        /// </summary>
        public Action<Employee> AddExistingItemEmployee { get; private set; }
        Action<Employee> IAddNewExisting<Employee>.AddExistingItem { get { return AddExistingItemEmployee; } }

        /// <summary>
        /// An action to remove a Employee from the current Employee.
        /// </summary>
        public Func<Employee> RemoveItemEmployee { get; private set; }
        Func<Employee> IRemove<Employee>.RemoveItem { get { return RemoveItemEmployee; } }

        /// <summary>
        /// An action to remove a Employee from the current Employee and delete it.
        /// </summary>
        public Func<Employee> DeleteItemEmployee { get; private set; }
        Func<Employee> IRemoveDelete<Employee>.DeleteItem { get { return DeleteItemEmployee; } }

        #endregion
        #region Implementation of IAddNewExisting<Vehicle> & IRemoveDelete<Vehicle>

        /// <summary>
        /// An action to add a new Vehicle to the current Route.
        /// </summary>
        public Func<string, Vehicle> AddNewItemVehicle { get; private set; }
        Func<string, Vehicle> IAddNew<Vehicle>.AddNewItem { get { return AddNewItemVehicle; } }

        /// <summary>
        /// Gets the add existing item user account.
        /// </summary>
        public Action<Vehicle> AddExistingItemVehicle { get; private set; }
        Action<Vehicle> IAddNewExisting<Vehicle>.AddExistingItem { get { return AddExistingItemVehicle; } }

        /// <summary>
        /// An action to remove a Vehicle from the current Route.
        /// </summary>
        public Func<Vehicle> RemoveItemVehicle { get; private set; }
        Func<Vehicle> IRemove<Vehicle>.RemoveItem { get { return RemoveItemVehicle; } }

        /// <summary>
        /// An action to remove a Vehicle from the current Route and delete it.
        /// </summary>
        public Func<Vehicle> DeleteItemVehicle { get; private set; }
        Func<Vehicle> IRemoveDelete<Vehicle>.DeleteItem { get { return DeleteItemVehicle; } }

        #endregion

        #endregion

        #region Locals

        /// <summary>
        /// A subscription to update the EmployeesDestinationItemsSource and VehiclesDestinationItemsSource whenever they change
        /// </summary>
        private readonly IDisposable _destinationItemsSourcesUpdatedSubscription;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteVM"/> class.
        /// </summary>
        /// <param name="route">The current route.</param>
        public RouteVM(Route route)
        {
            if (route == null)
                return;

            Route = route;

            #region Implementation of IAddToDeleteFromDestination<Employee> and IAddToDeleteFromDestination<Vehicle>

            //Notify the DestinationItemsSources changed

            //a) Whenever the current route's technicians or employees changes notify the itemssource updated.
            //  Part of a workaround for the RadGridView displaying incorrect items when an item is removed.
            _destinationItemsSourcesUpdatedSubscription = Route.Technicians.FromCollectionChangedGeneric().Merge(Route.Vehicles.FromCollectionChangedGeneric())
                .Throttle(TimeSpan.FromMilliseconds(250)).ObserveOnDispatcher().Subscribe(_ =>
                {
                    //Notify the DestinationItemsSources changed
                    this.RaisePropertyChanged("EmployeesDestinationItemsSource");
                    this.RaisePropertyChanged("VehiclesDestinationItemsSource");
                });

            #endregion

            #region Implementation of IAddNewExisting<Employee> & IRemoveDelete<Employee>

            AddNewItemEmployee = name =>
            {
                var newEmployee = VM.Employees.CreateNewItem(name);
                Route.Technicians.Add(newEmployee);
                return newEmployee;
            };

            AddExistingItemEmployee = existingItem => Route.Technicians.Add(existingItem);

            RemoveItemEmployee = () =>
            {
                var employeeToRemove = SelectedEmployee;

                //foreach (Employee t in EmployeesDestinationItemsSource)
                //    Debug.WriteLine("B " + t.DisplayName);

                this.Route.Technicians.Remove(employeeToRemove);

                //foreach (Employee t in EmployeesDestinationItemsSource)
                //    Debug.WriteLine("A " + t.DisplayName);

                return employeeToRemove;
            };

            DeleteItemEmployee = () =>
            {
                var employeeToDelete = SelectedEmployee;
                this.Route.Technicians.Remove(employeeToDelete);
                VM.Employees.DeleteEntity(employeeToDelete);
                return employeeToDelete;
            };

            #endregion

            #region Implementation of IAddNewExisting<Vehicle> & IRemoveDelete<Vehicle>

            AddNewItemVehicle = name =>
            {
                var newVehicle = VM.Vehicles.CreateNewItem(name);
                this.Route.Vehicles.Add(newVehicle);
                return newVehicle;
            };

            AddExistingItemVehicle = existingItem => Route.Vehicles.Add(existingItem);

            RemoveItemVehicle = () =>
            {
                var vehicleToRemove = SelectedVehicle;
                this.Route.Vehicles.Remove(vehicleToRemove);
                return vehicleToRemove;
            };

            DeleteItemVehicle = () =>
            {
                var selectedVehicle = RemoveItemVehicle();
                this.Route.Vehicles.Remove(selectedVehicle);
                VM.Vehicles.DeleteEntity(selectedVehicle);
                return selectedVehicle;
            };

            #endregion
        }

        #region Logic

        /// <summary>
        /// Cleans up any subscriptions.
        /// </summary>
        public void Dispose()
        {
            if (_destinationItemsSourcesUpdatedSubscription != null)
                _destinationItemsSourcesUpdatedSubscription.Dispose();
        }

        #endregion
    }
}
