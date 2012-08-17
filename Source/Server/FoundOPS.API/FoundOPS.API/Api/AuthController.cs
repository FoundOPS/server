using FoundOps.Core.Models.Authentication;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FoundOPS.API.Api
{
    /// <summary>
    /// An api controller which allows users to authenticate.
    /// </summary>
    public class AuthController : ApiController
    {
        private readonly IFormsAuthenticationService _formsService = new FormsAuthenticationService();
        private readonly IMembershipService _membershipService = new PartyMembershipService();

        /// <summary>
        /// Logs in the specified email address.
        /// TODO: login attempt tracking.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="pass">The password.</param>
        /// <param name="redirectUrl">(Optional) If specified, it will redirect here on success.</param>
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage Login(string email, string pass, string redirectUrl = "")
        {
            if (_membershipService.ValidateUser(email, pass))
            {
                _formsService.SignIn(email, false);

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(redirectUrl);
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.Accepted, true);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, false);
        }

        /// <summary>
        /// Log Out the current user.
        /// </summary>
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool LogOut()
        {
            _formsService.SignOut();
            return true;
        }
    }
}