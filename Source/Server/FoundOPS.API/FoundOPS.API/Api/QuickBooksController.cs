using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DevDefined.OAuth.Consumer;
using FoundOPS.API.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;
using FoundOps.Core.Tools;

namespace FoundOPS.API.Api
{
    public class QuickBooksController : ApiController
    {
        public CoreEntitiesContainer coreEntitiesContainer = new CoreEntitiesContainer();

        [AcceptVerbs("GET", "POST")]
        public bool DeauthorizeBusinessToken(Guid roleId)
        {
            var currentBusinessAccount = coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new Exception("Bad Request");

            //URL for the QuickBooks API for getting the Users Info
            //Here we are accessing the Intuit Partner Platform instead of QuickBooks Online data
            var serviceEndPoint = "https://appcenter.intuit.com/api/v1/Connection/Disconnect";

            //Creates OAuth session and generates AccessToken
            var oSession = QuickBooksTools.CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sends and signs the request to the Data Service
            IConsumerRequest conReq = oSession.Request().Get().ForUrl(serviceEndPoint).SignWithToken();

            //OAuth parameters are needed in the header for this call
            conReq.Context.GenerateOAuthParametersForHeader();

            //Reads the response XML
            var txtServiceResponse = conReq.ReadBody();

            //Checks for an Error Code in the response XML
            if (!(txtServiceResponse.Contains("<ErrorCode>0</ErrorCode>")))
            {
                currentBusinessAccount.QuickBooksEnabled = false;
                currentBusinessAccount.QuickBooksAccessToken = null;
                currentBusinessAccount.QuickBooksAccessTokenSecret = null;

                coreEntitiesContainer.SaveChanges();
                //A return Value of true means that the token has been de-authorized
                return true;
            }

            //A return value of false means that the attempt at de-authorization has failed
            //This would be due to an already invalid token
            return false;
        }

        /// <summary>
        /// Generates a call to the Intuit Partner Platform for get the Users Info
        /// Reads the response from that call and checks for an error code
        /// Based on whether there is an error or not, we determine if the user needs to authenticate their QuickBooks login information
        /// </summary>
        /// <param name="roleId">The current business account.</param>
        /// <returns>
        /// Boolean value that determines if the user needs to authenticate their QuickBooks login information
        /// </returns>
        public QuickBooksConnect GetUserInfo(Guid roleId)
        {
            var currentBusinessAccount = coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new Exception("Bad Request");

            var connect = new QuickBooksConnect();
            connect.IsEnabled = currentBusinessAccount.QuickBooksEnabled;

            if (connect.IsEnabled == false)
            {
                connect.IsConnected = false;
                return connect;
            }

            //URL for the QuickBooks API for getting the Users Info
            //Here we are accessing the Intuit Partner Platform instead of QuickBooks Online data
            var serviceEndPoint = "https://workplace.intuit.com/db/main?a=API_GetUserInfo";

            //Creates OAuth session and generates AccessToken
            var oSession = QuickBooksTools.CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sends and signs the request to the Data Service
            IConsumerRequest conReq = oSession.Request().Get().ForUrl(serviceEndPoint).SignWithToken();

            //OAuth parameters are needed in the header for this call
            conReq.Context.GenerateOAuthParametersForHeader();

            //Reads the response XML
            var txtServiceResponse = conReq.ReadBody();

            //Checks for an Error Code in the response XML
            if (!(txtServiceResponse.Contains("<errcode>0</errcode>")))
            {
                //Authorization is needed
                connect.IsEnabled = false;
            }

            //Attempt at authorization has succeeded
            //No further action needs to be taken to access QuickBooks Online data
            connect.IsEnabled = true;

            return connect;
        }

        /// <summary>
        /// Changes the quickbooks status on a business account
        /// </summary>
        /// <param name="roleId">RoleId of the business account</param>
        /// <param name="enabled">Boolean value that you would like to change the status to</param>
        /// <returns>Http status code of accepted if all goes well</returns>
        [AcceptVerbs("GET", "POST")]
        public HttpStatusCode UpdateQuickBooksStatus (Guid roleId, bool enabled)
        {
            var currentBusinessAccount = coreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                return HttpStatusCode.BadRequest;

            currentBusinessAccount.QuickBooksEnabled = enabled;

            coreEntitiesContainer.SaveChanges();

            return HttpStatusCode.Accepted;
        }
    }
}
