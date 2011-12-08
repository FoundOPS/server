using System.Linq;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.Account.Extensions;
using FoundOps.Core.Models.CoreEntities;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace FoundOps.Server.Services
{
    [System.ServiceModel.DomainServices.Hosting.EnableClientAccess]
    public class AuthenticationService : AuthenticationBase<WebContextUser>
    {
        protected override WebContextUser GetAuthenticatedUser(System.Security.Principal.IPrincipal principal)
        {
            return new WebContextUser(AuthenticationLogic.CurrentUserAccountQueryable(new CoreEntitiesContainer()).FirstOrDefault());
        }
    }
}

