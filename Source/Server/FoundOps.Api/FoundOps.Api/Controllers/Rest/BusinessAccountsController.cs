using FoundOps.Api.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using BusinessAccount = FoundOps.Api.Models.BusinessAccount;

namespace FoundOps.Api.Controllers.Rest
{
    public class BusinessAccountsController : BaseApiController
    {
        /// <summary>
        /// Returns the current business account with ImageUrl
        /// NOTE: Must have admin privileges
        /// </summary>
        public IQueryable<BusinessAccount> Get(Guid roleId)
        {
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            currentBusinessAccount.PartyImageReference.Load();

            var businessAccount = new BusinessAccount(currentBusinessAccount.CreatedDate) { Id = currentBusinessAccount.Id, Name = currentBusinessAccount.Name };
            businessAccount.SetLastModified(currentBusinessAccount.LastModified, currentBusinessAccount.LastModifyingUserId);

            //Load image url
            if (currentBusinessAccount.PartyImage != null)
            {
                var imageUrl = currentBusinessAccount.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(currentBusinessAccount.Id, currentBusinessAccount.PartyImage.Id);
                businessAccount.ImageUrl = imageUrl;
            }

            return (new[] { businessAccount }).AsQueryable();
        }

        /// <summary>
        /// Can only change properties: Name
        /// </summary>
        /// <param name="businessAccount">The business account</param>
        public void Put(BusinessAccount businessAccount)
        {
            var modelBusinessAccount = CoreEntitiesContainer.BusinessAccount(businessAccount.Id, new[] { RoleType.Administrator }).FirstOrDefault();
            if (modelBusinessAccount == null)
                throw Request.NotAuthorized();

            modelBusinessAccount.Name = businessAccount.Name;
            modelBusinessAccount.LastModified = DateTime.UtcNow;
            modelBusinessAccount.LastModifyingUserId = CoreEntitiesContainer.CurrentUserAccount().Id;

            SaveWithRetry();
        }

        //modelBusinessAccount.PartyImageReference.Load();
        //if (modelBusinessAccount.PartyImage == null)
        //{
        //    var partyImage = new PartyImage { OwnerParty = modelBusinessAccount };
        //    modelBusinessAccount.PartyImage = partyImage;
        //}
        //PartyTools.UpdatePartyImageHelper(CoreEntitiesContainer, modelBusinessAccount, Request);
    }
}
