using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.Import;
using FoundOps.Core.Tools;
using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
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

            using (var csv = new CsvReader(new MemoryStream(dataCSV)))
            {
                var headerRecord = csv.ReadHeaderRecord();
                var importColumnDataCategories = headerRecord.Values.Select(v => (DataCategory) Enum.Parse(typeof (DataCategory), v)).ToArray();

                //Setup a Tuple<IEnumerable<Category, string>> for each row (DataRecord)
                //This will be used by the CreateEntity method
                var rowsCategoriesValues =
                    csv.DataRecords.Select(record => ImportRowTools.ExtractCategoriesWithValues(importColumnDataCategories, record)).ToList();

                if (importDestination == ImportDestination.Clients)
                {
                    foreach (var newClient in rowsCategoriesValues.Select(rowCategoriesValues => ImportRowTools.CreateClient(businessAccount, rowCategoriesValues)))
                    {
                        this.ObjectContext.Clients.AddObject(newClient);
                        //TODO add available services
                    }
                }
                else if (importDestination == ImportDestination.Locations)
                {
                    //Load all necessary Client associations
                    var clientsToLoad = rowsCategoriesValues.SelectMany(cvs => cvs.First(cv => cv.Item1 == DataCategory.ClientName).Item2).Distinct();

                    //TODO set default billing location
                }
            }

            this.ObjectContext.SaveChanges();

            return true;
        }

        private void GetClientAssociations(IEnumerable<string> clientNames)
        {
            #region Hookup remaining assocations

            //Hookup Locations to Clients
            if (importDestination == ImportDestination.Clients)
            {
                var locationNamesToLoad = clientLocationAssociationsToHookup.Select(cl => cl.Item2).Distinct();

                //Load all the associatedLocations
                var associatedLocations = (from location in this.ObjectContext.Locations.Where(l => l.OwnerPartyId == businessAccount.Id)
                                           where locationNamesToLoad.Contains(location.Name)
                                           select location).ToArray();

                foreach (var clientLocationAssociation in clientLocationAssociationsToHookup)
                {
                    var associatedLocation = associatedLocations.FirstOrDefault(l => l.Name == clientLocationAssociation.Item2);

                    if (associatedLocation == null) continue;
                    //If the associatedLocation was found set it's PartyId to this Client's (OwnerParty) Id
                    //and set the Client's DefaultBillingLocation to the associatedLocation

                    associatedLocation.PartyId = clientLocationAssociation.Item1.Id;
                    clientLocationAssociation.Item1.DefaultBillingLocationId = clientLocationAssociation.Item1.Id;
                }
            }

            //Hookup Clients to Locations
            else if (importDestination == ImportDestination.Locations)
            {
                var clientNamesToLoad = locationClientAssociationsToHookup.Select(cl => cl.Item2).Distinct();

                //TODO: When importing people clients, fix the ChildName logic below (probably setup 2 left joins)
                //Load all the associatedClients
                var associatedClients =
                    (from client in ObjectContext.Clients.Where(c => c.VendorId == businessAccount.Id)
                     //Need to get the Clients names
                     join p in ObjectContext.PartiesWithNames
                         on client.Id equals p.Id
                     where clientNamesToLoad.Contains(p.ChildName)
                     select new { p.ChildName, client }).ToArray();

                foreach (var locationClientAssociation in locationClientAssociationsToHookup)
                {
                    var associatedClient =
                        associatedClients.FirstOrDefault(c => c.ChildName == locationClientAssociation.Item2);

                    if (associatedClient == null) continue;
                    //If the associatedClient was found set the Location's PartyId to this Client's (OwnerParty) Id
                    //and set the associatedClient's DefaultBillingLocation to the associatedLocation

                    locationClientAssociation.Item1.PartyId = associatedClient.client.Id;
                    associatedClient.client.DefaultBillingLocationId = locationClientAssociation.Item1.Id;
                }
            }

            #endregion

        }
    }
}