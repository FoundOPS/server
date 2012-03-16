using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Web.Mvc;

namespace FoundOps.Server.Controllers
{
    public class ErrorController : Controller
    {
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
            var container = new CoreEntitiesContainer();
            
            //Current user logged in (email)
            var currentUser = AuthenticationLogic.CurrentUserAccountsEmailAddress();

            //Business account Name
            var businessAccount = container.BusinessAccountForRole(roleId).Name;

            var newError = new Error
                               {
                                   Id = Guid.NewGuid(),
                                   BusinessName = businessAccount,
                                   Date = DateTime.Now,
                                   UserEmail = currentUser,
                                   ErrorText = errorString,
                                   InnerException = innerException
                               };


            container.Errors.AddObject(newError);

            container.SaveChanges();
        }
    }
}
