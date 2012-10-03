using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;

namespace FoundOps.Api.ApiControllers
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
    [Authorize]
    public class ErrorController : BaseApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;

        public ErrorController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        public void Put(ErrorEntry errorEntry)
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