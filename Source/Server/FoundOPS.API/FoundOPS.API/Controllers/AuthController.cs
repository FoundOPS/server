using FoundOps.Core.Models.Authentication;
using System.Web.Http;

namespace FoundOPS.API.Controllers
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
        [AcceptVerbs("GET", "POST")]
        public bool Login(string email, string pass)
        {
            if (ModelState.IsValid)
                if (_membershipService.ValidateUser(email, pass))
                {
                    _formsService.SignIn(email, false);
                    return true;
                }

            return false;
        }
    }
}