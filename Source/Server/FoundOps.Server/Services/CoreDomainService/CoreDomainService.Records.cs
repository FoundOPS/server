using FoundOps.Common.NET;
using FoundOps.Core.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.ServiceModel.DomainServices.EntityFramework;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for any record entities:
    /// Clients, Contacts & ClientTitles, Employees & Employee History
    /// Locations, Regions, SubLocations, Vehicles & Vehicle History
    /// </summary>
    public partial class CoreDomainService
    {
        #region Client

        /// <summary>
        /// Gets the clients for the role id.
        /// NOTE: Skip and take are required to limit the force load workaround below.
        /// NOTE: Must add any other limiting parts as parameters to get full effect of their optimization. 
        /// Otherwise the force load of PartyImage will happen on unnecessary items.
        /// </summary>
        /// <param name="roleId">The role to determine the businessaccount from.</param>
        /// <returns></returns>
        public IQueryable<Client> GetClientsForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var clients = ObjectContext.Clients.Where(c => c.BusinessAccountId == businessAccount.Id);

            //Client's images are not currently used, so this can be commented out
            //TODO: Figure a way to force load OwnedParty.PartyImage. 
            //(from c in clients
            // join pi in this.ObjectContext.Files.OfType<PartyImage>()
            //     on c.PartyImage.Id equals pi.Id
            // select pi).ToArray();

            return clients.OrderBy(c => c.Name);
        }

        /// <summary>
        /// Gets the Client details.
        /// It includes the ContactInfo, Available Services (ServiceTemplates),
        /// RecurringServices, RecurringServices.Repeat, RecurringServices.ServiceTemplate (without fields)
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="clientId">The client id.</param>
        public Client GetClientDetailsForRole(Guid roleId, Guid clientId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            //Load contact info
            var client = ObjectContext.Clients.Where(c => c.BusinessAccountId == businessAccount.Id && c.Id == clientId)
                .Include(c => c.ContactInfoSet).Include(c => c.RecurringServices)
                .Include("RecurringServices.Repeat").Include("RecurringServices.ServiceTemplate").FirstOrDefault();

            if (client == null) return null;

            //Load clients ServiceTemplates
            GetClientServiceTemplates(businessAccount.Id, client.Id).ToArray();

            return client;
        }


        /// <summary>
        /// Returns the clients for the current role.
        /// It also includes the Client.ContactInfoSet
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="clientsIds">The ids of the Clients to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Client> GetClientsWithContactInfoSet(Guid roleId, IEnumerable<Guid> clientsIds)
        {
            var clients = GetClientsForRole(roleId).Where(c => clientsIds.Contains(c.Id)).Include(c => c.ContactInfoSet);
            return clients;
        }

        /// <summary>
        /// Searches the clients for the current role. Uses a StartsWith search mode.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Client> SearchClientsForRole(Guid roleId, string searchText)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();
            var clientsWithDisplayName = ObjectContext.Clients.Where(c => c.BusinessAccountId == businessAccount.Id);

            if (!String.IsNullOrEmpty(searchText))
                clientsWithDisplayName = clientsWithDisplayName.Where(cdn => cdn.Name.StartsWith(searchText));

            return clientsWithDisplayName;
        }

        public void InsertClient(Client client)
        {
            if ((client.EntityState != EntityState.Detached))
                this.ObjectContext.ObjectStateManager.ChangeObjectState(client, EntityState.Added);
            else
                this.ObjectContext.Clients.AddObject(client);
        }

        public void UpdateClient(Client currentClient)
        {
            this.ObjectContext.Clients.AttachAsModified(currentClient);
        }

        public void DeleteClient(Client client)
        {
            ObjectContext.DeleteClientBasedOnId(client.Id);
        }

        #endregion

        #region Employee

        /// <summary>
        /// Gets the Employees the current role has access to.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        public IQueryable<Employee> GetEmployeesForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            //If the account is a FoundOPS admin return all Employees
            //Otherwise return the Employees of the specified business account
            var employees = businessAccount.Id == BusinessAccountsDesignData.FoundOps.Id
                                     ? ObjectContext.Employees
                                     : ObjectContext.Employees.Where(c => c.EmployerId == businessAccount.Id);

            return employees.OrderBy(e => e.FirstName);
        }

        /// <summary>
        /// Gets the employee details.
        /// It includes the OwnedPerson, ContactInfoSet, LinkedUserAccount, and PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="employeeId">The employee id.</param>
        public Employee GetEmployeeDetailsForRole(Guid roleId, Guid employeeId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var employee = ObjectContext.Employees.FirstOrDefault(e => e.Id == employeeId && e.EmployerId == businessAccount.Id);
            if (employee == null) return null;

            //Force load party image
            var partyImage = this.ObjectContext.Files.OfType<PartyImage>().FirstOrDefault(pi => pi.PartyId == employeeId);

            //Force load linked user account
            var linkedUserAccount = this.ObjectContext.Parties.OfType<UserAccount>().FirstOrDefault(ua => ua.LinkedEmployees.Any(le => le.Id == employeeId));

            return employee;
        }

        /// <summary>
        /// Gets the Employees the current role has access to filtered by the search text.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Employee> SearchEmployeesForRole(Guid roleId, string searchText)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            //If the account is a FoundOPS admin return all Employees
            //Otherwise return the Employees of the specified business account
            var employees = businessAccount.Id == BusinessAccountsDesignData.FoundOps.Id
                                     ? ObjectContext.Employees
                                     : ObjectContext.Employees.Where(c => c.EmployerId == businessAccount.Id);

            if (!String.IsNullOrEmpty(searchText))
                employees = employees.Where(e =>
                    e.FirstName.StartsWith(searchText) || e.LastName.StartsWith(searchText)
                    || searchText.StartsWith(e.FirstName) || searchText.Contains(e.LastName));

            return employees;
        }

        public void InsertEmployee(Employee employee)
        {
            if ((employee.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(employee, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Employees.AddObject(employee);
            }
        }

        public void UpdateEmployee(Employee currentEmployee)
        {
            this.ObjectContext.Employees.AttachAsModified(currentEmployee);
        }

        public void DeleteEmployee(Employee employee)
        {
            if ((employee.EntityState == EntityState.Detached))
                this.ObjectContext.Employees.Attach(employee);

            employee.Employer = null;
            employee.LinkedUserAccount = null;

            employee.EmployeeHistoryEntries.Load();
            employee.EmployeeHistoryEntries.ToArray();
            foreach (var employeeHistoryEntry in employee.EmployeeHistoryEntries.ToArray())
                this.DeleteEmployeeHistoryEntry(employeeHistoryEntry);

            employee.Routes.Load();
            employee.Routes.Clear();

            this.ObjectContext.Employees.DeleteObject(employee);
        }

        #endregion

        #region EmployeeHistoryEntry

        public IQueryable<EmployeeHistoryEntry> GetEmployeeHistoryEntriesForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            return ObjectContext.EmployeeHistoryEntries.Where(e => e.Employee.EmployerId == businessAccount.Id).
                    OrderBy(ehe => ehe.Date);
        }

        public void InsertEmployeeHistoryEntry(EmployeeHistoryEntry employeeHistoryEntry)
        {
            if ((employeeHistoryEntry.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(employeeHistoryEntry, EntityState.Added);
            }
            else
            {
                this.ObjectContext.EmployeeHistoryEntries.AddObject(employeeHistoryEntry);
            }
        }

        public void UpdateEmployeeHistoryEntry(EmployeeHistoryEntry currentEmployeeHistoryEntry)
        {
            this.ObjectContext.EmployeeHistoryEntries.AttachAsModified(currentEmployeeHistoryEntry);
        }

        public void DeleteEmployeeHistoryEntry(EmployeeHistoryEntry employeeHistoryEntry)
        {
            if ((employeeHistoryEntry.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(employeeHistoryEntry, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.EmployeeHistoryEntries.Attach(employeeHistoryEntry);
                this.ObjectContext.EmployeeHistoryEntries.DeleteObject(employeeHistoryEntry);
            }
        }

        #endregion

        #region Location

        /// <summary> 
        /// Gets the locations for the role's business account.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <returns>The Business's Client's Locations</returns>
        public IQueryable<Location> GetLocationsToAdministerForRole(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var locations = ObjectContext.Locations.Where(loc => loc.BusinessAccountId == businessAccount.Id && !loc.BusinessAccountIdIfDepot.HasValue)
                .OrderBy(l => l.Name).ThenBy(l=>l.AddressLineOne);
            return locations;
        }

        /// <summary>
        /// Gets the locations for role's business account.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="locationsIds">The ids of the Locations to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Location> GetLocations(Guid roleId, IEnumerable<Guid> locationsIds)
        {
            var locations = GetLocationsToAdministerForRole(roleId).Where(l => locationsIds.Contains(l.Id));
            return locations;
        }

        /// <summary>
        /// Gets the locations for role's business account.
        /// It also includes the ContactInfoSet.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="locationsIds">The ids of the Locations to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Location> GetLocationsWithContactInfoSet(Guid roleId, IEnumerable<Guid> locationsIds)
        {
            var locations = GetLocationsToAdministerForRole(roleId).Where(l => locationsIds.Contains(l.Id)).Include(l => l.ContactInfoSet);
            return locations;
        }

        /// <summary>
        /// Searches the locations for the current role. Where the address line one starts with the search text.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Location> SearchLocationsForRole(Guid roleId, string searchText)
        {
            var locations = GetLocationsToAdministerForRole(roleId);

            if (!string.IsNullOrEmpty(searchText))
                locations = locations.Where(l => l.Name.StartsWith(searchText) || l.AddressLineOne.StartsWith(searchText));

            return locations.OrderBy(l => l.Name).ThenBy(l => l.AddressLineOne);
        }

        /// <summary>
        /// Gets the location details.
        /// It includes the Client, Region, SubLocations, and ContactInfoSet.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="locationId">The location id.</param>
        public Location GetLocationDetailsForRole(Guid roleId, Guid locationId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var location = ObjectContext.Locations.Where(l => l.Id == locationId && l.BusinessAccountId == businessAccount.Id)
                .Include(l => l.Client).Include(l => l.Region).Include(l => l.SubLocations).Include(l => l.ContactInfoSet).FirstOrDefault();

            return location;
        }

        /// <summary>
        /// Gets the locations CSV for a role.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="clientId">The optional client id filter.</param>
        /// <param name="regionId">The optional region id filter.</param>
        /// <returns></returns>
        public byte[] GetLocationsCSVForRole(Guid roleId, Guid clientId, Guid regionId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            var memoryStream = new MemoryStream();

            var csvWriter = new CsvWriter(memoryStream);

            csvWriter.WriteHeaderRecord("Client", "Location", "Address 1", "Address 2", "City", "State", "Zip Code",
                                        "Region", "Latitude", "Longitude");

            var locations = ObjectContext.Locations.Where(loc => loc.BusinessAccountId == businessAccount.Id && !loc.BusinessAccountIdIfDepot.HasValue);

            //Add client context if it exists
            if (clientId != Guid.Empty)
                locations = locations.Where(loc => loc.ClientId == clientId);

            //Add region context if it exists
            if (regionId != Guid.Empty)
                locations = locations.Where(loc => loc.RegionId == regionId);

            var records = from loc in locations
                          //Get the Clients names
                          join c in ObjectContext.Clients
                              on loc.Client.Id equals c.Id
                          orderby loc.AddressLineOne
                          select new
                          {
                              ClientName = c.Name,
                              loc.Name,
                              loc.AddressLineOne,
                              loc.AddressLineTwo,
                              loc.City,
                              loc.State,
                              loc.ZipCode,
                              RegionName = loc.Region.Name,
                              loc.Latitude,
                              loc.Longitude
                          };

            foreach (var record in records.ToArray())
                csvWriter.WriteDataRecord(record.ClientName, record.Name, record.AddressLineOne, record.AddressLineTwo, record.City, record.State, record.ZipCode, record.RegionName,
                                       record.Latitude, record.Longitude);

            csvWriter.Close();

            var csv = memoryStream.ToArray();
            memoryStream.Dispose();

            return csv;
        }

        public void InsertLocation(Location location)
        {
            if ((location.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(location, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Locations.AddObject(location);
            }
        }

        public void UpdateLocation(Location currentLocation)
        {
            this.ObjectContext.Locations.AttachAsModified(currentLocation);
        }

        /// <summary>
        /// Deletes the location.
        /// </summary>
        /// <param name="location">The location.</param>
        public void DeleteLocation(Location location)
        {
            ObjectContext.DeleteLocationBasedOnId(location.Id);
        }

        #endregion

        #region Region

        /// <summary>
        /// Gets the regions for the role's business account.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        public IQueryable<Region> GetRegionsForServiceProvider(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).FirstOrDefault();

            if (businessAccount == null)
                return null;

            var regions = from region in this.ObjectContext.Regions.Where(v => v.BusinessAccountId == businessAccount.Id)
                          orderby region.Name
                          select region;

            return regions;
        }

        /// <summary>
        /// Searches the regions for the current role. Uses a StartsWith search mode.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="searchText">The search text.</param>
        public IQueryable<Region> SearchRegionsForRole(Guid roleId, string searchText)
        {
            var regions = GetRegionsForServiceProvider(roleId);

            if (!String.IsNullOrEmpty(searchText))
                regions = regions.Where(r => r.Name.StartsWith(searchText));

            return regions;
        }

        public void InsertRegion(Region region)
        {
            if ((region.EntityState == EntityState.Detached))
                this.ObjectContext.Regions.Attach(region);

            region.BusinessAccount = null;

            region.Locations.Load();
            region.Locations.Clear();

            if ((region.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(region, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Regions.AddObject(region);
            }
        }

        public void UpdateRegion(Region currentRegion)
        {
            this.ObjectContext.Regions.AttachAsModified(currentRegion);
        }

        public void DeleteRegion(Region region)
        {
            if ((region.EntityState == EntityState.Detached))
                this.ObjectContext.Regions.Attach(region);

            region.Locations.Load();
            region.Locations.Clear();

            region.BusinessAccount = null;

            if ((region.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(region, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Regions.Attach(region);
                this.ObjectContext.Regions.DeleteObject(region);
            }
        }

        #endregion

        #region SubLocation

        public IQueryable<SubLocation> GetSubLocations()
        {
            throw new NotSupportedException("Exists solely to generate SubLocation in the clients data project");
        }

        public void InsertSubLocation(SubLocation subLocation)
        {
            if ((subLocation.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(subLocation, EntityState.Added);
            }
            else
            {
                this.ObjectContext.SubLocations.AddObject(subLocation);
            }
        }

        public void UpdateSubLocation(SubLocation currentSubLocation)
        {
            this.ObjectContext.SubLocations.AttachAsModified(currentSubLocation);
        }

        public void DeleteSubLocation(SubLocation subLocation)
        {
            var loadedSubLocation = this.ObjectContext.SubLocations.FirstOrDefault(sl => sl.Id == subLocation.Id);

            if (loadedSubLocation != null)
                this.ObjectContext.Detach(loadedSubLocation);

            if (subLocation.EntityState == EntityState.Detached)
                this.ObjectContext.SubLocations.Attach(subLocation);
            this.ObjectContext.SubLocations.DeleteObject(subLocation);
        }

        #endregion

        [Invoke]
        public IEnumerable<GeocoderResult> TryGeocode(string searchText)
        {
            return BingLocationServices.TryGeocode(searchText);
        }

        [Invoke]
        public IEnumerable<GeocoderResult> TryGeocodeAddress(string addressLineOne, string city, string state, string zipCode)
        {
            return
                BingLocationServices.TryGeocode(new Address { AddressLineOne = addressLineOne, City = city, State = state, ZipCode = zipCode });
        }

        #region Vehicle

        public IQueryable<Vehicle> GetVehiclesForParty(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();
            return this.ObjectContext.Vehicles.Where(v => v.BusinessAccountId == businessAccount.Id).OrderBy(v => v.VehicleId);
        }

        /// <summary>
        /// Searches the vehicles for the current role. Uses a StartsWith search mode.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Vehicle> SearchVehiclesForRole(Guid roleId, string searchText)
        {
            var vehiclesQueryable = GetVehiclesForParty(roleId);

            if (!String.IsNullOrEmpty(searchText))
                vehiclesQueryable = vehiclesQueryable.Where(v => v.VehicleId.StartsWith(searchText)).OrderBy(v => v.VehicleId);

            return vehiclesQueryable;
        }

        public void InsertVehicle(Vehicle vehicle)
        {
            if ((vehicle.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicle, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Vehicles.AddObject(vehicle);
            }
        }

        public void UpdateVehicle(Vehicle currentVehicle)
        {
            this.ObjectContext.Vehicles.AttachAsModified(currentVehicle);
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            if ((vehicle.EntityState == EntityState.Detached))
                this.ObjectContext.Vehicles.Attach(vehicle);

            //Clear VehicleMaintenance
            vehicle.VehicleMaintenanceLog.Load();
            var vehicleMaintenanceLogEntries = vehicle.VehicleMaintenanceLog.ToArray();

            foreach (var vehicleMaintenanceLogEntry in vehicleMaintenanceLogEntries)
                this.DeleteVehicleMaintenanceLogEntry(vehicleMaintenanceLogEntry);

            //Remove routes from Vehicle
            vehicle.Routes.Load();
            vehicle.Routes.Clear();

            if ((vehicle.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicle, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Vehicles.DeleteObject(vehicle);
            }
        }

        #endregion

        #region VehicleMaintenanceLineItem

        public IQueryable<VehicleMaintenanceLineItem> GetVehicleMaintenanceLineItemsForParty(Guid roleId)
        {
            var party = ObjectContext.Owner(roleId).First();

            return
                this.ObjectContext.VehicleMaintenanceLineItems.Include(vm => vm.VehicleMaintenanceLogEntry)
                .Where(vm => vm.VehicleMaintenanceLogEntry.Vehicle.BusinessAccountId == party.Id);
        }

        // Get, Insert, Update, and Delete VehicleMaintenanceLineItems
        public void InsertVehicleMaintenanceLineItem(VehicleMaintenanceLineItem vehicleMaintenanceLineItem)
        {
            if ((vehicleMaintenanceLineItem.EntityState == EntityState.Detached))
                this.ObjectContext.VehicleMaintenanceLineItems.Attach(vehicleMaintenanceLineItem);

            if ((vehicleMaintenanceLineItem.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicleMaintenanceLineItem, EntityState.Added);
            }
            else
            {
                this.ObjectContext.VehicleMaintenanceLineItems.AddObject(vehicleMaintenanceLineItem);
            }
        }

        public void UpdateVehicleMaintenanceLineItem(VehicleMaintenanceLineItem currentVehicleMaintenanceLineItem)
        {
            this.ObjectContext.VehicleMaintenanceLineItems.AttachAsModified(currentVehicleMaintenanceLineItem);
        }

        public void DeleteVehicleMaintenanceLineItem(VehicleMaintenanceLineItem vehicleMaintenanceLineItem)
        {
            if ((vehicleMaintenanceLineItem.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicleMaintenanceLineItem, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.VehicleMaintenanceLineItems.Attach(vehicleMaintenanceLineItem);
                this.ObjectContext.VehicleMaintenanceLineItems.DeleteObject(vehicleMaintenanceLineItem);
            }
        }

        #endregion

        #region VehicleMaintenanceLogEntry

        public IQueryable<VehicleMaintenanceLogEntry> GetVehicleMaintenanceLogForParty(Guid roleId)
        {
            var businessAccount = ObjectContext.Owner(roleId).First();

            return ObjectContext.VehicleMaintenanceLog.Where(vm => vm.Vehicle.BusinessAccountId == businessAccount.Id)
                   .Include(vml => vml.LineItems).OrderBy(vm => vm.Date);
        }

        public void InsertVehicleMaintenanceLogEntry(VehicleMaintenanceLogEntry vehicle)
        {
            if ((vehicle.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicle, EntityState.Added);
            }
            else
            {
                this.ObjectContext.VehicleMaintenanceLog.AddObject(vehicle);
            }
        }

        public void UpdateVehicleMaintenanceLogEntry(VehicleMaintenanceLogEntry currentVehicleMaintenanceLogEntry)
        {
            this.ObjectContext.VehicleMaintenanceLog.AttachAsModified(currentVehicleMaintenanceLogEntry);
        }

        public void DeleteVehicleMaintenanceLogEntry(VehicleMaintenanceLogEntry vehicleMaintenanceLogEntry)
        {
            if ((vehicleMaintenanceLogEntry.EntityState == EntityState.Detached))
                this.ObjectContext.VehicleMaintenanceLog.Attach(vehicleMaintenanceLogEntry);

            vehicleMaintenanceLogEntry.Vehicle = null;

            vehicleMaintenanceLogEntry.LineItems.Load();
            var lineItemsToRemove = vehicleMaintenanceLogEntry.LineItems.ToArray();
            foreach (var lineItem in lineItemsToRemove)
                this.DeleteVehicleMaintenanceLineItem(lineItem);

            if ((vehicleMaintenanceLogEntry.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(vehicleMaintenanceLogEntry, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.VehicleMaintenanceLog.DeleteObject(vehicleMaintenanceLogEntry);
            }
        }

        #endregion
    }
}

