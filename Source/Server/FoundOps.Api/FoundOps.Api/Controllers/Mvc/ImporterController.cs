using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using FoundOPS.API.Models;
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

namespace FoundOps.Api.Controllers.Mvc
{
    public class ImporterController : Controller
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        private readonly Guid _newEntityGuid = new Guid("10000000-0000-0000-0000-000000000000");

        public ImporterController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        public enum CellStatus
        {
            Linked = 0,
            New = 1,
            Suggested = 2,
            Error = 3
        }

        public Suggestions ValidateInput(Guid roleId, string[] headers, List<Cell[]> rows)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                return null;

            const string sql = @"SELECT * FROM dbo.Locations WHERE BusinessAccountId = @id
                                             SELECT * FROM dbo.Clients WHERE BusinessAccountId = @id
                                 SELECT * FROM dbo.Regions WHERE BusinessAccountId = @id";

            Location[] locations;
            Client[] clients;
            FoundOps.Core.Models.CoreEntities.Region[] regions;


            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@id", businessAccount.Id);

                using (var data = conn.QueryMultiple(sql, new { id = businessAccount.Id }))
                {
                    locations = data.Read<Location>().ToArray();

                    clients = data.Read<Client>().ToArray();

                    regions = data.Read<FoundOps.Core.Models.CoreEntities.Region>().ToArray();
                }

                conn.Close();
            }

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
            var repeatOnCol = Array.IndexOf(headers, "Repeat On");
            var repeatEveryCol = Array.IndexOf(headers, "Repeat Every");
            var startDateCol = Array.IndexOf(headers, "Repeat Start Date");
            var endDateCol = Array.IndexOf(headers, "Repeat End Date");
            var frequencyDetailCol = Array.IndexOf(headers, "FrequencyDetail");

            #endregion

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
                        if (latitude.V == "" && longitude.V == "")
                        {
                            var geocodeResult = BingLocationServices.TryGeocode(new Address { AddressLineOne = addressLineOne.V, City = city.V, State = state.V, ZipCode = zipCode.V }).FirstOrDefault();

                            if (geocodeResult != null)
                            {
                                latitude.V = geocodeResult.Latitude;
                                longitude.V = geocodeResult.Longitude;
                            }

                            //If they still do not have values, throw error on all entries in the row.
                            if (latitude.V == null && longitude.V == null)
                                importedLocation.StatusInt = (int)CellStatus.Error;

                            //Matched the address entered and client name matched to a location
                            var matchedLocation = locations.FirstOrDefault(l => row[clientNameCol] != null && l.ClientId != null &&
                                   (addressLineTwo != null && (l.AddressLineOne == addressLineOne.V && l.AddressLineTwo == addressLineTwo.V
                                        && clients.First(c => c.Id == l.ClientId).Name == row[clientNameCol].V)));

                            if (matchedLocation != null)
                            {
                                importedLocation = Api.Models.Location.ConvertModel(matchedLocation);

                                if (regionName != null)
                                {
                                    var region = regions.FirstOrDefault(r => r.Name == regionName.V);
                                    if (region != null)
                                        importedLocation.Region = Region.Convert(region);
                                }

                                importedLocation.StatusInt = (int)CellStatus.Linked;
                            }
                        }
                        //If lat/long exist, try and match based on that and 
                        else if (importedLocation.StatusInt != (int)CellStatus.Linked && latitude.V != "" && longitude.V != "")
                        {
                            //Try and match a location to one in the FoundOPS system
                            var existingLocation = addressLineTwo != null
                                //Use this statement if Address Line Two is not null
                            ? locations.FirstOrDefault(l => l.AddressLineTwo == addressLineTwo.V && (l.Latitude != null && l.Longitude != null)
                                && (decimal.Round(l.Latitude.Value, 6) == decimal.Round(Convert.ToDecimal(latitude.V), 6)
                                && decimal.Round(l.Longitude.Value, 6) == decimal.Round(Convert.ToDecimal(longitude.V), 6)))
                                //Use this statement if Address Line Two is null
                            : locations.FirstOrDefault(l => (l.Latitude != null && l.Longitude != null)
                                && (decimal.Round(l.Latitude.Value, 6) == decimal.Round(Convert.ToDecimal(latitude.V), 6)
                                && decimal.Round(l.Longitude.Value, 6) == decimal.Round(Convert.ToDecimal(longitude.V), 6)));

                            //If a match is found, assign Linked status to all cells
                            if (existingLocation != null)
                            {
                                importedLocation = Api.Models.Location.ConvertModel(existingLocation);

                                if (regionName != null)
                                {
                                    var region = regions.FirstOrDefault(r => r.Name == regionName.V);
                                    if (region != null)
                                        importedLocation.Region = Region.Convert(region);
                                }

                                importedLocation.StatusInt = (int)CellStatus.Linked;
                            }
                        }
                        //If no linked location is found, it means that the location is new
                        else if (importedLocation.StatusInt != (int)CellStatus.Linked)
                        {
                            importedLocation = new Api.Models.Location
                            {
                                Id = _newEntityGuid,
                                AddressLineOne = addressLineOne.V,
                                AddressLineTwo = addressLineTwo != null ? addressLineTwo.V : null,
                                City = city.V,
                                State = state.V,
                                ZipCode = zipCode.V,
                                StatusInt = (int)CellStatus.New,
                                ContactInfoSet = new List<ContactInfo>(),
                                Latitude = latitude.V,
                                Longitude = longitude.V
                            };

                            if (regionName != null)
                            {
                                var region = regions.FirstOrDefault(r => r.Name == regionName.V);
                                if (region != null)
                                    importedLocation.Region = Region.Convert(region);
                            }
                        }
                    }

                    importRow.Location = importedLocation;


                    #endregion

                    var clientName = row[clientNameCol];

                    #region Client

                    if (clientName != null && clientName.V != "")
                    {
                        var existingClient = clients.FirstOrDefault(c => c.Name == clientName.V);

                        Models.Client importedClient;

                        if (existingClient != null)
                        {
                            importedClient = FoundOps.Api.Models.Client.ConvertModel(existingClient);
                            importedClient.StatusInt = (int)CellStatus.Linked;
                        }
                        else
                        {
                            importedClient = new FoundOps.Api.Models.Client
                            {
                                Id = _newEntityGuid,
                                Name = clientName.V,
                                ContactInfoSet = new List<ContactInfo>(),
                                StatusInt = (int)CellStatus.New
                            };

                        }

                        importRow.Client = importedClient;
                    }

                    #endregion
                });

            return null;
        }

        public Suggestions ValidateEntites(ImportRow[] rows)
        {
            var suggestionToReturn = new Suggestions();

            foreach (var row in rows)
            {
                var rowSuggestions = new RowSuggestions();



            }
            return null;
        }

    }
}
