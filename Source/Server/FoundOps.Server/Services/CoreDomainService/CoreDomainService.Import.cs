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

            var locationsToAdd = new List<Location>();
            var clientsToAdd = new List<Client>();

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

                        //TODO Columns with possible associations

                        EntityObject newEntity = null;
                        //Add new entity
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

                            //TODO
                            //if (selectedClient != null)
                            //    location.Party = selectedClient.OwnedParty;

                            newEntity = location;
                        }

                        if (importDestination == ImportDestination.Clients)
                        {
                            var client = new Client
                            {
                                Vendor = business,
                                OwnedParty = new Business { Name = clientName }
                            };

                            //TODO
                            //if (selectedLocation != null)
                            //    client.OwnedParty.Locations.Add(selectedLocation);

                            newEntity = client;
                        }

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

                            locationsToAdd.Add((Location)newEntity);
                        }
                        else if (newEntity is Client)
                        {
                            foreach (var contactInfo in contactInfoSet)
                                ((Client)newEntity).OwnedParty.ContactInfoSet.Add(contactInfo);

                            clientsToAdd.Add((Client)newEntity);
                        }

                        #endregion
                    }
                }
            }

            foreach(var clientToAdd in clientsToAdd)
                this.ObjectContext.Clients.AddObject(clientToAdd);

            foreach (var locationToAdd in locationsToAdd)
                this.ObjectContext.Locations.AddObject(locationToAdd);

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