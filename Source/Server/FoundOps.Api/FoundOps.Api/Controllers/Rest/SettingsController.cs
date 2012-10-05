using System.Linq;
using FoundOps.Api.Controllers.Rest;
using FoundOps.Api.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FoundOps.Core.Tools;
using UserAccount = FoundOps.Api.Models.UserAccount;

namespace FoundOps.Api.ApiControllers
{
    [Core.Tools.Authorize]
    public class SettingsController : BaseApiController
    {
        public HttpResponseMessage Put(UserAccount settings, Guid roleId, bool changePass = false, string newPass = null, string oldPass = null)
        {
            var user = CoreEntitiesContainer.CurrentUserAccount().First();

            //The settings passed are not for the current User
            if (user.Id != settings.Id)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            if (SettingsTools.UserExistsConflict(settings.EmailAddress, user.EmailAddress))
                return SettingsTools.UserExistsResponse(Request);

            //Will only be set to false if there is a problem changing the users password
            var changeSuccessful = true;

            if (!changePass)
            {
                //Update Properties
                user.FirstName = settings.FirstName;
                user.LastName = settings.LastName;
                user.EmailAddress = settings.EmailAddress.Trim();
                //user.TimeZone = settings.TimeZoneInfo.TimeZoneId;

                SaveWithRetry();
            }
            else
            {
                if (newPass != null && oldPass != null)
                    changeSuccessful = CoreEntitiesMembershipProvider.ChangePassword(user.EmailAddress, oldPass, newPass);
            }

            //if there was a problem changing the password, return a Not acceptable status code
            //else, return Accepted status code
            return changeSuccessful ? Request.CreateResponse(HttpStatusCode.Accepted) : Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }

        /// <summary>
        /// Updates a user's image.
        /// The form should have 5 inputs:
        /// imageData: image file
        /// x, y, w, h: for cropping
        /// </summary>
        /// <returns>The image url, expiring in 3 hours</returns>
        public string UpdateUserImage()
        {
            var user = CoreEntitiesContainer.CurrentUserAccount().First();

            user.PartyImageReference.Load();

            if (user.PartyImage == null)
            {
                var partyImage = new PartyImage { OwnerParty = user };
                user.PartyImage = partyImage;
            }

            return UpdatePartyImageHelper(user);
        }
    }
}