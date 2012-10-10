using FoundOps.Core.Models;
using FoundOps.Core.Models.Authentication;
using Recaptcha;
using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FoundOps.Server.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        #region Helper Methods

        private CoreEntitiesMembershipProvider _coreEntitiesMembershipProvider;
        protected override void Initialize(RequestContext requestContext)
        {
            if (_coreEntitiesMembershipProvider == null) { _coreEntitiesMembershipProvider = new CoreEntitiesMembershipProvider(); }

            base.Initialize(requestContext);
        }

        private ActionResult LogOnLogic(LogOnModel model, string returnUrl, string redirectToOnFailureAction)
        {
            var loginAttempts = AddLoginAttempt();

            if (loginAttempts < 5)
            {
                if (ModelState.IsValid)
                {
                    if (_coreEntitiesMembershipProvider.ValidateUser(model.EmailAddress, model.Password))
                    {
                        //Also authenticate API
                        ClearLoginAttempts();
                        FormsAuthentication.SetAuthCookie(model.EmailAddress, true);

                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return Redirect(AppConstants.ApplicationUrl);
                    }
                    ModelState.AddModelError("", "The email address or password provided is incorrect.");
                }
            }
            else
            {
                return RedirectToAction("Captcha", "Account");
            }

            // If we got this far something failed. Display page w validation errors
            return View(redirectToOnFailureAction, model);
        }

        private bool PerformRecaptcha()
        {
            var validator = new RecaptchaValidator
            {
                PrivateKey = "<6LeMMMYSAAAAAP0q9WGcmrIO3KsRbct-O6KyL4r6>",
                RemoteIP = Request.UserHostAddress,
                Response = Request.Form["recaptcha_response_field"],
                Challenge = Request.Form["recaptcha_challenge_field"]
            };

            try
            {
                var validationResult = validator.Validate();

                if (validationResult.ErrorMessage == "incorrect-captcha-sol")
                    ModelState.AddModelError("PassReCaptcha", string.Format("Please retry the ReCaptcha portion again."));

                return validationResult.IsValid;
            }
            catch (Exception)
            {
                ModelState.AddModelError("PassReCaptcha", "an error occured with ReCaptcha please consult documentation.");
                return false;
            }
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

        private void ClearLoginAttempts()
        {
            //Clear login attempts
            TempData.Remove("LoginAttempts");
        }

        #endregion

        #region Login/LogOut Actions

        /// <summary>
        /// Allows a user to login after they have passed the 5 attempts threshold.
        /// </summary>
        public ActionResult Captcha()
        {
            return View();
        }

        /// <summary>
        /// Allows a user to login after they have passed the 5 attempts threshold.
        /// </summary>
        [HttpPost]
        public ActionResult CaptchaLogin(LogOnModel model, string returnUrl)
        {
            if (PerformRecaptcha())
                ClearLoginAttempts();

            return LogOnLogic(model, returnUrl, "Captcha");
        }

        public ActionResult Login()
        {
            //Setup ViewData when returning Login View
            var viewData = TempData["ViewData"];

            if (viewData != null)  //Save ViewData if passed from TempData
                ViewData = (ViewDataDictionary)viewData;

            return View();
        }

        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            return LogOnLogic(model, returnUrl, "Login");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return Redirect(AppConstants.RootFrontSiteUrl);
        }

        #endregion

        #region Password Actions

        /// <summary>
        /// A captcha page for forgetting password
        /// </summary>
        public ActionResult ForgotPassword()
        {
            //Setup ViewData when returning Login View
            var viewData = TempData["ViewData"];

            if (viewData != null)  //Save ViewData if passed from TempData
                ViewData = (ViewDataDictionary)viewData;

            return View();
        }

        /// <summary>
        /// Checks the captcha, then sends an email with a reset password link.
        /// </summary>
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid && PerformRecaptcha())
            {
                if (_coreEntitiesMembershipProvider.ResetAccount(model.EmailAddress))
                {
                    return RedirectToAction("ForgotPasswordSuccess", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "");

            //Store the ViewData so that you can maintain validation errors on LoginPage
            TempData["ViewData"] = ViewData;
            return RedirectToAction("ForgotPassword", "Account", model);
        }

        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// A page for resetting a password
        /// </summary>
        public ActionResult ResetPassword()
        {
            //sign out first, in case they are logged in
            FormsAuthentication.SignOut();
            return View();
        }

        /// <summary>
        /// Sets the password
        /// </summary>
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model, string resetCode)
        {
            if (ModelState.IsValid && _coreEntitiesMembershipProvider.SetPassword(resetCode, model.NewPassword))
                return RedirectToAction("ResetPasswordSuccess", "Account");

            //If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "");

            return RedirectToAction("ResetPassword", "Account", new RouteValueDictionary { { "resetCode", resetCode } });
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        #endregion
    }
}