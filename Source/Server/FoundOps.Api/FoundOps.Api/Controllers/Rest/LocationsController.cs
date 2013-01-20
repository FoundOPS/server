using System.Data;
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
        /// all depot locations for a business account, or geocode based off the search string
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="locationId">If set: get a specific location</param>
        /// <param name="clientId">Else if set: get all locations for a client</param>
        /// <param name="getDepots">Else if set: get all depot locations for a business account</param>
        /// <param name="search">If set: attempt to geocode. Only returns high-confidence results</param>
        /// <returns>List of locations from geocoded search and from existing locations</returns>
        public IQueryable<Location> Get(Guid roleId, Guid? locationId, Guid? clientId, bool? getDepots = false, string search = null)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            Core.Models.CoreEntities.Location[] locations;

            string sql = string.Empty;
            Guid? lookupId = null;

            if (locationId.HasValue)
            {
                //one location from Id
                sql = "SELECT * FROM Locations WHERE Id = @Id";
                lookupId = locationId;
            }
            else if (clientId != null) //Override with 
            {
                //all Locations for a client
                sql = "SELECT * FROM Locations WHERE ClientId = @Id";
                lookupId = clientId;
            }
            else if (getDepots.HasValue && getDepots.Value)
            {
                //depot locations for a business accounts
                sql = "SELECT * FROM Locations WHERE BusinessAccountIdIfDepot = @Id";
                lookupId = currentBusinessAccount.Id;
            }
            else if (string.IsNullOrEmpty(search))
            {
                throw Request.BadRequest();
            }

            var geocodeResult = new GeocoderResult[] { };
            if (!string.IsNullOrEmpty(search))
            {
                //attempt to geocode
                geocodeResult = BingLocationServices.TryGeocode(search).ToArray();

                if (!geocodeResult.Any())
                    geocodeResult = BingLocationServices.TryGeocode(search).ToArray();

                if (sql == string.Empty)
                    return geocodeResult.Select(Location.ConvertGeocode).AsQueryable();
            }

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                locations = conn.Query<Core.Models.CoreEntities.Location>(sql, new { Id = lookupId }).Where(loc => !loc.DateDeleted.HasValue).ToArray();

                conn.Close();
            }

            var result = geocodeResult.Any() ? locations.Select(Location.ConvertModel).Concat(geocodeResult.Select(Location.ConvertGeocode)).AsQueryable()
                                        : locations.Select(Location.ConvertModel).AsQueryable();
            return result;
        }

        /// <summary>
        /// Insert a new location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The location to be inserted</param>
        public void Post(Guid roleId, Location location)
        {
            //Check to be sure the user has access
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            //Convert the API model back to the FoundOPS model of Locations
            var newLocation = Location.ConvertBack(location);

            var date = DateTime.UtcNow;
            newLocation.CreatedDate = date;
            newLocation.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

            //Update the locations business account
            newLocation.BusinessAccountId = currentBusinessAccount.Id;

            //Insert the location
            CoreEntitiesContainer.Locations.AddObject(newLocation);

            SaveWithRetry();
        }

        /// <summary>
        /// Updates a location
        /// </summary>
        /// <param name="roleId">The current roleId</param>
        /// <param name="location">The location to be inserted</param>
        public void Put(Guid roleId, Location location)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var updatedLocation = Location.ConvertBack(location);

            CoreEntitiesContainer.Locations.Attach(updatedLocation);
            CoreEntitiesContainer.ObjectStateManager.ChangeObjectState(updatedLocation, EntityState.Modified);
            updatedLocation.LastModified = DateTime.UtcNow;
            updatedLocation.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

            SaveWithRetry();
        }
    }
}
