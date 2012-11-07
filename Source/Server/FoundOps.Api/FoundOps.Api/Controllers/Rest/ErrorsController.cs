using FoundOps.Api.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    /// <summary>
    /// An api controller to track errors.
    /// </summary>
    [Authorize]
    public class ErrorsController : BaseApiController
    {
        public void Put(ErrorEntry errorEntry)
        {
            var currentUser = CoreEntitiesContainer.CurrentUserAccount().First();

            CoreEntitiesContainer.Errors.AddObject(new Error
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                ErrorText = errorEntry.Message,
                UserEmail = currentUser.EmailAddress,
                BusinessName = errorEntry.Business,
                InnerException = errorEntry.Url
            });

            SaveWithRetry();
        }
    }
}