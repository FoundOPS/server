using System.IO;
using FoundOps.Core.Models.Import;
using Kent.Boogaart.KBCsv;

namespace FoundOps.Server.Services.CoreDomainService
{
    /// <summary>
    /// Holds the domain service operations for importing entities.
    /// </summary>
    public partial class CoreDomainService
    {
        public bool ImportEntities(ImportDestination importDestination, byte[] dataCSV)
        {
            using (var csv = new CsvReader(new MemoryStream(dataCSV)))
            {

            }

            //         var locationsToAdd = new List<Location>();
            //         var clientsToAdd = new List<Client>();

            //         #region Setup Entities to Add

            //         foreach (var row in DataTable.Rows)
            //         {
            //             Client selectedClient = null;
            //             string clientName = null;

            //             //string salespersonName = null;

            //             string contactInfoEmailAddressLabel = null;
            //             string contactInfoEmailAddressData = null;
            //             string contactInfoFaxNumberLabel = null;
            //             string contactInfoFaxNumberData = null;
            //             string contactInfoPhoneNumberLabel = null;
            //             string contactInfoPhoneNumberData = null;
            //             string contactInfoOtherLabel = null;
            //             string contactInfoOtherData = null;
            //             string contactInfoWebsiteLabel = null;
            //             string contactInfoWebsiteData = null;

            //             Location selectedLocation = null;
            //             string locationName = null;

            //             decimal? locationLatitude = null;
            //             decimal? locationLongitude = null;
            //             string locationAddressLineOne = null;
            //             string locationAddressLineTwo = null;
            //             string locationCity = null;
            //             string locationState = null;
            //             string locationZipCode = null;

            //             //DateTime? serviceDate = null;

            //             //Go through each selected column (those with a ImportColumnType !=null)
            //             foreach (var column in DataTable.Columns.OfType<ImportColumn>().Where(ic => ic.ImportColumnType != null))
            //             {
            //                 #region Columns with possible associations

            //                 if (column.ImportColumnType.Type == DataCategory.ClientName)
            //                 {
            //                     clientName = (string)row[column.ColumnName];
            //                     //TODO    
            //                     //selectedClient = Clients.FirstOrDefault(client => client.DisplayName == clientName);
            //                 }

            //                 if (column.ImportColumnType.Type == DataCategory.LocationName)
            //                 {
            //                     locationName = (string)row[column.ColumnName];
            //                     //TODO    
            //                     //selectedLocation = Locations.FirstOrDefault(location => location.Name == locationName);
            //                 }

            //                 #endregion

            //                 #region Contact Info Columns

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoEmailAddressLabel)
            //                     contactInfoEmailAddressLabel = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoEmailAddressData)
            //                     contactInfoEmailAddressData = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoFaxNumberLabel)
            //                     contactInfoFaxNumberLabel = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoFaxNumberData)
            //                     contactInfoFaxNumberData = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoPhoneNumberLabel)
            //                     contactInfoPhoneNumberLabel = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoPhoneNumberData)
            //                     contactInfoPhoneNumberData = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoOtherLabel)
            //                     contactInfoOtherLabel = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoOtherData)
            //                     contactInfoOtherData = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoWebsiteLabel)
            //                     contactInfoWebsiteLabel = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.ContactInfoWebsiteData)
            //                     contactInfoWebsiteData = (string)row[column.ColumnName];

            //                 #endregion

            //                 #region Location Columns

            //                 if (column.ImportColumnType.Type == DataCategory.LocationLatitude)
            //                 {
            //                     decimal parsedDecimal;
            //                     if (Decimal.TryParse((string)row[column.ColumnName], out parsedDecimal))
            //                         locationLatitude = parsedDecimal;
            //                 }

            //                 if (column.ImportColumnType.Type == DataCategory.LocationLongitude)
            //                 {
            //                     decimal parsedDecimal;
            //                     if (Decimal.TryParse((string)row[column.ColumnName], out parsedDecimal))
            //                         locationLongitude = parsedDecimal;
            //                 }

            //                 if (column.ImportColumnType.Type == DataCategory.LocationAddressLineOne)
            //                     locationAddressLineOne = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.LocationAddressLineTwo)
            //                     locationAddressLineTwo = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.LocationCity)
            //                     locationCity = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.LocationState)
            //                     locationState = (string)row[column.ColumnName];

            //                 if (column.ImportColumnType.Type == DataCategory.LocationZipCode)
            //                     locationZipCode = (string)row[column.ColumnName];

            //                 #endregion
            //             }

            //             Entity newEntity = null;
            //             //Add new entity
            //             if (ImportDestination == ImportDestination.Locations)
            //             {
            //                 var location = new Location
            //                 {
            //                     OwnerParty = Manager.Context.OwnerAccount,
            //                     Name = locationName,
            //                     Latitude = locationLatitude,
            //                     Longitude = locationLongitude,
            //                     AddressLineOne = locationAddressLineOne,
            //                     AddressLineTwo = locationAddressLineTwo,
            //                     City = locationCity,
            //                     State = locationState,
            //                     ZipCode = locationZipCode
            //                 };

            //                 if (selectedClient != null)
            //                     location.Party = selectedClient.OwnedParty;

            //                 newEntity = location;
            //             }

            //             if (ImportDestination == ImportDestination.Clients)
            //             {
            //                 var client = new Client
            //                 {
            //                     Vendor = (BusinessAccount)Manager.Context.OwnerAccount,
            //                     OwnedParty = new Business { Name = clientName }
            //                 };

            //                 if (selectedLocation != null)
            //                     client.OwnedParty.Locations.Add(selectedLocation);

            //                 newEntity = client;
            //             }

            //             //Add contact info
            //             var contactInfoSet = new List<ContactInfo>();

            //             if (!String.IsNullOrEmpty(contactInfoEmailAddressLabel) || !String.IsNullOrEmpty(contactInfoEmailAddressData))
            //                 contactInfoSet.Add(new ContactInfo { Type = "Email Address", Label = contactInfoEmailAddressLabel ?? "", Data = contactInfoEmailAddressData ?? "" });

            //             if (!String.IsNullOrEmpty(contactInfoFaxNumberLabel) || !String.IsNullOrEmpty(contactInfoFaxNumberData))
            //                 contactInfoSet.Add(new ContactInfo { Type = "Fax Number", Label = contactInfoFaxNumberLabel ?? "", Data = contactInfoFaxNumberData ?? "" });

            //             if (!String.IsNullOrEmpty(contactInfoPhoneNumberLabel) || !String.IsNullOrEmpty(contactInfoPhoneNumberData))
            //                 contactInfoSet.Add(new ContactInfo { Type = "Phone Number", Label = contactInfoPhoneNumberLabel ?? "", Data = contactInfoPhoneNumberData ?? "" });

            //             if (!String.IsNullOrEmpty(contactInfoOtherLabel) || !String.IsNullOrEmpty(contactInfoOtherData))
            //                 contactInfoSet.Add(new ContactInfo { Type = "Other", Label = contactInfoOtherLabel ?? "", Data = contactInfoOtherData ?? "" });

            //             if (!String.IsNullOrEmpty(contactInfoWebsiteLabel) || !String.IsNullOrEmpty(contactInfoWebsiteData))
            //                 contactInfoSet.Add(new ContactInfo { Type = "Website", Label = contactInfoWebsiteLabel ?? "", Data = contactInfoWebsiteData ?? "" });

            //             //Add contactinfo, and add entity to list of entities
            //             if (newEntity is Location)
            //             {
            //                 foreach (var contactInfo in contactInfoSet)
            //                     ((Location)newEntity).ContactInfoSet.Add(contactInfo);

            //                 locationsToAdd.Add((Location)newEntity);
            //             }

            //             if (newEntity is Client)
            //             {
            //                 foreach (var contactInfo in contactInfoSet)
            //                     ((Client)newEntity).OwnedParty.ContactInfoSet.Add(contactInfo);

            //                 clientsToAdd.Add((Client)newEntity);
            //             }
            //         }
            //         #endregion

            //         foreach (var location in locationsToAdd)
            //             Manager.Data.Context.Locations.Add(location);

            //         foreach (var client in clientsToAdd)
            //             Manager.Data.Context.Clients.Add(client);

            return true;
        }
    }
}