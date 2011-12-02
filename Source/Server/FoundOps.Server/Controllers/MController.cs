using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using FoundOps.Core.Server.Models.Account;
using FoundOps.Core.Server.Models.Authentication;
using FoundOps.Core.Server.Models.Party;
using Recaptcha;

namespace FoundOps.Core.Server.Controllers
{
    public class MController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new PartyMembershipService(); }

            base.Initialize(requestContext);
        }

        [AddTestUsersThenAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "M", null);

            return View();
        }

        [HttpPost] //PhoneGapLogin
        public ContentResult PhoneGapLogin(LogOnModel model)
        {
            
            if (model.EmailAddress == null || model.Password == null)
                return new ContentResult { Content = "false", ContentType = "text/plain" };

            //If this is the first login attempt, add LoginAttempts key to ViewData dictionary
            if (TempData["LoginAttempts"] == null)
                TempData.Add("LoginAttempts", 0);

            var loginAttempts = (int)TempData["LoginAttempts"];

            TempData["LoginAttempts"] = ++loginAttempts;

            if (loginAttempts < 5 && MembershipService.ValidateUser(model.EmailAddress, model.Password))
            {
                FormsService.SignIn(model.EmailAddress, model.RememberMe);

                return new ContentResult { Content = "true", ContentType = "text/plain" };
            }

            return new ContentResult { Content = "false", ContentType = "text/plain" };
        }

        [HttpPost] //LogOn
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (model.EmailAddress == null || model.Password == null)
                return View(model);

            //If this is the first login attempt, add LoginAttempts key to ViewData dictionary
            if (TempData["LoginAttempts"] == null)
                TempData.Add("LoginAttempts", 0);

            var loginAttempts = (int)TempData["LoginAttempts"];

            TempData["LoginAttempts"] = ++loginAttempts;

            if (loginAttempts < 5)
            {
                if (MembershipService.ValidateUser(model.EmailAddress, model.Password))
                {
                    FormsService.SignIn(model.EmailAddress, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    //Force redirect
                    return RedirectToAction("Index", "M", null);
                }
                ModelState.AddModelError("", "The email address or password provided is incorrect.");
            }
            else
            {
                return RedirectToAction("Captcha", "M", null);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost] //LogOn
        public ActionResult CaptchaLogon(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid && PerformRecaptcha())
            {
                TempData["LoginAttempts"] = 0;

                if (MembershipService.ValidateUser(model.EmailAddress, model.Password))
                {
                    FormsService.SignIn(model.EmailAddress, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "M");
                }
                ModelState.AddModelError("", "The email address or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Captcha", "M");
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

        [HttpPost]
        public ActionResult ResetPassword(EmailAddressModel model)
        {
            if (ModelState.IsValid && MembershipService.ValidateReset(model.EmailAddress) && PerformRecaptcha()) //Ensure the EmailAddress is valid
            {
                //Set new temporary password
                var temporaryPassword = GeneratePassword();

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

                return RedirectToAction("ForgotPasswordSuccess", "M");
            }
            ModelState.AddModelError("", "Please enter a valid email address.");

            // If we got this far, something failed, redisplay form
            return RedirectToAction("ForgotPassword", "M", model);
        }

        public string GeneratePassword()
        {
            var password = "";
            var random = new Random();
            const int length = 8;
            for (int i = 0; i < length; i++)
            {
                if (random.Next(0, 3) == 0) //if random.Next() == 0 then we generate a random character
                {
                    password += ((char)random.Next(65, 91)).ToString();
                }
                else //if random.Next() == 0 then we generate a random digit
                {
                    password += random.Next(0, 9);
                }
            }

            return password;
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Login", "M");
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

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
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess", "M");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult Captcha()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }
    }
}
