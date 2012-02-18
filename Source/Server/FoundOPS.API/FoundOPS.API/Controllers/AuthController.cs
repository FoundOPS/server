using System.Web.Mvc;
using System.Web.Routing;
using FoundOps.Core.Models.Authentication;

namespace FoundOPS.API.Controllers
{
    public class AuthController : Controller
    {
        private IFormsAuthenticationService FormsService { get; set; }
        private IMembershipService MembershipService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new PartyMembershipService(); }

            base.Initialize(requestContext);
        }

        public bool Login(string email, string pass)
        {
            var loginAttempts = AddLoginAttempt();

            if (loginAttempts < 5)
            {
                if (ModelState.IsValid)
                {
                    if (MembershipService.ValidateUser(email, pass))
                    {
                        FormsService.SignIn(email, false);

                        return true;
                    }
                }
            }

            return false;
        }

        private int AddLoginAttempt()
        {
            //Keep track of login attempts
            if (TempData["LoginAttempts"] == null)
                TempData.Add("LoginAttempts", 0);

            var loginAttempts = (int)TempData["LoginAttempts"];
            TempData["LoginAttempts"] = ++loginAttempts;

            return loginAttempts;
        }
    }
}