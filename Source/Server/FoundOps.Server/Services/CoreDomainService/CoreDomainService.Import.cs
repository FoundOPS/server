using System.Data.Entity;
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

            //If the destination is Locations
            //Load region associations
            Region[] regionAssociations = null;
            if (importDestination == ImportDestination.Locations)
                regionAssociations = LoadCreateRegionAssociations(currentRoleId, businessAccount, rows);

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
                    var regionAssociation = GetRegionAssociation(regionAssociations, row);
                    var newLocation = ImportRowTools.CreateLocation(businessAccount, row, clientAssocation, regionAssociation);
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
                    var locationAssociation = GetLocationAssociation(locationAssociations, row);
                    var newRecurringService = ImportRowTools.CreateRecurringService(businessAccount, row, clientServiceTemplate, clientAssocation, locationAssociation);

                    this.ObjectContext.RecurringServices.AddObject(newRecurringService);
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
        /// Gets the Location association from the loaded locationAssociations.
        /// It uses the DataCategory.LocationLatitude and DataCategory.LocationLongitude to find the location.
        /// </summary>
        /// <param name="locationAssociations">The loaded location associations.</param>
        /// <param name="row">The row's categories/values.</param>
        private static Location GetLocationAssociation(Location[] locationAssociations, Tuple<DataCategory, string>[] row)
        {
            var locationName = row.GetCategoryValue(DataCategory.LocationName);
            Location associatedLocation = null;

            var locationLatitude = row.GetCategoryValue(DataCategory.LocationLatitude);
            var locationLongitude = row.GetCategoryValue(DataCategory.LocationLongitude);
            if (locationLatitude != null && locationLongitude != null)
                associatedLocation =
                    locationAssociations.FirstOrDefault(
                        c =>
                        c.Latitude == Convert.ToDecimal(locationLatitude) &&
                        c.Longitude == Convert.ToDecimal(locationLongitude));

            if (associatedLocation == null && locationName != null)
                associatedLocation = locationAssociations.FirstOrDefault(l => l.Name == locationName);

            return associatedLocation;
        }

        /// <summary>
        /// Gets the Region association from the loaded regionAssociations and a row's Categories/Associations (if there is one).
        /// It used the DataCategory.RegionName to find the Region.
        /// </summary>
        /// <param name="regionAssociations">The loaded region associations</param>
        /// <param name="row">The row's categories/values.</param>
        private static Region GetRegionAssociation(Region[] regionAssociations, Tuple<DataCategory, string>[] row)
        {
            var regionName = row.GetCategoryValue(DataCategory.RegionName);
            if (string.IsNullOrEmpty(regionName))
                return null;

            var region = regionAssociations.First(ca => ca.Name == regionName);
            return region;
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
        /// It uses the DataCategory.LocationLatitude or the DataCategory.LocationLongitude to find the locations.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="rows">The rows of Tuple&lt;DataCategory,string&gt;[]"</param>
        /// <returns></returns>
        private Location[] LoadLocationAssociations(Guid roleId, Tuple<DataCategory, string>[][] rows)
        {
            //Get the distinct latitude/longitudes from the rows
            var latsToLoad = rows.Select(cvs =>
            {
                var latString = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.LocationLatitude);
                if (latString == null || string.IsNullOrEmpty(latString.Item2))
                    return null;
               return (decimal?) Convert.ToDecimal(latString.Item2);
            }).Distinct().Where(cn => cn != null).ToArray();

            var lngsToLoad = rows.Select(cvs =>
            {
                var lngString = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.LocationLongitude);
                if (lngString == null || string.IsNullOrEmpty(lngString.Item2))
                    return null;
                return (decimal?)Convert.ToDecimal(lngString.Item2);
            }).Distinct().Where(cn => cn != null).ToArray();

            var associatedLocations =
                (from location in GetLocationsToAdministerForRole(roleId)
                 where latsToLoad.Contains(location.Latitude)
                 where lngsToLoad.Contains(location.Longitude)
                 select location).ToArray();

            return associatedLocations.ToArray();
        }


        /// <summary>
        /// Loads the Region associations from a set of rows. If there is not a region for a name it will create a new one.
        /// It uses the DataCategory.RegionName to find/create the regions.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="serviceProvider">The service provider (in case this needs to create new regions).</param>
        /// <param name="rows">The rows of Tuple&lt;DataCategory,string&gt;[]"</param>
        /// <returns></returns>
        private Region[] LoadCreateRegionAssociations(Guid roleId, BusinessAccount serviceProvider, Tuple<DataCategory, string>[][] rows)
        {
            //Get the distinct region names from the rows' DataCategory.RegionName values
            var regionNamesToLoad = rows.Select(cvs =>
            {
                var regionNameCategoryValue = cvs.FirstOrDefault(cv => cv.Item1 == DataCategory.RegionName);
                return regionNameCategoryValue == null ? null : regionNameCategoryValue.Item2;
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
