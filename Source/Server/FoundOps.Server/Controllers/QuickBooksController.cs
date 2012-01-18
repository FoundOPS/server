using System;
using System.Web.Mvc;
using FoundOps.Common.Tools;
using FoundOps.Server.Authentication;
using FoundOps.Core.Models.QuickBooks;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Controllers
{
    public class QuickBooksController : Controller
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer = new CoreEntitiesContainer();

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
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountForRole(roleId);

            //A null BusinessAccount means that it is actually a user account
            //If QuickBooks is disabled on the account
            if (currentBusinessAccount == null || currentBusinessAccount.QuickBooksEnabled == false)
                return false;

            //Required check for an existant QuickBooksXML. If it does not exist, then we know that there is no chance that there is a valid Token
            if (currentBusinessAccount.QuickBooksSessionXml == null)
                return true;

            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //Checks for null tokens on a BusinessAccount that has QuickBooks enabled
            if (quickBooksSession.QBToken == null || quickBooksSession.QBTokenSecret == null)
                return true;

            //Check validity of tokens
            //If the tokens are invalid return false, else return true
            return (QuickBooksTools.GetUserInfo(currentBusinessAccount));
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
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountForRole(new Guid(roleId));

            //Creates the initial call to QuickBooks with our callback URL
            var oauthCallbackUrl = String.Format("{0}?roleId={1}", QuickBooksTools.OauthConstants.OauthCallbackUrl, roleId);

            var quickBooksSession = new QuickBooksSession();

            //Gets the authorization URL as well as saves the Verification token for later use
            var authUrl = QuickBooksTools.GetAuthorizationUrl(oauthCallbackUrl, quickBooksSession);

            //Serializes the QuickBooksSession class back to the database
            currentBusinessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);

            _coreEntitiesContainer.SaveChanges();

            return Redirect(authUrl);
        }

        /// <summary>
        /// Saves the authentication information (OauthToken, OAuthVerifier) to the database.
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
            var currentBusinessAccount = _coreEntitiesContainer.BusinessAccountForRole(currentRoleId);

            var oauthVerifyer = Request.QueryString["oauth_verifier"];
            var realmid = Request.QueryString["realmId"];
            
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //Saves the Verifier provided by OAuth for use when we exchange the Request token for the Access token
            quickBooksSession.OAuthVerifier = oauthVerifyer;

            //The Id of the QuickBooks Account that is being accessed. Saved for use by API's             
            quickBooksSession.RealmId = realmid;

            //Exchanges the RequestToken 
            var accessTokenaAndSecret = QuickBooksTools.GetAccessToken(quickBooksSession);

            //Saving the Token to private storage
            quickBooksSession.QBToken = accessTokenaAndSecret.Token;
            quickBooksSession.QBTokenSecret = accessTokenaAndSecret.TokenSecret;

            //Serializes the QuickBooksSession class back to the database
            currentBusinessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);

            _coreEntitiesContainer.SaveChanges();

            return View();
        }
    }
}