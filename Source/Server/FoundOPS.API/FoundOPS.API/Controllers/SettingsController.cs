using FoundOPS.API.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FoundOPS.API.Controllers
{
#if !DEBUG
    [Authorize]
#endif
    public class SettingsController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public SettingsController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        [AcceptVerbs("GET", "POST")]
        public UserSettings GetUserSettings()
        {
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).First();
            var userSettings = new UserSettings { FirstName = user.FirstName, LastName = user.LastName, EmailAddress = user.EmailAddress };

            return userSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateUserSettings(UserSettings settings)
        {
            var user = AuthenticationLogic.CurrentUserAccountQueryable(_coreEntitiesContainer).First();

            //If the email address of the current user changed, check the email address is not in use yet
            if (user.EmailAddress != settings.EmailAddress &&
                _coreEntitiesContainer.Parties.OfType<UserAccount>().Any(ua => ua.EmailAddress == user.EmailAddress))
                throw new Exception("The email address is already in use");

            user.FirstName = settings.FirstName;
            user.LastName = settings.LastName;
            user.EmailAddress = settings.EmailAddress;

            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        [AcceptVerbs("GET", "POST")]
        public BusinessSettings GetBusinessSettings(Guid roleId)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            var businessSettings = new BusinessSettings { Name = businessAccount.Name};
            return businessSettings;
        }

        [AcceptVerbs("POST")]
        public HttpResponseMessage UpdateBusinessSettings(BusinessSettings settings)
        {
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRoleQueryable(roleId).FirstOrDefault();

            if (businessAccount == null)
                ExceptionHelper.ThrowNotAuthorizedBusinessAccount();

            businessAccount.Name = settings.Name;
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}