using FoundOps.Api.Models;
using FoundOps.Core.Tools;
using System;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    [Authorize]
    public class LocationsController : BaseApiController
    {
        /// <summary>
        /// Gets the Locations for a Client
        /// </summary>
        public IQueryable<Location> Get(Guid roleId, Guid clientId)
        {
            return CoreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.OwnedLocations)
                .Where(l => l.ClientId == clientId).OrderBy(c => c.AddressLineOne)
                .Select(Location.ConvertModel).AsQueryable();
        }
    }
}
