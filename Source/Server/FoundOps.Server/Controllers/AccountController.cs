using System;
using System.Net;
using Recaptcha;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Routing;
using System.Web.Security;
using FoundOps.Common.Silverlight.Tools;
using FoundOps.Core.Models.Authentication;

namespace FoundOps.Server.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        #region Helper Methods

        private IFormsAuthenticationService FormsService { get; set; }
        private IMembershipService MembershipService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new PartyMembershipService(); }

            base.Initialize(requestContext);
        }

        private ActionResult LogOnLogic(LogOnModel model, string returnUrl, string redirectToOnFailureAction)
        {
            var loginAttempts = AddLoginAttempt();

            if (loginAttempts < 5)
            {
                if (ModelState.IsValid)
                {
                    if (MembershipService.ValidateUser(model.EmailAddress, model.Password))
                    {
                        ClearLoginAttempts();
                        FormsService.SignIn(model.EmailAddress, model.RememberMe);

                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Silverlight", "Home");
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

        #region Login/LogOff Actions

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

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return Redirect(Global.RootBlogUrl);
        }

        #endregion

        #region Password Actions

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
            {
                return RedirectToAction("ChangePasswordSuccess", "Account");
            }
            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            //Setup ViewData when returning Login View
            var viewData = TempData["ViewData"];

            if (viewData != null)  //Save ViewData if passed from TempData
                ViewData = (ViewDataDictionary)viewData;

            return View();
        }

        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(EmailAddressModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateReset(model.EmailAddress) && PerformRecaptcha()) //Ensure the EmailAddress is valid
                {
                    //Set new temporary password
                    var temporaryPassword = PasswordTools.GeneratePassword();

                    //Send email
                    var ss = new SmtpClient("smtp.gmail.com", 587)
                    {
                        EnableSsl = true,
                        Timeout = 10000,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("info@foundops.com", "6Neoy1KRjSVQV6sCk6ax")
                    };

                    var to = model.EmailAddress;
                    const string from = "info@foundops.com";
                    const string subject = "FoundOPS Password Reset";
                    var body = "Your new password is: " + temporaryPassword;
                    var mm = new MailMessage(from, to, subject, body)
                    {
                        BodyEncoding = Encoding.UTF8,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };
                    ss.Send(mm);

                    //Change Password
                    ((CoreEntitiesMembershipProvider)Membership.Provider).ChangePassword(model.EmailAddress, temporaryPassword);

                    return RedirectToAction("ForgotPasswordSuccess", "Account");
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "");

            //Store the ViewData so that you can maintain validation errors on LoginPage
            TempData["ViewData"] = ViewData;
            return RedirectToAction("ForgotPassword", "Account", model);
        }

        #endregion
    }
}