using System;
using System.Linq;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;

namespace FoundOPS.API.Api
{
    /// <summary>
    /// An api controller to track errors.
    /// </summary>
#if !DEBUG
    [FoundOps.Server.Tools.Authorize]
#endif
    public class ErrorController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public ErrorController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Stores an error.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public void Track([FromBody]string error, [FromUri] string business, [FromUri] string section)
        {
            var currentUser = _coreEntitiesContainer.CurrentUserAccount().First();

            _coreEntitiesContainer.Errors.AddObject(new Error
                {
                    Date = DateTime.UtcNow,
                    ErrorText = error,
                    UserEmail = currentUser.EmailAddress,
                    BusinessName = business,
                    InnerException = section
                });

            _coreEntitiesContainer.SaveChanges();
        }
    }
}