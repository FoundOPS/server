using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Api.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Microsoft.WindowsAzure.StorageClient;
using BusinessAccount = FoundOps.Api.Models.BusinessAccount;

namespace FoundOps.Api.Controllers.Rest
{
    public class BusinessAccountsController : BaseApiController
    {
        public BusinessAccount Get(Guid roleId)
        {
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw Request.NotAuthorized();

            currentBusinessAccount.PartyImageReference.Load();

            var businessAccount = new BusinessAccount { Name = currentBusinessAccount.Name };

            //Load image url
            if (currentBusinessAccount.PartyImage != null)
            {
                var imageUrl = currentBusinessAccount.PartyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(currentBusinessAccount.Id, currentBusinessAccount.PartyImage.Id);
                businessAccount.ImageUrl = imageUrl;
            }

            return businessAccount;
        }

        public HttpResponseMessage Put(BusinessAccount account, Guid roleId, bool updateImage = false)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            businessAccount.Name = account.Name;

            var value = account.Name;

            if(!updateImage)
            {
                businessAccount.PartyImageReference.Load();

                if (businessAccount.PartyImage == null)
                {
                    var partyImage = new PartyImage { OwnerParty = businessAccount };
                    businessAccount.PartyImage = partyImage;
                }

                value = PartyTools.UpdatePartyImageHelper(businessAccount, Request);
            }

            SaveWithRetry();
            
            return Request.CreateResponse(HttpStatusCode.Accepted, value);
        }
    }
}
