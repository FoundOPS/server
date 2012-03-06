using FoundOps.Common.Silverlight.Models.Import;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.ImportData;
using GalaSoft.MvvmLight.Command;
using Kent.Boogaart.KBCsv;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using Telerik.Data;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Import Data
    /// </summary>
    [ExportViewModel("ImportDataVM")]
    public class ImportDataVM : DataFedVM
    {
        #region Public Properties

        private bool _isBusy;
        /// <summary>
        /// Indicates the current viewmodel is importing data.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                this.RaisePropertyChanged("IsBusy");
            }
        }

        private ImportDestination _importDestination;
        /// <summary>
        /// The type of entities to.
        /// </summary>
        public ImportDestination ImportDestination
        {
            get { return _importDestination; }
            set
            {
                _importDestination = value;
                this.RaisePropertyChanged("ImportDestination");
                this.RaisePropertyChanged("DestinationColumnTypes");
            }
        }

        /// <summary>
        /// Gets the import destination column types.
        /// </summary>
        public IEnumerable<ImportColumnType> DestinationColumnTypes
        {
            get
            {
                switch (this.ImportDestination)
                {
                    case ImportDestination.Clients:
                        return ImportDestinationTools.ClientsColumnTypes;
                    case ImportDestination.Locations:
                        return ImportDestinationTools.LocationsColumnTypes;
                    case ImportDestination.Services:
                        return ImportDestinationTools.ServicesColumnTypes;
                    default:
                        return new List<ImportColumnType>();
                }
            }
        }

        private DataTable _dataTable;
        /// <summary>
        /// Gets the imported csv data table.
        /// </summary>
        public DataTable DataTable
        {
            get { return _dataTable; }
            private set
            {
                _dataTable = value;
                this.RaisePropertyChanged("DataTable");
            }
        }

        /// <summary>
        /// A command to choose a csv file to import.
        /// </summary>
        public RelayCommand<FileInfo> SelectFileCommand { get; private set; }
        /// <summary>
        /// A command to import the current data.
        /// </summary>
        public RelayCommand ImportDataCommand { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public ImportDataVM()
        {
            Manager.Data.Subscribe<Client>(DataManager.Query.Clients, ObservationState, null);
            Manager.Data.Subscribe<Location>(DataManager.Query.Locations, ObservationState, null);

            #region Setup Commands

            //When the SelectFileCommand is executed read the CSV data
            SelectFileCommand = new RelayCommand<FileInfo>(fileInfo =>
            {
                IsBusy = true;
                //use the CSVReader to read in the data
                using (var csv = new CsvReader(fileInfo.OpenRead()))
                    DataTable = ReadInCSVData(csv);
                IsBusy = false;
            });

            //When the ImportDataCommand is executed save the datatable to the database
            ImportDataCommand = new RelayCommand(SaveDataTableToDatabase);

            #endregion
        }

        #region Logic

        /// <summary>
        /// Adds a column to the DataTable.
        /// </summary>
        /// <param name="newColumnUniqueName">New name of the column unique.</param>
        public ImportColumn AddColumn(string newColumnUniqueName)
        {
            var newDataTable = new DataTable();

            //Copy the previous datatable

            //Add the new column
            var newColumn = new ImportColumn { ColumnName = newColumnUniqueName };
            DataTable.Columns.Add(newColumn);

            //Set the new DataTable            

            return newColumn;
        }

        private static DataTable ReadInCSVData(CsvReader reader)
        {
            var dataTable = new DataTable();

            try
            {
                var headerRecord = reader.ReadHeaderRecord();
                foreach (var headerName in headerRecord.Values)
                    dataTable.Columns.Add(new ImportColumn { ColumnName = headerName, DataType = typeof(string) });

                foreach (var record in reader.DataRecords)
                {
                    var newRow = dataTable.NewRow();
                    var headerIndex = 0;
                    foreach (var headerKey in headerRecord.Values)
                    {
                        var cleanedString = record.Values[headerIndex].Replace('�', ' ');
                        newRow[headerKey] = cleanedString;
                        headerIndex++;
                    }

                    dataTable.Rows.Add(newRow);
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("The data needs to be in CSV format.", "Error importing data.", MessageBoxButton.OK);
                return null;
            }

            return dataTable;
        }

        private void SaveDataTableToDatabase()
        {
            IsBusy = true;

            var locationsToAdd = new List<Location>();
            var clientsToAdd = new List<Client>();

            #region Setup Entities to Add

            foreach (var row in DataTable.Rows)
            {
                Client selectedClient = null;
                string clientName = null;

                //string salespersonName = null;

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

                Location selectedLocation = null;
                string locationName = null;

                decimal? locationLatitude = null;
                decimal? locationLongitude = null;
                string locationAddressLineOne = null;
                string locationAddressLineTwo = null;
                string locationCity = null;
                string locationState = null;
                string locationZipCode = null;

                //DateTime? serviceDate = null;

                //Go through each selected column (those with a ImportColumnType !=null)
                foreach (var column in DataTable.Columns.OfType<ImportColumn>().Where(ic => ic.ImportColumnType != null))
                {
                    #region Columns with possible associations

                    if (column.ImportColumnType.Type == ImportColumnEnum.ClientName)
                    {
                        clientName = (string)row[column.ColumnName];
                        //TODO    
                        //selectedClient = Clients.FirstOrDefault(client => client.DisplayName == clientName);
                    }

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationName)
                    {
                        locationName = (string)row[column.ColumnName];
                        //TODO    
                        //selectedLocation = Locations.FirstOrDefault(location => location.Name == locationName);
                    }

                    #endregion

                    #region Contact Info Columns

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoEmailAddressLabel)
                        contactInfoEmailAddressLabel = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoEmailAddressData)
                        contactInfoEmailAddressData = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoFaxNumberLabel)
                        contactInfoFaxNumberLabel = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoFaxNumberData)
                        contactInfoFaxNumberData = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoPhoneNumberLabel)
                        contactInfoPhoneNumberLabel = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoPhoneNumberData)
                        contactInfoPhoneNumberData = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoOtherLabel)
                        contactInfoOtherLabel = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoOtherData)
                        contactInfoOtherData = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoWebsiteLabel)
                        contactInfoWebsiteLabel = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.ContactInfoWebsiteData)
                        contactInfoWebsiteData = (string)row[column.ColumnName];

                    #endregion

                    #region Location Columns

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationLatitude)
                    {
                        decimal parsedDecimal;
                        if (Decimal.TryParse((string)row[column.ColumnName], out parsedDecimal))
                            locationLatitude = parsedDecimal;
                    }

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationLongitude)
                    {
                        decimal parsedDecimal;
                        if (Decimal.TryParse((string)row[column.ColumnName], out parsedDecimal))
                            locationLongitude = parsedDecimal;
                    }

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationAddressLineOne)
                        locationAddressLineOne = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationAddressLineTwo)
                        locationAddressLineTwo = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationCity)
                        locationCity = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationState)
                        locationState = (string)row[column.ColumnName];

                    if (column.ImportColumnType.Type == ImportColumnEnum.LocationZipCode)
                        locationZipCode = (string)row[column.ColumnName];

                    #endregion
                }

                Entity newEntity = null;
                //Add new entity
                if (ImportDestination == ImportDestination.Locations)
                {
                    var location = new Location
                    {
                        OwnerParty = Manager.Context.OwnerAccount,
                        Name = locationName,
                        Latitude = locationLatitude,
                        Longitude = locationLongitude,
                        AddressLineOne = locationAddressLineOne,
                        AddressLineTwo = locationAddressLineTwo,
                        City = locationCity,
                        State = locationState,
                        ZipCode = locationZipCode
                    };

                    if (selectedClient != null)
                        location.Party = selectedClient.OwnedParty;

                    newEntity = location;
                }

                if (ImportDestination == ImportDestination.Clients)
                {
                    var client = new Client
                    {
                        Vendor = (BusinessAccount)Manager.Context.OwnerAccount,
                        OwnedParty = new Business { Name = clientName }
                    };

                    if (selectedLocation != null)
                        client.OwnedParty.Locations.Add(selectedLocation);

                    newEntity = client;
                }

                //Add contact info
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

                //Add contactinfo, and add entity to list of entities
                if (newEntity is Location)
                {
                    foreach (var contactInfo in contactInfoSet)
                        ((Location)newEntity).ContactInfoSet.Add(contactInfo);

                    locationsToAdd.Add((Location)newEntity);
                }

                if (newEntity is Client)
                {
                    foreach (var contactInfo in contactInfoSet)
                        ((Client)newEntity).OwnedParty.ContactInfoSet.Add(contactInfo);

                    clientsToAdd.Add((Client)newEntity);
                }
            }
            #endregion

            foreach (var location in locationsToAdd)
                Manager.Data.Context.Locations.Add(location);

            foreach (var client in clientsToAdd)
                Manager.Data.Context.Clients.Add(client);

            Manager.Data.EnqueueSubmitOperation(onSaveCallback =>
            {
                if (!onSaveCallback.HasError)
                    MessageBox.Show("Data Imported");
                else
                {
                    MessageBox.Show("There was a problem with the last import, please try again.");
                    Manager.Data.Context.RejectChanges();
                }

                IsBusy = false;
            });
        }

        #endregion
    }
}