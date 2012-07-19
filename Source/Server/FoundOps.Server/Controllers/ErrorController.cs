using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Mvc;

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

        [FoundOps.Core.Tools.Authorize]
        public void SaveChangesError(Guid roleId, string errorString, string innerException)
        {
            //Current user logged in (email)
            var currentUser = _coreEntitiesContainer.CurrentUserAccount().First();

            //Business account Name
            var businessAccount = _coreEntitiesContainer.Owner(roleId).First().Name;

            var newError = new Error
            {
                Id = Guid.NewGuid(),
                BusinessName = businessAccount,
                Date = DateTime.UtcNow,
                UserEmail = currentUser.EmailAddress,
                ErrorText = errorString,
                InnerException = innerException
            };


            _coreEntitiesContainer.Errors.AddObject(newError);

            _coreEntitiesContainer.SaveChanges();
        }
    }
}
