﻿using FoundOps.Common.NET;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
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
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null) return null;

            var clients =
              from c in ObjectContext.Clients.Where(c => c.VendorId == businessForRole.Id)
              join p in ObjectContext.PartiesWithNames
                  on c.Id equals p.Id
              orderby p.ChildName
              select c;

            //Client's images are not currently used, so this can be commented out
            //TODO: Figure a way to force load OwnedParty.PartyImage. 
            //(from c in clients
            // join pi in this.ObjectContext.Files.OfType<PartyImage>()
            //     on c.OwnedParty.PartyImage.Id equals pi.Id
            // select pi).ToArray();

            return clients.Include("OwnedParty");
        }

        /// <summary>
        /// Returns the clients for the current role.
        /// It also includes the Client.OwnedParty and Client.OwnedParty.ContactInfoSet
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="clientsIds">The ids of the Clients to load.</param>
        [Query(HasSideEffects = true)] //HasSideEffects so a POST is used and a maximum URI length is not thrown
        public IQueryable<Client> GetClientsWithContactInfoSet(Guid roleId, IEnumerable<Guid> clientsIds)
        {
            var clients = GetClientsForRole(roleId).Where(c => clientsIds.Contains(c.Id)).Include(c => c.OwnedParty.ContactInfoSet);
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
            var ownerParty = ObjectContext.OwnerPartyOfRole(roleId);
            var clientsWithDisplayName =
                from c in ObjectContext.Clients.Where(c => c.VendorId == ownerParty.Id)
                join p in ObjectContext.PartiesWithNames
                    on c.Id equals p.Id
                orderby p.ChildName
                select new { c, p.ChildName };

            if (!String.IsNullOrEmpty(searchText))
                clientsWithDisplayName = clientsWithDisplayName.Where(cdn => cdn.ChildName.StartsWith(searchText));

            return clientsWithDisplayName.Select(c => c.c).Include("OwnedParty");
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

        #region ClientTitle

        public IQueryable<ClientTitle> GetClientTitlesForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null)
                return null;

            var clientTitles =
                ((ObjectQuery<ClientTitle>)
                 this.ObjectContext.ClientTitles.Include("Client").Where(
                     clientTitle => clientTitle.Client.VendorId == businessForRole.Id)).Include("Client.OwnedParty");

            return clientTitles;
        }

        public void InsertClientTitle(ClientTitle clientTitle)
        {
            if ((clientTitle.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(clientTitle, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ClientTitles.AddObject(clientTitle);
            }
        }

        public void UpdateClientTitle(ClientTitle currentClientTitle)
        {
            this.ObjectContext.ClientTitles.AttachAsModified(currentClientTitle);
        }

        public void DeleteClientTitle(ClientTitle clientTitle)
        {
            if ((clientTitle.EntityState == EntityState.Detached))
                this.ObjectContext.ClientTitles.Attach(clientTitle);

            this.ObjectContext.ClientTitles.DeleteObject(clientTitle);
        }

        #endregion

        #region Contact

        /// <summary>
        /// Gets the contacts for role. Includes the OwnedPerson
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public IEnumerable<Contact> GetContactsForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null)
                return null;

            var contactsPeople =
                from contact in this.ObjectContext.Contacts.Where(contact => contact.OwnerPartyId == businessForRole.Id)
                join person in ObjectContext.Parties.OfType<Person>()
                    on contact.Id equals person.Id
                orderby person.LastName + " " + person.FirstName
                select new { contact, person };

            //Force loads the OwnedPerson (workaround for inheritance includes not working in Entity Framework)
            contactsPeople.Select(cp => cp.person).ToArray();

            return contactsPeople.Select(i => i.contact);
        }

        public void InsertContact(Contact contact)
        {
            if ((contact.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(contact, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Contacts.AddObject(contact);
            }
        }

        public void UpdateContact(Contact currentContact)
        {
            this.ObjectContext.Contacts.AttachAsModified(currentContact);
        }

        public void DeleteContact(Contact contact)
        {
            if ((contact.EntityState == EntityState.Detached))
                this.ObjectContext.Contacts.Attach(contact);

            contact.ClientTitles.Load();
            foreach (var clientTitle in contact.ClientTitles.ToArray())
                DeleteClientTitle(clientTitle);

            this.DeleteParty(contact.OwnedPerson);

            this.ObjectContext.Contacts.DeleteObject(contact);
        }

        #endregion

        #region Employee

        /// <summary>
        /// Gets the Employees the current role has access to.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        public IQueryable<Employee> GetEmployeesForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);
            if (businessForRole == null)
                return null;

            //If the account is a FoundOPS admin return all Employees
            //Otherwise return the Employees of the specified business account
            var employees = businessForRole.Id == BusinessAccountsDesignData.FoundOps.Id
                                     ? ObjectContext.Employees
                                     : ObjectContext.Employees.Where(c => c.EmployerId == businessForRole.Id);

            var employeesPeople =
                from employee in employees
                join person in ObjectContext.Parties.OfType<Person>()
                    on employee.Id equals person.Id
                orderby person.LastName + " " + person.FirstName
                select new { employee, person };

            //TODO: optimize this
            //See http://stackoverflow.com/questions/5699583/when-how-does-a-ria-services-query-get-added-to-ef-expression-tree
            //http://stackoverflow.com/questions/8358681/ria-services-domainservice-query-with-ef-projection-that-calls-method-and-still
            //Force load OwnedPerson
            //Workaround http://stackoverflow.com/questions/6648895/ef-4-1-inheritance-and-shared-primary-key-association-the-resulttype-of-the-s
            employeesPeople.Select(cp => cp.person).ToArray();

            return employeesPeople.Select(i => i.employee);
        }

        /// <summary>
        /// Gets the employee details.
        /// It includes the OwnedPerson, ContactInfoSet, LinkedUserAccount, and PartyImage.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="employeeId">The employee id.</param>
        public Employee GetEmployeeDetailsForRole(Guid roleId, Guid employeeId)
        {
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            var employee = ObjectContext.Employees.FirstOrDefault(e => e.Id == employeeId && e.EmployerId == partyForRole.Id);
            if (employee == null) return null;

            //Force load contact info set
            var contactInfoSet =
                this.ObjectContext.Parties.OfType<Person>().Where(p => p.Id == employee.Id).Select(p => p.ContactInfoSet)
                    .FirstOrDefault();

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
            var employees = GetEmployeesForRole(roleId);

            if (!String.IsNullOrEmpty(searchText))
                employees = employees.Where(e =>
                    e.OwnedPerson.FirstName.StartsWith(searchText) || e.OwnedPerson.LastName.StartsWith(searchText) 
                    || (e.OwnedPerson.FirstName + " " + e.OwnedPerson.LastName).Contains(searchText));

            return employees.OrderBy(e => e.OwnedPerson.LastName + " " + e.OwnedPerson.FirstName);
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

            employee.OwnedPersonReference.Load();
            if (employee.OwnedPerson != null)
                this.DeleteParty(employee.OwnedPerson);

            employee.Routes.Load();
            employee.Routes.Clear();

            this.ObjectContext.Employees.DeleteObject(employee);
        }

        #endregion

        #region EmployeeHistoryEntry

        public IQueryable<EmployeeHistoryEntry> GetEmployeeHistoryEntriesForRole(Guid roleId)
        {
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            if (businessForRole == null)
                return null;

            return ObjectContext.EmployeeHistoryEntries.Where(e => e.Employee.EmployerId == businessForRole.Id).
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
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            var locations = ObjectContext.Locations.Where(loc => loc.OwnerPartyId == partyForRole.Id).OrderBy(l => l.Name);
            return locations;
        }

        /// <summary>
        /// Gets the locations for  role's business account.
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
        /// Searches the locations for the current role. Uses a StartsWith search mode.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public IQueryable<Location> SearchLocationsForRole(Guid roleId, string searchText)
        {
            var locations = GetLocationsToAdministerForRole(roleId);

            if (!String.IsNullOrEmpty(searchText))
                locations = locations.Where(l => l.Name.StartsWith(searchText)).OrderBy(l => l.Name);

            return locations;
        }

        /// <summary>
        /// Gets the location details.
        /// It includes the Client, Region, SubLocations, and ContactInfoSet.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="locationId">The location id.</param>
        public Location GetLocationDetailsForRole(Guid roleId, Guid locationId)
        {
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            var location = ObjectContext.Locations.Where(l => l.Id == locationId && l.OwnerPartyId == partyForRole.Id)
                .Include("Party.ClientOwner").Include("Region").Include("SubLocations").Include("ContactInfoSet").FirstOrDefault();

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
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            var memoryStream = new MemoryStream();

            var csvWriter = new CsvWriter(memoryStream);

            csvWriter.WriteHeaderRecord("Name", "Region", "Client",
                                        "Address 1", "Address 2", "City", "State", "Zip Code",
                                        "Latitude", "Longitude");

            var locations = ObjectContext.Locations.Where(loc => loc.OwnerPartyId == partyForRole.Id);

            //Add client context if it exists
            if (clientId != Guid.Empty)
                locations = locations.Where(loc => loc.PartyId == clientId);

            //Add region context if it exists
            if (regionId != Guid.Empty)
                locations = locations.Where(loc => loc.RegionId == regionId);

            var records = from loc in locations
                          //Get the Clients names
                          join p in ObjectContext.PartiesWithNames
                              on loc.Party.Id equals p.Id
                          orderby loc.Name
                          select new
                          {
                              loc.Name,
                              RegionName = loc.Region.Name,
                              ClientName = p.ChildName,
                              loc.AddressLineOne,
                              loc.AddressLineTwo,
                              loc.City,
                              loc.State,
                              loc.ZipCode,
                              loc.Latitude,
                              loc.Longitude
                          };

            foreach (var record in records.ToArray())
                csvWriter.WriteDataRecord(record.Name, record.RegionName, record.ClientName,
                                       record.AddressLineOne, record.AddressLineTwo, record.City, record.State, record.ZipCode,
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
            var businessForRole = ObjectContext.BusinessOwnerOfRole(roleId);

            var regions = from region in this.ObjectContext.Regions.Where(v => v.BusinessAccountId == businessForRole.Id)
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
            //TODO: Also include Factual.com local business data
            return YahooPlaceFinder.TryGeocode(searchText);
        }

        #region Vehicle

        public IQueryable<Vehicle> GetVehiclesForParty(Guid roleId)
        {
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);
            return this.ObjectContext.Vehicles.Where(v => v.OwnerPartyId == partyForRole.Id).OrderBy(v => v.VehicleId);
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
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            return
                this.ObjectContext.VehicleMaintenanceLineItems.Include("VehicleMaintenanceLogEntry").Where(
                    vm => vm.VehicleMaintenanceLogEntry.Vehicle.OwnerPartyId == partyForRole.Id);
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
            var partyForRole = ObjectContext.OwnerPartyOfRole(roleId);

            return ObjectContext.VehicleMaintenanceLog.Where(vm => vm.Vehicle.OwnerPartyId == partyForRole.Id)
                .Include("LineItems").OrderBy(vm => vm.Date);
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

