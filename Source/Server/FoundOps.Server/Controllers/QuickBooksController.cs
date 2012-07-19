﻿using System;
using System.Linq;
using System.Web.Mvc;
using FoundOps.Core.Tools;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.QuickBooks;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Controllers
{
    [FoundOps.Core.Tools.Authorize]
    public class QuickBooksController : Controller
    {
        public CoreEntitiesContainer CoreEntitiesContainer = new CoreEntitiesContainer();

        /// <summary>
        /// Checkes whether or not the current Business Account has the neccessary credentials to login to QuickBooks
        /// </summary>
        /// <param name="roleId">The current Business Account's roleId.</param>
        /// <returns>
        /// Boolean value used to determine whether or not the users account requires QuickBooks Authentication
        /// True = Authorization is needed
        /// False = Authorization is not needed
        /// </returns>
        public bool NeedsAuthorization(Guid roleId)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();

            //A null BusinessAccount means that it is actually a user account
            //If QuickBooks is disabled on the account
            if (businessAccount == null || businessAccount.QuickBooksEnabled == false)
                return false;
            //Checks for null tokens on a BusinessAccount that has QuickBooks enabled
            if (businessAccount.QuickBooksAccessToken == null || businessAccount.QuickBooksAccessTokenSecret == null)
                return true;

            //Check validity of tokens
            //If the tokens are invalid return false, else return true
            return (QuickBooksTools.GetUserInfo(businessAccount));
        }

        /// <summary>
        /// Creates the URL needed to send to QuickBooks to get the user authenticated
        /// </summary>
        /// <param name="roleId">The roleId.</param>
        /// <returns>
        /// Creates a page the goes to the GrantURL
        /// </returns>
        public ActionResult GetAuthorization(Guid roleId)
        {
            //Pass the grantUrl to the View as its Model.
            //Cast it as an Object so the proper View method is called
            var grantUrl = String.Format("{0}?roleId={1}", QuickBooksTools.OauthConstants.GrantUrl, roleId);

            return View((Object)grantUrl);
        }

        /// <summary>
        /// Creates the initial call to QuickBooks with our callback URL
        /// </summary>
        /// <param name="roleId">The roleId.</param>
        /// <returns>
        /// Redirects the page to the Authorization URL
        /// </returns>
        public ActionResult OAuthGrantLogin(string roleId)
        {
            //Creates the initial call to QuickBooks with our callback URL
            var oauthCallbackUrl = String.Format("{0}?roleId={1}", QuickBooksTools.OauthConstants.OauthCallbackUrl, roleId);

            //Gets the Business Account based on the roleId passed
            var currentRoleId = new Guid(roleId);
            var businessAccount = CoreEntitiesContainer.Owner(currentRoleId).First();

            var authUrl = QuickBooksTools.GetAuthorizationUrl(oauthCallbackUrl, roleId, businessAccount);

            CoreEntitiesContainer.SaveChanges();

            return Redirect(authUrl);
        }

        /// <summary>
        /// Saves the realmId and the OAuthVerifier to the database.
        /// Uses the BusinessAccount to get the AccessToken and the Base Url for the current QuickBooks user
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>
        /// Directs the page to OAuthGrantHandler Url, which simply closes the window or tab
        /// </returns>
        public ActionResult OAuthGrantHandler(string roleId)
        {
            //Gets the BusinessAccount based on the roleId
            var currentRoleId = new Guid(roleId);
            var businessAccount = CoreEntitiesContainer.Owner(currentRoleId).First();

            var oauthVerifyer = Request.QueryString["oauth_verifier"];
            var realmid = Request.QueryString["realmId"];

            //Creates the QuickBooksSession class based on the information in QuickBooksXml
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(businessAccount.QuickBooksSessionXml);

            //The Id of the QuickBooks Account that is being accessed. Saved for use by API's 
            quickBooksSession.RealmId = realmid;

            //Code used alon side the tokens to ensure that they are not being guessed
            quickBooksSession.OAuthVerifier = oauthVerifyer;

            //Serializes the QuickBooksSession class back to the database
            businessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);

            //Exchanges the RequestToken 
            QuickBooksTools.GetAccessToken(businessAccount, CoreEntitiesContainer);

            //Saves the base url needed for QuickBooks APIs for this user
            QuickBooksTools.GetBaseUrl(businessAccount, CoreEntitiesContainer);

            return View();
        }
    }
}