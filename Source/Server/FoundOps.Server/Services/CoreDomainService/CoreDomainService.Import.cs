using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.Import;
using FoundOps.Core.Tools;
using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for importing entities.
    /// </summary>
    public partial class CoreDomainService
    {
        public bool ImportEntities(Guid currentRoleId, ImportDestination importDestination, byte[] dataCSV)
        {
            var businessAccount = ObjectContext.BusinessAccountOwnerOfRole(currentRoleId);
            if (businessAccount == null)
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            Tuple<DataCategory, string>[][] rows;

            using (var csv = new CsvReader(new MemoryStream(dataCSV)))
            {
                var headerRecord = csv.ReadHeaderRecord();
                var categories = headerRecord.Values.Select(v => (DataCategory)Enum.Parse(typeof(DataCategory), v)).ToArray();

                //Setup a Tuple<Category, string>[] for each row (DataRecord)
                //This will be used by the CreateEntity method
                rows = csv.DataRecords.Select(record => ImportRowTools.ExtractCategoriesWithValues(categories, record)).ToArray();
            }

            #region Load the necessary associations

            //If the destination is Locations or RecurringServices
            //Load clients associations with names
            Tuple<Client, string>[] clientAssociations = null;
            if (importDestination == ImportDestination.Locations || importDestination == ImportDestination.RecurringServices)
                clientAssociations = LoadClientAssociations(currentRoleId, businessAccount, rows, importDestination == ImportDestination.RecurringServices);

            IEnumerable<ServiceTemplate> serviceProviderServiceTemplates = null;
            //If the destination is Clients or RecurringServices
            //Load ServiceProvider ServiceTemplates (and sub data)
            if (importDestination == ImportDestination.Clients || importDestination == ImportDestination.RecurringServices)
                serviceProviderServiceTemplates = GetServiceProviderServiceTemplates(currentRoleId).ToArray();

            //If the destination is RecurringServices, load location associations
            Location[] locationAssociations = null;
            if (importDestination == ImportDestination.RecurringServices)
                locationAssociations = LoadLocationAssociations(currentRoleId, rows);

            #endregion

            if (importDestination == ImportDestination.Clients)
            {
                foreach (var row in rows)
                {
                    var newClient = ImportRowTools.CreateClient(businessAccount, row);

                    //Add the available services
                    foreach (var serviceTemplate in serviceProviderServiceTemplates)
                        newClient.ServiceTemplates.Add(serviceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined));

                    this.ObjectContext.Clients.AddObject(newClient);
                }
            }
            else if (importDestination == ImportDestination.Locations)
            {
                foreach (var row in rows)
                {
                    var clientAssocation = GetClientAssociation(clientAssociations, row);
                    var newLocation = ImportRowTools.CreateLocation(businessAccount, row, clientAssocation);
                    this.ObjectContext.Locations.AddObject(newLocation);
                }
            }
            else if (importDestination == ImportDestination.RecurringServices)
            {
                foreach (var row in rows)
                {
                    var clientAssocation = GetClientAssociation(clientAssociations, row);

                    //Get the service template associaton
                    var serviceTemplateName = row.GetCategoryValue(DataCategory.ServiceType);

                    var clientServiceTemplate = clientAssocation.ServiceTemplates.FirstOrDefault(st => st.Name == serviceTemplateName);
                    //If the Client does not have the available service add it to the client
                    if (clientServiceTemplate == null)
                    {
                        clientServiceTemplate = serviceProviderServiceTemplates.First(st => st.Name == serviceTemplateName).MakeChild(ServiceTemplateLevel.ClientDefined);
                        clientAssocation.ServiceTemplates.Add(clientServiceTemplate);
                    }

                    //Get the location associaton
                    var locationAssociation = GetLocationAssociation(locationAssociations, clientAssociations, row);

                    var newRecurringService = ImportRowTools.CreateRecurringService(businessAccount, row, clientServiceTemplate, clientAssocation, locationAssociation);
                }
            }

            this.ObjectContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Gets the Client association from the loaded clientAssociations and a row's Categories/Associations (if there is one).
        /// It used the DataCategory.ClientName to find the Client.
        /// </summary>
        /// <param name="clientAssociations">The loaded clientAssociations</param>
        /// <param name="row">The row's categories/values.</param>
        private static Client GetClientAssociation(IEnumerable<Tuple<Client, string>> clientAssociations, Tuple<DataCategory, string>[] row)
        {
            //Get the client association if there is one
            var clientName = row.GetCategoryValue(DataCategory.ClientName);
            Client associatedClient = null;
            if (clientName != null)
            {
                var clientTuple = clientAssociations.FirstOrDefault(ca => ca.Item2 == clientName);
                if (clientTuple != null)
                    associatedClient = clientTuple.Item1;
            }

            return associatedClient;
        }

        /// <summary>
        /// Gets the Location association from the loaded clientAssociations and a row's Categories/Associations (if there is one).
        /// It uses the DataCategory.LocationName or DataCategory.ClientName and DataCategory.LocationAddressLineOne to find the locations.
        /// </summary>
        /// <param name="locationAssociations">The loaded location associations.</param>
        /// <param name="clientAssociations">The loaded client associations</param>
        /// <param name="row">The row's categories/values.</param>
        /// <returns></returns>
        private static Location GetLocationAssociation(Location[] locationAssociations, Tuple<Client, string>[] clientAssociations, Tuple<DataCategory, string>[] row)
        {
            var locationName = row.GetCategoryValue(DataCategory.LocationName);
            Location associatedLocation = null;
            if (locationName != null)
                associatedLocation = locationAssociations.FirstOrDefault(l => l.Name == locationName);

            var clientName = row.GetCategoryValue(DataCategory.ClientName);
            var locationAddressLineOne = row.GetCategoryValue(DataCategory.LocationAddressLineOne);
            if (clientName != null && locationAddressLineOne != null && associatedLocation == null)
            {
                var clientId = clientAssociations.First(c => c.Item2 == clientName).Item1.Id;
                associatedLocation = locationAssociations.FirstOrDefault(l => l.AddressLineOne == locationAddressLineOne && l.PartyId == clientId);
            }

            return associatedLocation;
        }

        /// <summary>
        /// Loads the client associations and names from a set of rows.
        /// It uses the DataCategory.ClientName to find the client.
        /// </summary>
        /// <param name="roleId">The current role id</param>
        /// <param name="businessAccount">The current business account</param>
        /// <param name="rows">The rows of Tuple&lt;DataCategory,string&gt;[]"</param>
        /// <param name="includeAvailableServices">if set to <c>true</c> [include client service templates].</param>
        private Tuple<Client, string>[] LoadClientAssociations(Guid roleId, BusinessAccount businessAccount, Tuple<DataCategory, string>[][] rows, bool includeAvailableServices)
        {
            //Get the distinct client names from the rows' DataCategory.ClientName values
            var clientNamesToLoad = rows.Select(cvs =>
            {
                var clientNameCategoryValue = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.ClientName);
                return clientNameCategoryValue == null ? null : clientNameCategoryValue.Item2;
            }).Distinct().Where(cn => cn != null).ToArray();

            //TODO: When importing people clients, fix the ChildName logic below (probably setup 2 left joins)
            //Load all the associatedClients
            var associatedClients =
                (from client in ObjectContext.Clients.Where(c => c.VendorId == businessAccount.Id)
                 //Need to get the Clients names
                 join p in ObjectContext.PartiesWithNames
                     on client.Id equals p.Id
                 where clientNamesToLoad.Contains(p.ChildName)
                 select new { p.ChildName, client }).ToArray();

            if (includeAvailableServices)
            {
                GetServiceProviderServiceTemplates(roleId).Where(st => st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined);
            }

            return associatedClients.Select(a => new Tuple<Client, string>(a.client, a.ChildName)).ToArray();
        }

        /// <summary>
        /// Loads the Location associations from a set of rows.
        /// It uses the DataCategory.LocationName or the DataCategory.LocationAddressLineOne to find the locations.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="rows">The rows of Tuple&lt;DataCategory,string&gt;[]"</param>
        /// <returns></returns>
        private Location[] LoadLocationAssociations(Guid roleId, Tuple<DataCategory, string>[][] rows)
        {
            //Get the distinct location names from the rows' DataCategory.LocationName values
            var locationNamesToLoad = rows.Select(cvs =>
            {
                var locationNameCategoryValue = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.LocationName);
                return locationNameCategoryValue == null ? null : locationNameCategoryValue.Item2;
            }).Distinct().Where(cn => cn != null).ToArray();

            var associatedLocationsFromName = GetLocationsToAdministerForRole(roleId).Where(l => locationNamesToLoad.Contains(l.Name)).ToArray();

            //Get the distinct addresses from the DataCategory.LocationAddressLineOne category of the rows
            var addressesToLoad = rows.Select(cvs =>
            {
                           var addressLineOneCategoryValue = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.LocationAddressLineOne);
                var addressLineOne = addressLineOneCategoryValue == null ? null : addressLineOneCategoryValue.Item2;
                return addressLineOne;
            }).Distinct().Where(cn => cn != null).ToArray();

            var associatedLocationsFromAddressLineOne = GetLocationsToAdministerForRole(roleId).Where(l => addressesToLoad.Contains(l.AddressLineOne)).ToArray();

            return associatedLocationsFromName.Union(associatedLocationsFromAddressLineOne).Distinct().ToArray();
        }

        //foreach (var locationClientAssociation in locationClientAssociationsToHookup)
        //{
        //    var associatedClient =
        //        associatedClients.FirstOrDefault(c => c.ChildName == locationClientAssociation.Item2);

        //    if (associatedClient == null) continue;
        //    //If the associatedClient was found set the Location's PartyId to this Client's (OwnerParty) Id
        //    //and set the associatedClient's DefaultBillingLocation to the associatedLocation

        //    locationClientAssociation.Item1.PartyId = associatedClient.client.Id;
        //    associatedClient.client.DefaultBillingLocationId = locationClientAssociation.Item1.Id;
        //}

        //    var locationNamesToLoad = clientLocationAssociationsToHookup.Select(cl => cl.Item2).Distinct();

        //    //Load all the associatedLocations
        //    var associatedLocations = (from location in this.ObjectContext.Locations.Where(l => l.OwnerPartyId == businessAccount.Id)
        //                               where locationNamesToLoad.Contains(location.Name)
        //                               select location).ToArray();

        //    foreach (var clientLocationAssociation in clientLocationAssociationsToHookup)
        //    {
        //        var associatedLocation = associatedLocations.FirstOrDefault(l => l.Name == clientLocationAssociation.Item2);

        //        if (associatedLocation == null) continue;
        //        //If the associatedLocation was found set it's PartyId to this Client's (OwnerParty) Id
        //        //and set the Client's DefaultBillingLocation to the associatedLocation

        //        associatedLocation.PartyId = clientLocationAssociation.Item1.Id;
        //        clientLocationAssociation.Item1.DefaultBillingLocationId = clientLocationAssociation.Item1.Id;
        //    }
        //}

    }
}
