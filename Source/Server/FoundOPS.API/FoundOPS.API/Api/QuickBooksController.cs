using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DevDefined.OAuth.Consumer;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOPS.API.Api
{
    public class QuickBooksController : ApiController
    {
        public CoreEntitiesContainer coreEntitiesContainer = new CoreEntitiesContainer();

        public bool DeauthorizeBusinessToken(BusinessAccount currentBusinessAccount)
        {
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
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns>
        /// Boolean value that determines if the user needs to authenticate their QuickBooks login information
        /// </returns>
        public static bool GetUserInfo(BusinessAccount currentBusinessAccount)
        {
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
                //A return Value of true means that Authorization is needed
                return true;
            }

            //A return value of false means that the attempt at authorization has succeeded
            //No further action needs to be taken to access QuickBooks Online data
            return false;
        }
    }
}
