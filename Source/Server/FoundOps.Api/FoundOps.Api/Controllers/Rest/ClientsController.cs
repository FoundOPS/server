using FoundOps.Api.Models;
using FoundOps.Core.Tools;
using System;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    [Authorize]
    public class ClientsController : BaseApiController
    {
        /// <summary>
        /// Gets the clients for the business account.
        /// </summary>
        public IQueryable<Client> Get(Guid roleId, string search = "", int skip = 0, int take = 10)
        {
            return CoreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.Clients.Where(c => !c.DateDeleted.HasValue))
                .OrderBy(c => c.Name).Where(c => c.Name.ToLower().StartsWith(search.ToLower()))
                .Skip(skip).Take(take).ToArray()
                .Select(Client.ConvertModel).AsQueryable();
        }
    }
}
