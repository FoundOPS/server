using FoundOps.Core.Models.Import;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.SLClient.UI.Controls.ImportData;
using GalaSoft.MvvmLight.Command;
using Kent.Boogaart.KBCsv;
using MEFedMVVM.ViewModelLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;
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
                    case ImportDestination.RecurringServices:
                        return ImportDestinationTools.RecurringServicesColumnTypes;
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
        /// A command to try and geocode the current data.
        /// </summary>
        public RelayCommand TryGeocodeCommand { get; private set; }

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

            TryGeocodeCommand = new RelayCommand(TryGeocode);

            //When the ImportDataCommand is executed save the datatable to the database
            ImportDataCommand = new RelayCommand(SaveDataTableToDatabase);

            #endregion
        }

        #region Logic

        //TODO: Wait on http://www.telerik.com/community/forums/silverlight/gridview/lightweight-datatable-add-column-after-rows-have-been-added.aspx
        ///// <summary>
        ///// Adds a column to the DataTable.
        ///// </summary>
        ///// <param name="newColumnUniqueName">New name of the column unique.</param>
        //public ImportColumn AddColumn(string newColumnUniqueName)
        //{
        //    //Add the new column at the end
        //    var newColumn = new ImportColumn { ColumnName = newColumnUniqueName };
        //    DataTable.Columns.Add(newColumn);

        //    return newColumn;
        //}

        private static DataTable ReadInCSVData(CsvReader reader)
        {
            var dataTable = new DataTable();

            try
            {
                var headerRecord = reader.ReadHeaderRecord();
                foreach (var headerName in headerRecord.Values)
                    dataTable.Columns.Add(new ImportColumn { ColumnName = headerName, DataType = typeof(string) });

                //Add 3 extra columns
                for (var i = 0; i < 3; i++)
                    dataTable.Columns.Add(new ImportColumn { ColumnName = "Extra Column " + i, DataType = typeof(string) });

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

        private void TryGeocode()
        {
            if (ImportDestination != ImportDestination.Locations)
            {
                MessageBox.Show("This is only to be used when importing locations.");
                return;
            }

            MessageBox.Show("NOTE: All geocoding is limited to 50,000 locations per day");

            var selectedColumnDataCategories = DataTable.Columns.OfType<ImportColumn>().Where(ic => ic.ImportColumnType != null).Select(ic => new { ic.ColumnName, ic.ImportColumnType.Type });
            var addressLineOneColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationAddressLineOne);
            var cityColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationCity);
            var stateColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationState);
            var zipCodeColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationZipCode);
            var latitudeColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationLatitude);
            var longitudeColumn = selectedColumnDataCategories.FirstOrDefault(cdc => cdc.Type == DataCategory.LocationLongitude);

            //Make sure all the columns are selected
            if (addressLineOneColumn == null || cityColumn == null || stateColumn == null || zipCodeColumn == null || latitudeColumn == null || longitudeColumn == null)
            {
                MessageBox.Show("You need to select a column for: Address Line One, City, State, Zip Code, Latitude, and Longitude");
                return;
            }

            //Try to Geocode
            foreach (var row in this.DataTable.Rows)
            {
                var addressLineOne = (string)row[addressLineOneColumn.ColumnName];
                var city = (string)row[cityColumn.ColumnName];
                var state = (string)row[stateColumn.ColumnName];
                var zipCode = (string)row[zipCodeColumn.ColumnName];

                Manager.Data.TryGeocodeAddress(addressLineOne, city, state, zipCode, (geocodeComplete, userState) =>
                {
                    var rowToChange = (DataRow)userState;
                    if (geocodeComplete.Count() != 1) return;

                    var result = geocodeComplete.First();

                    //If it is not a good match return
                    if (result.Precision == "Low" || result.Precision == "Medium")
                        return;
                    if (result.Precision != "High" && Convert.ToInt32(result.Precision) < 85)
                        return;

                    //Clean the rows data with the result
                    rowToChange[addressLineOneColumn.ColumnName] = result.AddressLineOne;
                    rowToChange[cityColumn.ColumnName] = result.City;
                    rowToChange[stateColumn.ColumnName] = result.State;
                    rowToChange[zipCodeColumn.ColumnName] = result.ZipCode;
                    rowToChange[latitudeColumn.ColumnName] = result.Latitude;
                    rowToChange[longitudeColumn.ColumnName] = result.Longitude;
                }, row);
            }
        }

        private void SaveDataTableToDatabase()
        {
            //Convert the DataTable to CSV, and send it to be imported

            //Set this to busy until the operation is complete
            IsBusy = true;

            //Use the user selected ImportColumnTypes as the header record of the CSV
            var headerRecord = new List<string>();

            var columnIndexesToIgnore = new List<int>();
            var indexToIgnore = 0;
            foreach (var importColumn in DataTable.Columns.OfType<ImportColumn>())
            {
                if (importColumn.ImportColumnType != null)
                    headerRecord.Add(importColumn.ImportColumnType.Type.ToString());
                else //Ignore columns that do not have a ImportColumnType selected
                    columnIndexesToIgnore.Add(indexToIgnore);

                indexToIgnore++;
            }

            var csvToImport = DataTable.ToCSV(headerRecord, columnIndexesToIgnore);

            Manager.Data.DomainContext.ImportEntities(Manager.Context.RoleId, ImportDestination, csvToImport, invokeOp =>
            {
                if (!invokeOp.HasError)
                    MessageBox.Show("Data Imported");
                else
                {
                    MessageBox.Show(invokeOp.Error.Message);
                    Manager.Data.DomainContext.RejectChanges();
                }

                IsBusy = false;
            }, null);
        }

        #endregion
    }
}