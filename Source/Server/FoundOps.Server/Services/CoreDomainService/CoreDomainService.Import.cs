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
                var importColumnDataCategories = headerRecord.Values.Select(v => (DataCategory)Enum.Parse(typeof(DataCategory), v)).ToArray();

                //Setup a Tuple<IEnumerable<Category, string>> for each row (DataRecord)
                //This will be used by the CreateEntity method
                var rows = csv.DataRecords.Select(record => ImportRowTools.ExtractCategoriesWithValues(importColumnDataCategories, record)).ToArray();

                //Load the necessary associations
                
                //If the destination is Locations or RecurringServices
                //Load clients and child names
                Tuple<Client, string>[] clientAssociations = null;
                if (importDestination == ImportDestination.Locations || importDestination == ImportDestination.RecurringServices)
                    clientAssociations = LoadClients(businessAccount, rows);

                if (importDestination == ImportDestination.Clients)
                {
                    var availableServices = GetServiceProviderServiceTemplates(currentRoleId).ToArray();

                    foreach (var row in rows)
                    {
                        //TODO add Available Services
                        var newClient = ImportRowTools.CreateClient(businessAccount, row);

                        //Add the available services
                        foreach(var serviceTemplate in availableServices)
                            newClient.ServiceTemplates.Add(serviceTemplate.MakeChild(ServiceTemplateLevel.ClientDefined));

                        this.ObjectContext.Clients.AddObject(newClient);
                    }
                }
                else if (importDestination == ImportDestination.Locations)
                {
                    foreach(var row in rows)
                    {
                        var clientAssocation = GetClientAssociation(clientAssociations, row);
                        var newLocation = ImportRowTools.CreateLocation(businessAccount, row, clientAssocation);
                        this.ObjectContext.Locations.AddObject(newLocation);
                    }
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
        private static Client GetClientAssociation(IEnumerable<Tuple<Client, string>> clientAssociations, IEnumerable<Tuple<DataCategory, string>> row)
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
        /// Loads the clients and names from a set of rows.
        /// It uses the DataCategory.ClientName to find the client.
        /// </summary>
        /// <param name="businessAccount">The current business account</param>
        /// <param name="rows">The rows of Tuple&lt;DataCategory,string&gt;[]"</param>
        private Tuple<Client, string>[] LoadClients(BusinessAccount businessAccount, IEnumerable<Tuple<DataCategory, string>[]> rows)
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

            return associatedClients.Select(a => new Tuple<Client, string>(a.client, a.ChildName)).ToArray();
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
