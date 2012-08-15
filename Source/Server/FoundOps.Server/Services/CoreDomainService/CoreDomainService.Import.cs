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
        /// <summary>
        /// Imports the entities.
        /// </summary>
        /// <param name="roleId">The current role id.</param>
        /// <param name="importDestination">The import destination.</param>
        /// <param name="dataCSV">The data CSV.</param>
        /// <returns></returns>
        public bool ImportEntities(Guid roleId, ImportDestination importDestination, byte[] dataCSV)
        {
            var businessAccount = ObjectContext.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            //row index, column index, DataCategory, value
            var rows = new List<ImportRow>();

            using (var csv = new CsvReader(new MemoryStream(dataCSV)))
            {
                var headerRecord = csv.ReadHeaderRecord();
                var categories = headerRecord.Values.Select(v => (DataCategory)Enum.Parse(typeof(DataCategory), v)).ToArray();

                //setup a Tuple (DataRecord) for each row 
                var rowIndex = 0;
                foreach (var dataRecord in csv.DataRecords)
                {
                    rows.Add(ImportRowTools.ExtractCategoriesWithValues(rowIndex, categories, dataRecord));
                    rowIndex++;
                }
            }

            #region Load the necessary associations

            //If the destination is Locations or RecurringServices
            //Load clients associations with names
            Client[] clientAssociations = null;
            if (importDestination == ImportDestination.Locations || importDestination == ImportDestination.RecurringServices)
                clientAssociations = LoadClientAssociations(roleId, businessAccount, rows, importDestination == ImportDestination.RecurringServices);

            IEnumerable<ServiceTemplate> serviceProviderServiceTemplates = null;
            //If the destination is Clients or RecurringServices
            //Load ServiceProvider ServiceTemplates (and sub data)
            if (importDestination == ImportDestination.Clients || importDestination == ImportDestination.RecurringServices)
                serviceProviderServiceTemplates = GetServiceProviderServiceTemplates(roleId).ToArray();

            //If the destination is RecurringServices, load location associations
            Location[] locationAssociations = null;
            if (importDestination == ImportDestination.RecurringServices)
                locationAssociations = LoadLocationAssociations(roleId, rows);

            //If the destination is Locations
            //Load region associations
            Region[] regionAssociations = null;
            if (importDestination == ImportDestination.Locations)
                regionAssociations = LoadCreateRegionAssociations(roleId, businessAccount, rows);

            #endregion

            switch (importDestination)
            {
                case ImportDestination.Clients:
                    foreach (var row in rows)
                    {
                        var newClient = ImportRowTools.CreateClient(businessAccount, row);

                        if (!serviceProviderServiceTemplates.Any())
                            throw new Exception("This account does not have any service templates yet");

                        //Add the available services
                        foreach (var serviceTemplate in serviceProviderServiceTemplates)
                            newClient.ServiceTemplates.Add(serviceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined));

                        this.ObjectContext.Clients.AddObject(newClient);
                    }
                    break;
                case ImportDestination.Locations:
                    foreach (var row in rows)
                    {
                        var clientAssocation = GetClientAssociation(clientAssociations, row);
                        var regionAssociation = GetRegionAssociation(regionAssociations, row);
                        var newLocation = ImportRowTools.CreateLocation(businessAccount, row, clientAssocation, regionAssociation);
                        this.ObjectContext.Locations.AddObject(newLocation);
                    }
                    break;
                case ImportDestination.RecurringServices:
                    {
                        foreach (var row in rows)
                        {
                            //Get the service template associaton
                            var serviceTemplateCell = row.GetCell(DataCategory.ServiceType);
                            if (serviceTemplateCell == null)
                                throw ImportRowTools.Exception("ServiceType not set", row);

                            var clientAssocation = GetClientAssociation(clientAssociations, row);

                            var serviceTemplateName = serviceTemplateCell.Value;

                            var clientServiceTemplate = clientAssocation.ServiceTemplates.FirstOrDefault(st => st.Name == serviceTemplateName);

                            //If the Client does not have the available service add it to the client
                            if (clientServiceTemplate == null)
                            {
                                var serviceProviderServiceTemplate = serviceProviderServiceTemplates.FirstOrDefault(st => st.Name == serviceTemplateName);

                                if (serviceProviderServiceTemplate == null)
                                    throw ImportRowTools.Exception(String.Format("ServiceType '{0}' not found on this provider", serviceTemplateName), row);

                                clientServiceTemplate = serviceProviderServiceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined);

                                clientAssocation.ServiceTemplates.Add(clientServiceTemplate);
                            }

                            //Get the location associaton
                            var locationAssociation = GetLocationAssociation(locationAssociations, row);
                            var newRecurringService = ImportRowTools.CreateRecurringService(businessAccount, row, clientServiceTemplate, clientAssocation, locationAssociation);

                            this.ObjectContext.RecurringServices.AddObject(newRecurringService);
                        }
                    }
                    break;
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
        private static Client GetClientAssociation(IEnumerable<Client> clientAssociations, ImportRow row)
        {
            //Get the client association
            var clientNameCell = row.GetCell(DataCategory.ClientName);
            if (clientNameCell == null)
                throw ImportRowTools.Exception("Client not set", row);

            var associatedClient = clientAssociations.FirstOrDefault(ca => ca.Name.ToLower().Trim() == clientNameCell.Value.ToLower().Trim());
            if (associatedClient == null)
                throw ImportRowTools.Exception(String.Format("Could not find Client '{0}'", clientNameCell.Value), row);

            return associatedClient;
        }

        /// <summary>
        /// Gets the Location association from the loaded locationAssociations.
        /// It uses the DataCategory.LocationLatitude and DataCategory.LocationLongitude to find the location.
        /// </summary>
        /// <param name="locationAssociations">The loaded location associations.</param>
        /// <param name="importRow">The row's categories/values.</param>
        /// <returns></returns>
        private static Location GetLocationAssociation(IEnumerable<Location> locationAssociations, ImportRow importRow)
        {
            Location associatedLocation = null;

            var locationLatitude = importRow.GetCell(DataCategory.LocationLatitude);
            var locationLongitude = importRow.GetCell(DataCategory.LocationLongitude);
            if (locationLatitude == null)
                throw ImportRowTools.Exception("Latitude not set", importRow);

            if (locationLongitude == null)
                throw ImportRowTools.Exception("Longitude not set", importRow);

            //TODO remove

            associatedLocation =
                locationAssociations.FirstOrDefault(
                    c =>
                    c.Latitude == Convert.ToDecimal(locationLatitude.Value) &&
                    c.Longitude == Convert.ToDecimal(locationLongitude.Value));

            if (associatedLocation == null)
                throw ImportRowTools.Exception(
                    String.Format("Could not find Location with latitude '{0}', longitude '{1}'", locationLatitude, locationLongitude), importRow);

            return associatedLocation;
        }

        /// <summary>
        /// Gets the Region association from the loaded regionAssociations and a row's Categories/Associations (if there is one).
        /// It used the DataCategory.RegionName to find the Region.
        /// </summary>
        /// <param name="regionAssociations">The loaded region associations</param>
        /// <param name="row">The row's categories/values.</param>
        private static Region GetRegionAssociation(IEnumerable<Region> regionAssociations, ImportRow row)
        {
            var regionNameCell = row.GetCell(DataCategory.RegionName);
            if (regionNameCell == null || string.IsNullOrEmpty(regionNameCell.Value))
                return null;

            var region = regionAssociations.First(ca => ca.Name == regionNameCell.Value);
            return region;
        }

        /// <summary>
        /// Loads the client associations and names from a set of rows.
        /// It uses the DataCategory.ClientName to find the client.
        /// </summary>
        /// <param name="roleId">The current role id</param>
        /// <param name="businessAccount">The current business account</param>
        /// <param name="importRows">The import rows"</param>
        /// <param name="includeAvailableServices">if set to <c>true</c> [include client service templates].</param>
        private Client[] LoadClientAssociations(Guid roleId, BusinessAccount businessAccount, IEnumerable<ImportRow> importRows, bool includeAvailableServices)
        {
            //Get the distinct client names from the rows' DataCategory.ClientName values
            var clientNamesToLoad = importRows.Select(importRow =>
            {
                var clientNameCategoryValue = importRow.FirstOrDefault(cv => cv.DataCategory == DataCategory.ClientName);
                return clientNameCategoryValue == null ? null : clientNameCategoryValue.Value;
            }).Distinct().Where(cn => cn != null).ToArray();

            //TODO: When importing people clients, fix the ChildName logic below (probably setup 2 left joins)
            //Load all the associatedClients
            var associatedClients =
                (from client in ObjectContext.Clients.Where(c => c.BusinessAccountId == businessAccount.Id)
                 where clientNamesToLoad.Contains(client.Name)
                 select client).ToArray();

            if (includeAvailableServices)
            {
                GetServiceProviderServiceTemplates(roleId).Where(st => st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined);
            }

            return associatedClients.ToArray();
        }

        /// <summary>
        /// Loads the Location associations from a set of rows.
        /// It uses the DataCategory.LocationLatitude or the DataCategory.LocationLongitude to find the locations.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="importRows">The import rows"</param>
        /// <returns></returns>
        private Location[] LoadLocationAssociations(Guid roleId, IEnumerable<ImportRow> importRows)
        {
            //Get the distinct latitude/longitudes from the rows
            var latsToLoad = importRows.Select(importRow =>
            {
                var latString = importRow.FirstOrDefault(cv => cv.DataCategory == DataCategory.LocationLatitude);
                if (latString == null || string.IsNullOrEmpty(latString.Value))
                    return null;
                //set precision to 6 decimal places (< 1 meter precision)
                return (decimal?)Math.Truncate(1000000 * Convert.ToDecimal(latString.Value)) / 1000000;
            }).Distinct().Where(cn => cn != null).ToArray();

            var lngsToLoad = importRows.Select(cvs =>
            {
                var lngString = cvs.FirstOrDefault(cv => cv.DataCategory == DataCategory.LocationLongitude);
                if (lngString == null || string.IsNullOrEmpty(lngString.Value))
                    return null;
                //set precision to 6 decimal places (< 1 meter precision)
                return (decimal?)Math.Truncate(1000000 * Convert.ToDecimal(lngString.Value)) / 1000000;
            }).Distinct().Where(cn => cn != null).ToArray();

            var associatedLocations =
                (from location in GetLocationsToAdministerForRole(roleId).Where(l => l.Latitude.HasValue && l.Longitude.HasValue)
                 let latitude = System.Data.Objects.EntityFunctions.Truncate(location.Latitude.Value, 6)
                 let longitude = System.Data.Objects.EntityFunctions.Truncate(location.Longitude.Value, 6)
                 where latsToLoad.Contains(latitude)
                 where lngsToLoad.Contains(longitude)
                 select location).ToArray();

            return associatedLocations.ToArray();
        }


        /// <summary>
        /// Loads the Region associations from a set of rows. If there is not a region for a name it will create a new one.
        /// It uses the DataCategory.RegionName to find/create the regions.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="serviceProvider">The service provider (in case this needs to create new regions).</param>
        /// <param name="rows">The import rows"</param>
        /// <returns></returns>
        private Region[] LoadCreateRegionAssociations(Guid roleId, BusinessAccount serviceProvider, IEnumerable<ImportRow> rows)
        {
            //Get the distinct region names from the rows' DataCategory.RegionName values
            var regionNamesToLoad = rows.Select(cvs =>
            {
                var regionNameCategoryValue = cvs.FirstOrDefault(cv => cv.DataCategory == DataCategory.RegionName);
                return regionNameCategoryValue == null ? null : regionNameCategoryValue.Value;
            }).Distinct().Where(cn => cn != null).ToArray();

            var loadedRegionsFromName =
                (from region in GetRegionsForServiceProvider(roleId)
                 where regionNamesToLoad.Contains(region.Name)
                 select region).ToArray();

            //Create a new region for any region name that did not exist
            var newRegions = regionNamesToLoad.Except(loadedRegionsFromName.Select(r => r.Name)).Select(newRegionName =>
                new Region { Id = Guid.NewGuid(), BusinessAccount = serviceProvider, Name = newRegionName });

            return loadedRegionsFromName.Union(newRegions).ToArray();
        }
    }
}
