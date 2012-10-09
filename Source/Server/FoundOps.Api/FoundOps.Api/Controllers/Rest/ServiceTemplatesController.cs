using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using ServiceTemplate = FoundOps.Api.Models.ServiceTemplate;

namespace FoundOps.Api.Controllers.Rest
{
    public class ServiceTemplatesController : BaseApiController
    {
        /// <summary>
        /// Return the ServiceProvider's service types
        /// </summary>
        public IQueryable<ServiceTemplate> Get(Guid roleId)
        {
            var serviceTemplates = CoreEntitiesContainer.Owner(roleId).SelectMany(ba => ba.ServiceTemplates)
                .Where(st => st.LevelInt == (int)ServiceTemplateLevel.ServiceProviderDefined);

            return serviceTemplates.Select(ServiceTemplate.ConvertModel).AsQueryable().OrderBy(st => st.Name);
        }
    }
}
