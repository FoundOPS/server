using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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

namespace FoundOps.Api.Controllers.Mvc
{
    public class ImporterController : Controller
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        private Client[] _clients;
        private Location[] _locations;
        private FoundOps.Core.Models.CoreEntities.Region[] _regions;
        private const string Sql = @"SELECT * FROM dbo.Locations WHERE BusinessAccountId = @id
                                             SELECT * FROM dbo.Clients WHERE BusinessAccountId = @id
                                 SELECT * FROM dbo.Regions WHERE BusinessAccountId = @id";

        public ImporterController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        public Suggestions ValidateInput(Guid roleId, List<string[]> rowsWithHeaders)
        {
            var headers = rowsWithHeaders[0];
            rowsWithHeaders.RemoveAt(0);
            var rows = rowsWithHeaders;

            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                return null;

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
            var frequencyDetailCol = Array.IndexOf(headers, "FrequencyDetail");

            #endregion

            var importRows = new List<ImportRow>();

            var rowCount = rows.Count();
            Parallel.For((long)0, rowCount, rowIndex =>
                {
                    var row = rows[(int)rowIndex];

                    var importRow = new ImportRow();

                    #region Location

                    var importedLocation = new Api.Models.Location();

                    var latitude = row[latitudeCol];
                    var longitude = row[longitudeCol];
                    var addressLineOne = row[addressLineOneCol];
                    var addressLineTwo = row[addressLineTwoCol];
                    var city = row[cityCol];
                    var state = row[stateCol];
                    var zipCode = row[zipCodeCol];
                    var regionName = regionNameCol != -1 ? row[regionNameCol] : null;

                    //Checks to be sure that all columns needed to make a location exist in the headers passed
                    if (new[] { addressLineOneCol, addressLineTwoCol, cityCol, stateCol, countryCodeCol, zipCodeCol, latitudeCol, longitudeCol }.All(col => col != -1))
                    {
                        //If Lat/Lon dont have values, try to GeoCode
                        if (latitude == "" && longitude == "")
                        {
                            var geocodeResult = BingLocationServices.TryGeocode(new Address { AddressLineOne = addressLineOne, City = city, State = state, ZipCode = zipCode }).FirstOrDefault();

                            if (geocodeResult != null)
                            {
                                latitude = geocodeResult.Latitude;
                                longitude = geocodeResult.Longitude;
                            }

                            //If they still do not have values, throw error on all entries in the row.
                            if (latitude == null && longitude == null)
                                importedLocation.StatusInt = (int)ImportStatus.Error;

                            //Matched the address entered and client name matched to a location
                            var matchedLocation = _locations.FirstOrDefault(l => row[clientNameCol] != null && l.ClientId != null &&
                                   (addressLineTwo != null && (l.AddressLineOne == addressLineOne && l.AddressLineTwo == addressLineTwo
                                        && _clients.First(c => c.Id == l.ClientId).Name == row[clientNameCol])));

                            if (matchedLocation != null)
                                importedLocation = ConvertLocationSetRegionAndStatus(matchedLocation, regionName, ImportStatus.Linked);
                        }
                        //If lat/long exist, try and match based on that and 
                        else if (latitude != "" && longitude != "")
                        {
                            //Try and match a location to one in the FoundOPS system
                            var matchedLocation = addressLineTwo != null
                                //Use this statement if Address Line Two is not null
                            ? _locations.FirstOrDefault(l => l.AddressLineTwo == addressLineTwo && (l.Latitude != null && l.Longitude != null)
                                && (decimal.Round(l.Latitude.Value, 6) == decimal.Round(Convert.ToDecimal(latitude), 6)
                                && decimal.Round(l.Longitude.Value, 6) == decimal.Round(Convert.ToDecimal(longitude), 6)))
                                //Use this statement if Address Line Two is null
                            : _locations.FirstOrDefault(l => (l.Latitude != null && l.Longitude != null)
                                && (decimal.Round(l.Latitude.Value, 6) == decimal.Round(Convert.ToDecimal(latitude), 6)
                                && decimal.Round(l.Longitude.Value, 6) == decimal.Round(Convert.ToDecimal(longitude), 6)));

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

                    var clientName = row[clientNameCol];

                    if (!string.IsNullOrEmpty(clientName))
                    {
                        var existingClient = _clients.FirstOrDefault(c => c.Name == clientName);

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
                                Name = clientName,
                                ContactInfoSet = new List<ContactInfo>(),
                                StatusInt = (int)ImportStatus.New
                            };
                        }

                        importRow.Client = importedClient;
                    }

                    #endregion

                    #region Repeat

                    var repeat = new Repeat
                    {
                        Id = Guid.NewGuid(),
                        StartDate = Convert.ToDateTime(row[startDateCol]),
                        EndDate = Convert.ToDateTime(row[endDateCol]),
                        EndAfterTimes = Convert.ToInt32(row[endAfterCol]),
                        RepeatEveryTimes = Convert.ToInt32(row[repeatEveryCol])
                    };

                    #region Frequency

                    var val = row[frequencyCol].ToLower();
                    if (val == "once" || val == "o")
                        repeat.FrequencyInt = (int)Frequency.Once;
                    else if (val == "daily" || val == "d")
                        repeat.FrequencyInt = (int)Frequency.Daily;
                    else if (val == "weekly" || val == "w")
                        repeat.FrequencyInt = (int)Frequency.Weekly;
                    else if (val == "monthly" || val == "m")
                        repeat.FrequencyInt = (int)Frequency.Monthly;
                    else if (val == "yearly" || val == "y")
                        repeat.FrequencyInt = (int)Frequency.Yearly;

                    #endregion

                    #region Frequency Detail

                    val = row[frequencyDetailCol];
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

                            if (
                                dayStrings.Any(
                                    s =>
                                    s == "r" || s == "th" || s == "tr" || s == "thur" || s == "thurs" || s == "thursday"))
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

                    importRow.Repeat = repeat;

                    #endregion

                    importRows.Add(importRow);

                    #endregion
                });

            var suggestion = SuggestEntites(roleId, importRows.ToArray());

            return suggestion;
        }

        public Suggestions SuggestEntites(Guid roleId, ImportRow[] rows)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                return null;

            SetupClientLocationRegionSets(businessAccount.Id);

            var suggestionToReturn = new Suggestions();
            var clients = new List<FoundOps.Api.Models.Client>();
            var locations = new List<FoundOps.Api.Models.Location>();

            var rowCount = rows.Count();
            Parallel.For((long)0, rowCount, rowIndex =>
            {
                var row = rows[rowIndex];

                var rowSuggestions = new RowSuggestions();

                //Location
                //Add the matched/new location as the first suggestion
                rowSuggestions.LocationSuggestions.Add(row.Location.Id);

                var locationSuggestions = _locations.Where(l => l.ClientId == row.Client.Id).Select(l => l.Id).ToArray();
                rowSuggestions.LocationSuggestions.AddRange(locationSuggestions);

                locations.AddRange(_locations.Where(l => locationSuggestions.Contains(l.Id)).Select(FoundOps.Api.Models.Location.ConvertModel));

                //If a new Location was created, add it to the list of location entites
                if (locations.Select(l => l.Id).Contains(row.Location.Id))
                    locations.Add(row.Location);

                // Client
                //Add the matched/new client as the first suggestion
                rowSuggestions.ClientSuggestions.Add(row.Client.Id);

                var clientSuggestions = _clients.Where(c => c.Locations.Select(l => l.Id).Contains(row.Location.Id)).Select(c => c.Id).ToArray();
                rowSuggestions.ClientSuggestions.AddRange(clientSuggestions);

                clients.AddRange(_clients.Where(c => clientSuggestions.Contains(c.Id)).Select(FoundOps.Api.Models.Client.ConvertModel));

                //If a new Client was created, add it to the list of client entites
                if (clients.Select(c => c.Id).Contains(row.Client.Id))
                    clients.Add(row.Client);

                //Repeat
                rowSuggestions.Repeat.Add(row.Repeat);
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
