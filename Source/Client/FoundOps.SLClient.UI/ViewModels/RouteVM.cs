﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Tools;
using Analytics = FoundOps.SLClient.Data.Services.Analytics;

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

        private List<IDisposable> addToDeleteFromSubscriptions = new List<IDisposable>();
        /// <summary>
        /// Subscribes to the add delete from control observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void LinkToAddToDeleteFrom(AddToDeleteFrom control, Type sourceType)
        {
            if (sourceType == typeof(Employee))
            {
                addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemEmployee(existingItem)));
                addToDeleteFromSubscriptions.Add(control.AddNewItem.Subscribe(text => AddNewItemEmployee(text)));
                addToDeleteFromSubscriptions.Add(control.RemoveItem.Subscribe(_ => RemoveItemEmployee()));
                addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemEmployee()));
            }

            if (sourceType == typeof(Vehicle))
            {
                addToDeleteFromSubscriptions.Add(control.AddExistingItem.Subscribe(existingItem => AddExistingItemVehicle(existingItem)));
                addToDeleteFromSubscriptions.Add(control.AddNewItem.Subscribe(text => AddNewItemVehicle(text)));
                addToDeleteFromSubscriptions.Add(control.RemoveItem.Subscribe(_ => RemoveItemVehicle()));
                addToDeleteFromSubscriptions.Add(control.DeleteItem.Subscribe(_ => DeleteItemVehicle()));
            }
        }

        /// <summary>
        /// Disposes the subscriptions to the add delete from control observables.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">Type of the source.</param>
        public void UnlinkAddToDeleteFrom(AddToDeleteFrom c, Type type)
        {
            foreach (var subscription in addToDeleteFromSubscriptions)
                subscription.Dispose();

            addToDeleteFromSubscriptions.Clear();
        }

        /// <summary>
        /// Gets the user account destination items source.
        /// ToArray creates a new IEnumerable, part of a workaround for the RadGridView displaying incorrect items when an item is removed.
        /// </summary>
        public IEnumerable EmployeesDestinationItemsSource { get { return Route == null ? null : Route.Employees.ToArray(); } }

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
        public Action<object> AddExistingItemEmployee { get; private set; }
        Action<object> IAddNewExisting<Employee>.AddExistingItem { get { return AddExistingItemEmployee; } }

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
        public Action<object> AddExistingItemVehicle { get; private set; }
        Action<object> IAddNewExisting<Vehicle>.AddExistingItem { get { return AddExistingItemVehicle; } }

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
            Route = route;

            #region Implementation of IAddToDeleteFromDestination<Employee> and IAddToDeleteFromDestination<Vehicle>

            //Notify the DestinationItemsSources changed

            //a) Whenever the current route's technicians or employees changes notify the itemssource updated.
            //  Part of a workaround for the RadGridView displaying incorrect items when an item is removed.
            if (Route != null)
                _destinationItemsSourcesUpdatedSubscription = Route.Employees.FromCollectionChangedGeneric().Merge(Route.Vehicles.FromCollectionChangedGeneric())
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
                if (Route == null)
                    return null;

                var newEmployee = VM.Employees.CreateNewItem(name);
                Route.Employees.Add(newEmployee);

                Analytics.Track("Add Employee To Route");

                return newEmployee;
            };

            AddExistingItemEmployee = existingItem =>
            {
                if (Route == null)
                    return;

                if (!Route.Employees.Contains(existingItem))
                    Route.Employees.Add((Employee)existingItem);

                Analytics.Track("Add Employee To Route");
            };
            RemoveItemEmployee = () =>
            {
                if (Route == null)
                    return null;

                var employeeToRemove = SelectedEmployee;

                //foreach (Employee t in EmployeesDestinationItemsSource)
                //    Debug.WriteLine("B " + t.DisplayName);

                this.Route.Employees.Remove(employeeToRemove);

                //foreach (Employee t in EmployeesDestinationItemsSource)
                //    Debug.WriteLine("A " + t.DisplayName);

                return employeeToRemove;
            };

            DeleteItemEmployee = () =>
            {
                if (Route == null)
                    return null;

                var employeeToDelete = SelectedEmployee;
                this.Route.Employees.Remove(employeeToDelete);
                VM.Employees.DeleteEntity(employeeToDelete);
                return employeeToDelete;
            };

            #endregion

            #region Implementation of IAddNewExisting<Vehicle> & IRemoveDelete<Vehicle>

            AddNewItemVehicle = name =>
            {
                if (Route == null)
                    return null;

                var newVehicle = VM.Vehicles.CreateNewItem(name);
                this.Route.Vehicles.Add(newVehicle);

                Analytics.Track("Add Vehicle To Route");

                return newVehicle;
            };

            AddExistingItemVehicle = existingItem =>
            {
                if (Route == null)
                    return;

                if (!Route.Vehicles.Contains(existingItem))
                    Route.Vehicles.Add((Vehicle)existingItem);

                Analytics.Track("Add Vehicle To Route");
            };

            RemoveItemVehicle = () =>
            {
                if (Route == null)
                    return null;

                var vehicleToRemove = SelectedVehicle;
                this.Route.Vehicles.Remove(vehicleToRemove);
                return vehicleToRemove;
            };

            DeleteItemVehicle = () =>
            {
                if (Route == null)
                    return null;

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
