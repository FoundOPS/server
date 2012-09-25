using System;
using System.Linq;
using System.Web.Http;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;

namespace FoundOPS.API.Api
{
    public class ErrorEntry
    {
        public string Business { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }

    /// <summary>
    /// An api controller to track errors.
    /// </summary>
    [FoundOps.Core.Tools.Authorize]
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
        public void Track(ErrorEntry errorEntry)
        {
            var currentUser = _coreEntitiesContainer.CurrentUserAccount().First();

            _coreEntitiesContainer.Errors.AddObject(new Error
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.UtcNow,
                    ErrorText = errorEntry.Message,
                    UserEmail = currentUser.EmailAddress,
                    BusinessName = errorEntry.Business,
                    InnerException = errorEntry.Url
                });

            _coreEntitiesContainer.SaveChanges();
        }
    }
}