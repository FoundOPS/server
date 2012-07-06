using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.Authentication;
using FoundOps.Core.Tools;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace FoundOps.Server.Services
{
    [System.ServiceModel.DomainServices.Hosting.EnableClientAccess]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AuthenticationService : AuthenticationBase<WebContextUser>
    {
        protected override WebContextUser GetAuthenticatedUser(System.Security.Principal.IPrincipal principal)
        {
            return new WebContextUser(new CoreEntitiesContainer().CurrentUserAccount().FirstOrDefault());
        }
    }
}

