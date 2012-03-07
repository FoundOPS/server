using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.Import;
using FoundOps.Server.Authentication;
using Kent.Boogaart.KBCsv;
using Microsoft.Samples.EntityDataReader;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for importing entities.
    /// </summary>
    public partial class CoreDomainService
    {
        public bool ImportEntities(Guid currentRoleId, ImportDestination importDestination, byte[] dataCSV)
        {
            var business = ObjectContext.BusinessAccountForRole(currentRoleId);
            if (business == null)
                throw new AuthenticationException("Invalid attempted access logged for investigation.");

            var clientsToAdd = new List<Client>();
            var locationsToAdd = new List<Location>();

            //Tuple contains the Client and the name of the Location to associate
            var clientLocationAssociationsToHookup = new List<Tuple<Client, string>>();

            //Tuple contains the Location and the name of the Client to associate
            var locationClientAssociationsToHookup = new List<Tuple<Location, string>>();

            using (var csv = new CsvReader(new MemoryStream(dataCSV)))
            {
                var headerRecord = csv.ReadHeaderRecord();
                var importColumnTypes = headerRecord.Values.Select(v => (DataCategory)Enum.Parse(typeof(DataCategory), v)).ToArray();

                foreach (var record in csv.DataRecords)
                {
                    #region Empty Entity Property Values

                    //Contact info properties (for Clients and Locations)
                    string contactInfoEmailAddressLabel = null;
                    string contactInfoEmailAddressData = null;
                    string contactInfoFaxNumberLabel = null;
                    string contactInfoFaxNumberData = null;
                    string contactInfoPhoneNumberLabel = null;
                    string contactInfoPhoneNumberData = null;
                    string contactInfoOtherLabel = null;
                    string contactInfoOtherData = null;
                    string contactInfoWebsiteLabel = null;
                    string contactInfoWebsiteData = null;

                    //Client properties
                    string clientName = null;

                    //Location properties
                    string locationName = null;

                    decimal? locationLatitude = null;
                    decimal? locationLongitude = null;
                    string locationAddressLineOne = null;
                    string locationAddressLineTwo = null;
                    string locationCity = null;
                    string locationState = null;
                    string locationZipCode = null;

                    #endregion

                    //Go through each column and set the values
                    int columnIndex = 0;
                    foreach (var value in record.Values)
                    {
                        var importType = importColumnTypes.ElementAt(columnIndex);
                        columnIndex++;

                        #region Contact Info Columns

                        if (importType == DataCategory.ContactInfoEmailAddressLabel)
                            contactInfoEmailAddressLabel = value;

                        if (importType == DataCategory.ContactInfoEmailAddressData)
                            contactInfoEmailAddressData = value;

                        if (importType == DataCategory.ContactInfoFaxNumberLabel)
                            contactInfoFaxNumberLabel = value;

                        if (importType == DataCategory.ContactInfoFaxNumberData)
                            contactInfoFaxNumberData = value;

                        if (importType == DataCategory.ContactInfoPhoneNumberLabel)
                            contactInfoPhoneNumberLabel = value;

                        if (importType == DataCategory.ContactInfoPhoneNumberData)
                            contactInfoPhoneNumberData = value;

                        if (importType == DataCategory.ContactInfoOtherLabel)
                            contactInfoOtherLabel = value;

                        if (importType == DataCategory.ContactInfoOtherData)
                            contactInfoOtherData = value;

                        if (importType == DataCategory.ContactInfoWebsiteLabel)
                            contactInfoWebsiteLabel = value;

                        if (importType == DataCategory.ContactInfoWebsiteData)
                            contactInfoWebsiteData = value;

                        #endregion

                        #region Client Columns

                        if (importType == DataCategory.ClientName)
                            clientName = value;

                        #endregion

                        #region Location Columns

                        if (importType == DataCategory.LocationName)
                            locationName = value;

                        if (importType == DataCategory.LocationLatitude)
                        {
                            decimal parsedDecimal;
                            if (Decimal.TryParse(value, out parsedDecimal))
                                locationLatitude = parsedDecimal;
                        }

                        if (importType == DataCategory.LocationLongitude)
                        {
                            decimal parsedDecimal;
                            if (Decimal.TryParse(value, out parsedDecimal))
                                locationLongitude = parsedDecimal;
                        }

                        if (importType == DataCategory.LocationAddressLineOne)
                            locationAddressLineOne = value;

                        if (importType == DataCategory.LocationAddressLineTwo)
                            locationAddressLineTwo = value;

                        if (importType == DataCategory.LocationCity)
                            locationCity = value;

                        if (importType == DataCategory.LocationState)
                            locationState = value;

                        if (importType == DataCategory.LocationZipCode)
                            locationZipCode = value;

                        #endregion
                    }

                    //Setup the newEntity

                    EntityObject newEntity = null;

                    #region Add the Client entity
                   
                    if (importDestination == ImportDestination.Clients)
                    {
                        var client = new Client
                        {
                            Vendor = business,
                            OwnedParty = new Business { Name = clientName }
                        };

                        //Add the location association to the hookup queue
                        if (!String.IsNullOrEmpty(locationName))
                            clientLocationAssociationsToHookup.Add(new Tuple<Client, string>(client, locationName));

                        this.ObjectContext.Clients.AddObject(client);

                        newEntity = client;
                    }

                    #endregion
                    #region Add the Location Entity

                    //Add the Location entity
                    if (importDestination == ImportDestination.Locations)
                    {
                        var location = new Location
                        {
                            OwnerParty = business,
                            Name = locationName,
                            Latitude = locationLatitude,
                            Longitude = locationLongitude,
                            AddressLineOne = locationAddressLineOne,
                            AddressLineTwo = locationAddressLineTwo,
                            City = locationCity,
                            State = locationState,
                            ZipCode = locationZipCode
                        };

                        //Add the client association to the hookup queue
                        if (!String.IsNullOrEmpty(clientName))
                            locationClientAssociationsToHookup.Add(new Tuple<Location, string>(location, clientName));

                        this.ObjectContext.Locations.AddObject(location);

                        newEntity = location;
                    }

                    #endregion

                    #region Add contact info to entities

                    var contactInfoSet = new List<ContactInfo>();

                    if (!String.IsNullOrEmpty(contactInfoEmailAddressLabel) || !String.IsNullOrEmpty(contactInfoEmailAddressData))
                        contactInfoSet.Add(new ContactInfo { Type = "Email Address", Label = contactInfoEmailAddressLabel ?? "", Data = contactInfoEmailAddressData ?? "" });

                    if (!String.IsNullOrEmpty(contactInfoFaxNumberLabel) || !String.IsNullOrEmpty(contactInfoFaxNumberData))
                        contactInfoSet.Add(new ContactInfo { Type = "Fax Number", Label = contactInfoFaxNumberLabel ?? "", Data = contactInfoFaxNumberData ?? "" });

                    if (!String.IsNullOrEmpty(contactInfoPhoneNumberLabel) || !String.IsNullOrEmpty(contactInfoPhoneNumberData))
                        contactInfoSet.Add(new ContactInfo { Type = "Phone Number", Label = contactInfoPhoneNumberLabel ?? "", Data = contactInfoPhoneNumberData ?? "" });

                    if (!String.IsNullOrEmpty(contactInfoOtherLabel) || !String.IsNullOrEmpty(contactInfoOtherData))
                        contactInfoSet.Add(new ContactInfo { Type = "Other", Label = contactInfoOtherLabel ?? "", Data = contactInfoOtherData ?? "" });

                    if (!String.IsNullOrEmpty(contactInfoWebsiteLabel) || !String.IsNullOrEmpty(contactInfoWebsiteData))
                        contactInfoSet.Add(new ContactInfo { Type = "Website", Label = contactInfoWebsiteLabel ?? "", Data = contactInfoWebsiteData ?? "" });

                    if (newEntity is Location)
                    {
                        foreach (var contactInfo in contactInfoSet)
                            ((Location)newEntity).ContactInfoSet.Add(contactInfo);
                    }
                    else if (newEntity is Client)
                    {
                        foreach (var contactInfo in contactInfoSet)
                            ((Client)newEntity).OwnedParty.ContactInfoSet.Add(contactInfo);
                    }

                    #endregion
                }
            }


            #region Hookup remaining assocations

            //Hookup Locations to Clients
            if (importDestination == ImportDestination.Clients)
            {
                var locationNamesToLoad = clientLocationAssociationsToHookup.Select(cl => cl.Item2).Distinct();
               
                //Load all the associatedLocations
                var associatedLocations = (from location in this.ObjectContext.Locations.Where(l => l.OwnerPartyId == business.Id)
                                           join locationNameToLoad in locationNamesToLoad
                                           on location.Name equals locationNameToLoad
                                           select location).ToArray();

                foreach (var clientLocationAssociation in clientLocationAssociationsToHookup)
                {
                    var associatedLocation = associatedLocations.FirstOrDefault(l => l.Name == clientLocationAssociation.Item2);
                    
                    //If the associatedLocation was found set it's PartyId to this Client's (OwnerParty) Id
                    if (associatedLocation != null)
                        associatedLocation.PartyId = clientLocationAssociation.Item1.Id;
                }
            }
            //Hookup Clients to Locations
            else if (importDestination == ImportDestination.Locations)
            {
                var clientNamesToLoad = locationClientAssociationsToHookup.Select(cl => cl.Item2).Distinct();

                //Load all the associatedClients
                var associatedClients =
                    (from client in ObjectContext.Clients.Where(c => c.VendorId == business.Id)
                     //Need to get the client displayname, so join with the person and the business table
                     join p in ObjectContext.Parties.OfType<Person>()
                         on client.Id equals p.Id into personClient
                     from personParty in personClient.DefaultIfEmpty()
                     //Left Join
                     join b in ObjectContext.Parties.OfType<Business>()
                         on client.Id equals b.Id into businessClient
                     from businessParty in businessClient.DefaultIfEmpty()
                     //Left Join
                     let displayName = businessParty != null
                                           ? businessParty.Name
                                           : personParty.LastName + " " + personParty.FirstName + " " +
                                             personParty.MiddleInitial
                     where clientNamesToLoad.Contains(displayName) 
                     select new {displayName, client}).ToArray();

                foreach (var locationClientAssociation in locationClientAssociationsToHookup)
                {
                    var associatedClient =
                        associatedClients.FirstOrDefault(c => c.displayName == locationClientAssociation.Item2);

                    //If the associatedClient was found set the Location's PartyId to this Client's (OwnerParty) Id
                    if (associatedClient != null)
                        locationClientAssociation.Item1.PartyId = associatedClient.client.Id;
                }
            }

            #endregion

            this.ObjectContext.SaveChanges();

            //TODO: Make bulk copy work
            //var con = new SqlConnection(CoreEntitiesServerManagement.SqlConnectionString);
            //con.Open();

            //using (var tran = con.BeginTransaction())
            //{
            //    #region Add Clients

            //    var clientsBulkCopy = new SqlBulkCopy(con,
            //                             SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers |
            //                             SqlBulkCopyOptions.KeepNulls, tran) { BatchSize = 1000, DestinationTableName = "Clients" };
            //    clientsBulkCopy.WriteToServer(clientsToAdd.AsDataReader());

            //    #endregion


            //    #region Add Locations

            //    var locationsBulkCopy = new SqlBulkCopy(con,
            //                             SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers |
            //                             SqlBulkCopyOptions.KeepNulls, tran) { BatchSize = 1000, DestinationTableName = "Locations" };

            //    locationsBulkCopy.WriteToServer(locationsToAdd.AsDataReader());

            //    #endregion

            //    #region Add Contact Info

            //    var contactInfoBulkCopy = new SqlBulkCopy(con,
            //                            SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers |
            //                            SqlBulkCopyOptions.KeepNulls, tran) { BatchSize = 1000, DestinationTableName = "Clients" };
            //    contactInfoBulkCopy.WriteToServer(clientsToAdd.SelectMany(c => c.OwnedParty.ContactInfoSet).Union(
            //            locationsToAdd.SelectMany(l => l.ContactInfoSet)).AsDataReader());

            //    #endregion

            //    tran.Commit();
            //    con.Close();
            //}

            return true;
        }
    }
}