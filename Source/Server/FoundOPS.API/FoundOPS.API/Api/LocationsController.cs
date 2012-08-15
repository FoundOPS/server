using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Http;
using Location = FoundOPS.API.Models.Location;

namespace FoundOPS.API.Api
{
    [FoundOps.Core.Tools.Authorize]
    public class LocationsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        public LocationsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets the Locations for the Client
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Location> Get(Guid roleId, Guid clientId)
        {
            return _coreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.OwnedLocations)
                    .Where(l => l.ClientId == clientId).OrderBy(c => c.AddressLineOne)
                    .Select(Location.ConvertModel).AsQueryable();
        }
    }
}
