using Dapper;
using FoundOps.Api.Tools;
using FoundOps.Common.NET;
using FoundOps.Core.Models;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Location = FoundOps.Api.Models.Location;

namespace FoundOps.Api.Controllers.Rest
{
    [Authorize]
    public class LocationsController : BaseApiController
    {
        /// <summary>
        /// Gets either one location based on Id passed, all locations for a client,
        /// or all depot locations for a business account
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="locationId">Used if you only want to get a specific location</param>
        /// <param name="clientId">Used if you want to get all locations for a client</param>
        /// <param name="getDepots">Used if you want to get all depot locations for a business account. Defaults to false</param>
        /// <returns>List of locations</returns>
        public Location[] Get(Guid roleId, Guid? locationId, Guid? clientId, bool? getDepots = false)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            Core.Models.CoreEntities.Location[] locations;

            //Default the SQL query and the Id to be for looking up one location
            var sql = "SELECT * FROM Locations WHERE Id = @Id";
            var lookupId = locationId;

            //Override with looking up all depot locations for a business accounts
            if (getDepots.HasValue && getDepots.Value)
            {
                sql = "SELECT * FROM Locations WHERE BusinessAccountIdIfDepot = @Id";
                lookupId = currentBusinessAccount.Id;
            }
            else if (clientId != null) //Override with looking up all Locations for a client
            {
                sql = "SELECT * FROM Locations WHERE ClientId = @Id";
                lookupId = clientId;
            }

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                locations = conn.Query<Core.Models.CoreEntities.Location>(sql, new { Id = lookupId }).ToArray();

                conn.Close();
            }

            return locations.Select(Location.ConvertModel).ToArray();
        }

        /// <summary>
        /// Tries to geocode based off of the search string
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="search">String to be geocoded</param>
        /// <returns>A list of high confidence geocoded locations based on the search string</returns>
        public IEnumerable<Location> GetAllLocations(Guid roleId, string search)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            //Attempt to geocode, returns only high-confidence results
            var geocodeResult = BingLocationServices.TryGeocode(search);

            return geocodeResult.Select(Location.ConvertGeocode);
        }

        /// <summary>
        /// Insert a new location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The location to be inserted</param>
        /// <returns>Http response to signify whether or not the location was inserted sucessfully</returns>
        public void Post(Guid roleId, Location location)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            //Convert the API model back to the FoundOPS model of Locations
            var newLocation = Location.ConvertBack(location);

            //Update the locations business account
            newLocation.BusinessAccountId = currentBusinessAccount.Id;

            //Insert the location
            CoreEntitiesContainer.Locations.AddObject(newLocation);

            SaveWithRetry();
        }

        /// <summary>
        /// Update a location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The updated location model to be saved</param>
        /// <returns>Http response to signify whether or not the location was updated sucessfully</returns>
        public void Put(Guid roleId, Location location)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            //Find the original location to be updated
            var original = CoreEntitiesContainer.Locations.FirstOrDefault(l => l.Id == location.Id);

            //If no location exists, return Http response with an error
            if (original == null)
                throw Request.NotFound();

            //Update all properties on the original location
            original.AddressLineOne = location.AddressLineOne;
            original.AddressLineTwo = location.AddressLineTwo;
            original.AdminDistrictOne = location.State;
            original.AdminDistrictTwo = location.City;
            original.PostalCode = location.ZipCode;
            original.CountryCode = location.CountryCode;

            SaveWithRetry();
        }
    }
}
