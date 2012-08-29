using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Http;
using Client = FoundOPS.API.Models.Client;

namespace FoundOPS.API.Api
{
    [FoundOps.Core.Tools.Authorize]
    public class ClientsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        public ClientsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets the clients for the business account.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public IQueryable<Client> Get(Guid roleId, string search, int skip, int take)
        {
            return _coreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.Clients)
                .OrderBy(c => c.Name).Where(c => c.Name.ToLower().StartsWith(search.ToLower()))
                .Skip(skip).Take(take).ToArray()
                .Select(Client.ConvertModel).AsQueryable();
        }
    }
}
