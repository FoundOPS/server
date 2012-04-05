using System;
using System.Web.Mvc;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Server.Authentication;

namespace FoundOps.Server.Controllers
{
    public class ErrorController : Controller
    {
        readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

        /// <summary>
        /// This is fired when the site gets a bad URL
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            // log here, perhaps you want to know when a user reaches a 404?
            return View();
        }

        [Authorize]
        public void SaveChangesError(Guid roleId, string errorString, string innerException)
        {
            //Current user logged in (email)
            var currentUser = AuthenticationLogic.CurrentUserAccountsEmailAddress();

            //Business account Name
            var businessAccount = _coreEntitiesContainer.BusinessAccountOwnerOfRole(roleId).Name;

            var newError = new Error
                               {
                                   Id = Guid.NewGuid(),
                                   BusinessName = businessAccount,
                                   Date = DateTime.Now,
                                   UserEmail = currentUser,
                                   ErrorText = errorString,
                                   InnerException = innerException
                               };


            _coreEntitiesContainer.Errors.AddObject(newError);

            _coreEntitiesContainer.SaveChanges();
        }
    }
}
