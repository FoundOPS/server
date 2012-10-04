using Dapper;
using FoundOps.Api.Models;
using FoundOps.Core.Models;
using FoundOps.Core.Tools;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

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
            //Check to be sure the user has correct abilities
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new HttpException(((int)HttpStatusCode.Unauthorized), "User is mobile only");

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

            //Convert the FoundOPS model to the API model
            return locations.Select(Location.ConvertModel).ToArray();
        }
    }
}
