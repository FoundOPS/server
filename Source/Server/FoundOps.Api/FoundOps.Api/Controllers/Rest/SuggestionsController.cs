using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Client = FoundOps.Core.Models.CoreEntities.Client;
using ContactInfo = FoundOps.Api.Models.ContactInfo;
using Location = FoundOps.Core.Models.CoreEntities.Location;
using Region = FoundOps.Api.Models.Region;
using Repeat = FoundOps.Api.Models.Repeat;

namespace FoundOps.Api.Controllers.Rest
{
    public class SuggestionsRequest
    {
        public List<string[]> RowsWithHeaders { get; set; }
        public ImportRow[] Rows { get; set; }
    }

    public class SuggestionsController : BaseApiController
    {
        private Client[] _clients;
        private Location[] _locations;
        private FoundOps.Core.Models.CoreEntities.Region[] _regions;
        private const string Sql = @"SELECT * FROM dbo.Locations WHERE BusinessAccountId = @id
SELECT * FROM dbo.Clients WHERE BusinessAccountId = @id
SELECT * FROM dbo.Regions WHERE BusinessAccountId = @id";

        /// <summary>
        /// This just call the appropriate function based on the inputs
        /// </summary>
        /// <param name="roleId">The current role id</param>
        /// <param name="rowsWithHeaders">If not null, call ValidateInput. The first string[] is the headers. The rest are data to be imported</param>
        /// <param name="rows">If not null, call SuggestEntites. The rows to be imported</param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions Put(Guid roleId, SuggestionsRequest request)
        {
            //If both or neither optional input are null, throw a bad request
            if ((request.RowsWithHeaders == null && request.Rows == null) || (request.RowsWithHeaders != null && request.Rows != null))
                throw Request.BadRequest();

            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.BadRequest();

            //Call the appropriate function and return the Suggestions
            return request.RowsWithHeaders != null ? 
                this.ValidateInput(request.RowsWithHeaders, businessAccount) :
                this.SuggestEntites(request.Rows, businessAccount);
        }

        /// <summary>
        /// Manipulates the input strings to either existing or new entities.
        /// Passes those entities to SuggestEntities to generate the suggestions.
        /// This method is only used for the initial step of the Import. Once entites are generated; use SuggestEntities
        /// </summary>
        /// <param name="rowsWithHeaders">List of string[]. The first string[] is the headers. the rest are data to be imported</param>
        /// <param name="businessAccount">The business account </param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions ValidateInput(List<string[]> rowsWithHeaders, FoundOps.Core.Models.CoreEntities.BusinessAccount businessAccount)
        {
            var headers = rowsWithHeaders[0];
            rowsWithHeaders.RemoveAt(0);
            var rows = rowsWithHeaders;

            SetupClientLocationRegionSets(businessAccount.Id);

            #region Assign Column placement

            var clientNameCol = Array.IndexOf(headers, "Client Name");
            var addressLineOneCol = Array.IndexOf(headers, "Address Line One");
            var addressLineTwoCol = Array.IndexOf(headers, "Address Line Two");
            var cityCol = Array.IndexOf(headers, "City");
            var stateCol = Array.IndexOf(headers, "State");
            var zipCodeCol = Array.IndexOf(headers, "Zipcode");
            var countryCodeCol = Array.IndexOf(headers, "Country Code");
            var regionNameCol = Array.IndexOf(headers, "Region Name");
            var latitudeCol = Array.IndexOf(headers, "Latitude");
            var longitudeCol = Array.IndexOf(headers, "Longitude");
            var frequencyCol = Array.IndexOf(headers, "Frequency");
            var repeatEveryCol = Array.IndexOf(headers, "Repeat Every");
            var startDateCol = Array.IndexOf(headers, "Start Date");
            var endDateCol = Array.IndexOf(headers, "End Date");
            var endAfterCol = Array.IndexOf(headers, "End After Times");
            var frequencyDetailCol = Array.IndexOf(headers, "Frequency Detail");

            #endregion

            var importRows = new List<ImportRow>();

            var rowCount = rows.Count();
            Parallel.For((long)0, rowCount, rowIndex =>
            {
                var row = rows[(int)rowIndex];

                var importRow = new ImportRow();

                #region Location

                var importedLocation = new Api.Models.Location();

                //Checks to be sure that all columns needed to make a location exist in the headers passed
                if (new[] { addressLineOneCol, addressLineTwoCol, cityCol, stateCol, countryCodeCol, zipCodeCol, latitudeCol, longitudeCol }.Any(col => col != -1))
                {
                    var latitude = row[latitudeCol];
                    var longitude = row[longitudeCol];
                    var addressLineOne = row[addressLineOneCol];
                    var addressLineTwo = row[addressLineTwoCol];
                    var city = row[cityCol];
                    var state = row[stateCol];
                    var zipCode = row[zipCodeCol];
                    var regionName = regionNameCol != -1 ? row[regionNameCol] : null;

                    //If Lat/Lon dont have values, try to GeoCode
                    if (latitude == "" && longitude == "")
                    {
                        GeocoderResult geocodeResult;

                        var address = new Address
                        {
                                AddressLineOne = addressLineOne ?? null,
                                City = city ?? null,
                                State = state ?? null,
                                ZipCode = zipCode ?? null
                        };

                        try
                        {
                            geocodeResult = BingLocationServices.TryGeocode(address).FirstOrDefault();
                        }
                        catch (Exception)
                        {
                            geocodeResult = BingLocationServices.TryGeocode(address).FirstOrDefault();
                        }

                        if (geocodeResult != null)
                        {
                            latitude = geocodeResult.Latitude;
                            longitude = geocodeResult.Longitude;
                        }

                        //If they still do not have values, throw error on all entries in the row.
                        if (latitude == null && longitude == null)
                            importedLocation.StatusInt = (int)ImportStatus.Error;

                        var clientName = row[clientNameCol];

                        //Matched the address entered and client name matched to a location
                        var matchedLocation = _locations.FirstOrDefault(l => clientName != null && l.ClientId != null &&
                               (addressLineTwo != null && (l.AddressLineOne == addressLineOne && l.AddressLineTwo == addressLineTwo
                                    && _clients.First(c => c.Id == l.ClientId).Name == clientName)));

                        if (matchedLocation != null)
                            importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation, regionName, ImportStatus.Linked);
                    }

                    //If lat/long exist, try and match based on that and 
                    if (latitude != "" && longitude != "")
                    {
                        var roundedLatitude = Math.Round(Convert.ToDecimal(latitude), 6);
                        var roundedLongitude = Math.Round(Convert.ToDecimal(longitude), 6);

                        //Try and match a location to one in the FoundOPS system
                        var matchedLocation = addressLineTwo != null
                            //Use this statement if Address Line Two is not null
                        ? _locations.FirstOrDefault(l => l.AddressLineTwo == addressLineTwo && (l.Latitude != null && l.Longitude != null)
                            && (decimal.Round(l.Latitude.Value, 6) == roundedLatitude
                            && decimal.Round(l.Longitude.Value, 6) == roundedLongitude))
                            //Use this statement if Address Line Two is null
                        : _locations.FirstOrDefault(l => (l.Latitude != null && l.Longitude != null)
                            && (decimal.Round(l.Latitude.Value, 6) == roundedLatitude
                            && decimal.Round(l.Longitude.Value, 6) == roundedLongitude));

                        //If a match is found, assign Linked status to all cells
                        if (matchedLocation != null)
                            importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation, regionName, ImportStatus.Linked);
                    }

                    //If no linked location is found, it means that the location is new
                    if (importedLocation.StatusInt != (int)ImportStatus.Linked)
                    {
                        importedLocation = new Api.Models.Location
                        {
                            Id = Guid.NewGuid(),
                            AddressLineOne = addressLineOne,
                            AddressLineTwo = addressLineTwo ?? null,
                            City = city,
                            State = state,
                            ZipCode = zipCode,
                            CountryCode = "US",
                            ContactInfoSet = new List<ContactInfo>(),
                            Latitude = latitude,
                            Longitude = longitude,
                            StatusInt = (int)ImportStatus.New,
                            Region = SetRegion(regionName)
                        };
                    }
                }

                importRow.Location = importedLocation;

                #endregion

                #region Client

                if (!string.IsNullOrEmpty(row[clientNameCol]))
                {
                    var existingClient = _clients.FirstOrDefault(c => c.Name == row[clientNameCol]);

                    Models.Client importedClient;

                    if (existingClient != null)
                    {
                        importedClient = FoundOps.Api.Models.Client.ConvertModel(existingClient);
                        importedClient.StatusInt = (int)ImportStatus.Linked;
                    }
                    else
                    {
                        importedClient = new FoundOps.Api.Models.Client
                        {
                            Id = Guid.NewGuid(),
                            Name = row[clientNameCol],
                            ContactInfoSet = new List<ContactInfo>(),
                            StatusInt = (int)ImportStatus.New
                        };
                    }

                    importRow.Client = importedClient;
                }

                #endregion

                #region Repeat

                //Note: If no start date is passed, set it to today
                var repeat = new Repeat
                {
                    Id = Guid.NewGuid(),
                    StartDate = startDateCol == -1 || row[startDateCol] == "" ? Convert.ToDateTime(row[startDateCol]) : CoreEntitiesContainer.CurrentUserAccount().First().Now(),
                    EndDate = endDateCol != -1 && row[endDateCol] != "" ? Convert.ToDateTime(row[endDateCol]) : (DateTime?)null,
                    EndAfterTimes = endAfterCol != -1 && row[endAfterCol] != "" ? Convert.ToInt32(row[endAfterCol]) : (int?)null,
                    RepeatEveryTimes = repeatEveryCol != -1 && row[repeatEveryCol] != "" ? Convert.ToInt32(row[repeatEveryCol]) : (int?)null
                };

                #region Frequency

                var val = frequencyCol != -1 ? row[frequencyCol].ToLower() : "";
                switch (val)
                {
                    case "o":
                    case "once":
                        repeat.FrequencyInt = (int)Frequency.Once;
                        break;
                    case "d":
                    case "daily":
                        repeat.FrequencyInt = (int)Frequency.Daily;
                        break;
                    case "w":
                    case "weekly":
                        repeat.FrequencyInt = (int)Frequency.Weekly;
                        break;
                    case "m":
                    case "monthly":
                        repeat.FrequencyInt = (int)Frequency.Monthly;
                        break;
                    case "y":
                    case "yearly":
                        repeat.FrequencyInt = (int)Frequency.Yearly;
                        break;
                    default:
                        repeat.FrequencyInt = null;
                        break;
                }

                #endregion

                #region Frequency Detail

                val = frequencyDetailCol != -1 ? row[frequencyDetailCol].ToLower() : "";
                val = val.Replace(" ", "");
                if (repeat.Frequency == Frequency.Weekly)
                {
                    var startDayOfWeek = repeat.StartDate.DayOfWeek;

                    //If it is empty assume the Start Date
                    if (string.IsNullOrEmpty(val))
                        repeat.FrequencyDetailAsWeeklyFrequencyDetail = new[] { startDayOfWeek };
                    else
                    {
                        var dayStrings = val.Split(',');
                        var daysOfWeek = new List<DayOfWeek>();

                        if (dayStrings.Any(s => s == "s" || s == "su" || s == "sun" || s == "sunday"))
                            daysOfWeek.Add(DayOfWeek.Sunday);

                        if (dayStrings.Any(s => s == "m" || s == "mo" || s == "mon" || s == "monday"))
                            daysOfWeek.Add(DayOfWeek.Monday);

                        if (dayStrings.Any(s => s == "t" || s == "tu" || s == "tue" || s == "tues" || s == "tuesday"))
                            daysOfWeek.Add(DayOfWeek.Tuesday);

                        if (dayStrings.Any(s => s == "w" || s == "we" || s == "wed" || s == "wednesday"))
                            daysOfWeek.Add(DayOfWeek.Wednesday);

                        if (dayStrings.Any(s => s == "r" || s == "th" || s == "tr" || s == "thur" || s == "thurs" || s == "thursday"))
                            daysOfWeek.Add(DayOfWeek.Thursday);

                        if (dayStrings.Any(s => s == "f" || s == "fr" || s == "fri" || s == "friday"))
                            daysOfWeek.Add(DayOfWeek.Friday);

                        if (dayStrings.Any(s => s == "s" || s == "sa" || s == "sat" || s == "saturday"))
                            daysOfWeek.Add(DayOfWeek.Saturday);

                        //Make sure the days include the startdate
                        if (!daysOfWeek.Contains(startDayOfWeek))
                            daysOfWeek.Add(startDayOfWeek);

                        repeat.FrequencyDetailAsWeeklyFrequencyDetail = daysOfWeek.OrderBy(e => (int)e).ToArray();
                    }
                }

                if (repeat.Frequency == Frequency.Monthly)
                {
                    if (string.IsNullOrEmpty(val) || val == "date")
                    {
                        repeat.FrequencyDetailAsMonthlyFrequencyDetail = MonthlyFrequencyDetail.OnDayInMonth;
                    }
                    else if (val == "day")
                    {
                        var detailsAvailable = repeat.AvailableMonthlyFrequencyDetailTypes.ToList();
                        if (detailsAvailable.Count() > 1)
                            detailsAvailable.Remove(MonthlyFrequencyDetail.OnDayInMonth);
                        repeat.FrequencyDetailAsMonthlyFrequencyDetail = detailsAvailable.First();
                    }
                }

                #endregion

                repeat.StatusInt = repeat.EndDate == null || repeat.EndAfterTimes == null ||
                                   repeat.RepeatEveryTimes == null || repeat.FrequencyInt == null
                                       ? (int) ImportStatus.Error
                                       : (int) ImportStatus.New;

                importRow.Repeat = repeat;

                #endregion

                importRows.Add(importRow);
            });

            var suggestion = SuggestEntites(importRows.ToArray(), businessAccount);

            return suggestion;
        }

        /// <summary>
        /// Suggests other potential options based on the entities passed
        /// </summary>
        /// <param name="rows">The rows being imported</param>
        /// <param name="businessAccount">The business account </param>
        /// <returns>Entites with suggestions</returns>
        public Suggestions SuggestEntites(ImportRow[] rows, FoundOps.Core.Models.CoreEntities.BusinessAccount businessAccount)
        {
            SetupClientLocationRegionSets(businessAccount.Id);

            var suggestionToReturn = new Suggestions();
            var clients = new List<FoundOps.Api.Models.Client>();
            var locations = new List<FoundOps.Api.Models.Location>();

            var rowCount = rows.Count();
            Parallel.For((long)0, rowCount, rowIndex =>
            {
                var row = rows[rowIndex];

                var rowSuggestions = new RowSuggestions();

                #region Location

                //Add the matched/new location as the first suggestion
                rowSuggestions.LocationSuggestions.Add(row.Location.Id);

                //Find all the Locations to be suggested by finding all Locations for the Client of the row
                var locationSuggestions = _locations.Where(l => l.ClientId == row.Client.Id || l.Id == row.Location.Id).ToArray();
                rowSuggestions.LocationSuggestions.AddRange(locationSuggestions.Select(l => l.Id));

                //Add all suggested Locations to the list of Locations to be returned
                locations.AddRange(locationSuggestions.Select(FoundOps.Api.Models.Location.ConvertModel));

                //If a new Location was created, add it to the list of location entites
                if (!_locations.Select(l => l.Id).Contains(row.Location.Id))
                    locations.Add(row.Location);

                #endregion

                #region Client

                //Add the matched/new client as the first suggestion
                rowSuggestions.ClientSuggestions.Add(row.Client.Id);

                //Find all the Clients to be suggested by finding all Clients for the Location of the row
                var clientSuggestions = _clients.Where(c => c.Id == row.Location.ClientId || c.Id == row.Client.Id).ToArray();
                rowSuggestions.ClientSuggestions.AddRange(clientSuggestions.Select(c => c.Id));

                //Add all suggested Clients to the list of Clients to be returned
                clients.AddRange(clientSuggestions.Select(FoundOps.Api.Models.Client.ConvertModel));

                //If a new Client was created, add it to the list of client entites
                if (!_clients.Select(c => c.Id).Contains(row.Client.Id))
                    clients.Add(row.Client);

                #endregion

                //Repeat
                rowSuggestions.Repeat.Add(row.Repeat);

                //Add this row's suggestions to the list to be returned
                suggestionToReturn.RowSuggestions.Add(rowSuggestions);
            });

            //Only add distinct Clients
            var distinctClients = clients.Distinct();
            suggestionToReturn.Clients.AddRange(distinctClients);

            //Only add distinct Locations
            var distinctLocations = locations.Distinct();
            suggestionToReturn.Locations.AddRange(distinctLocations);

            return suggestionToReturn;
        }

        #region Helpers

        /// <summary>
        /// Serves three functions. 
        /// 1) Convert the FoundOPS location to the API model.
        /// 2) Set the Region on the location if it exists.
        /// 3) Set the Status of the Location.
        /// </summary>
        /// <param name="matchedLocation"></param>
        /// <param name="regionName"></param>
        /// <param name="linked"></param>
        /// <returns></returns>
        private Models.Location ConvertLocationSetRegionAndStatus(Location matchedLocation, string regionName, ImportStatus status)
        {
            var location = Api.Models.Location.ConvertModel(matchedLocation);

            location.Region = SetRegion(regionName);

            location.StatusInt = (int)status;

            return location;
        }

        /// <summary>
        /// Find the correct region(if it exists) for the location
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        private Region SetRegion(string regionName)
        {
            if (regionName != null)
            {
                var region = _regions.FirstOrDefault(r => r.Name == regionName);
                if (region != null)
                    return Region.Convert(region);
            }

            return null;
        }

        /// <summary>
        /// Sets up the Client, Location and Regions lists used throughout the controller.
        /// Pulls full lists of entities for the business account so we only have to call the database once
        /// </summary>
        /// <param name="businessAccountId">The BusinessAccount's Id</param>
        private void SetupClientLocationRegionSets(Guid businessAccountId)
        {
            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                using (var data = conn.QueryMultiple(Sql, new { id = businessAccountId }))
                {
                    _locations = data.Read<Location>().ToArray();

                    _clients = data.Read<Client>().ToArray();

                    _regions = data.Read<FoundOps.Core.Models.CoreEntities.Region>().ToArray();
                }

                conn.Close();
            }
        }

        #endregion
    }
}
