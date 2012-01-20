using System;
using System.Linq;
using System.Text;
using FoundOps.Common.Tools;
using Microsoft.WindowsAzure;
using DevDefined.OAuth.Consumer;
using FoundOps.Core.Models.Azure;
using DevDefined.OAuth.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.WindowsAzure.StorageClient;

namespace FoundOps.Core.Models.QuickBooks
{
    /// <summary>
    /// Class that contains tools used to access and use QuickBooks Online
    /// </summary>
    public class QuickBooksTools
    {
        #region Constants

        public struct OauthConstants
        {
            public const string IdFedOAuthBaseUrl = "https://oauth.intuit.com/oauth/v1";
            public const string UrlRequestToken = "/get_request_token";
            public const string UrlAccessToken = "/get_access_token";

            //AKA oauthLink
            public const string AuthorizeUrl = "https://workplace.intuit.com/Connect/Begin";

            public const string GrantUrl = "http://localhost:31820/QuickBooks/OAuthGrantLogin";

            //Consumer Key/Secret for our IntuitAnywhere App
            public const string ConsumerKey = "qyprdFc7q3QeH04htbj4pPtXdlmX0f"; 
            public const string ConsumerSecret = "nxKtqCZGm0kpwWiVG85Ur6HGHUDvTW45NLmdGUWL";

            public const string OauthCallbackUrl = "http://localhost:31820/QuickBooks/OAuthGrantHandler";

            //Connection String for Azure Tables
            public const string StorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=fstorequickbooks;AccountKey=fyZ0dsbFfQZET9RFdIlSeVepYgmtO0aBQYVArhazF0KO2X80BUZ2drEJLmRYjDbzelf7PAKzTrePzMJpt3vGaA==";
        }

        #endregion

        #region OAuth Session, Token Validity Check, AuthorizationUrl and Access Token

        /// <summary>
        /// Creates the new OAuth session as well as getting the AccessToken for that Session.
        /// Here instead of getting the Access Token by exchanging RequestTokens, we are able to just pull it from storage
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns>
        /// Oath Session with the AccessToken held within it
        /// </returns>
        public static OAuthSession CreateOAuthSessionAndAccessToken(BusinessAccount currentBusinessAccount)
        {
            //Generates the consumer context based on the constants for our IntuitAnywhere App 
            //In this case the Consumer is actually FoundOps
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = OauthConstants.ConsumerKey,
                SignatureMethod = SignatureMethod.HmacSha1,
                ConsumerSecret = OauthConstants.ConsumerSecret
            };

            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //Generates the OAuth session based on the constants for our IntuitAnywhere App and the ConsumerContext created above 
            var oSession = new OAuthSession(consumerContext,
                                                     OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlRequestToken,
                                                     OauthConstants.AuthorizeUrl,
                                                     OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlAccessToken);
            oSession.AccessToken = new TokenBase
                                       {
                                           Token = quickBooksSession.QBToken,
                                           ConsumerKey = OauthConstants.ConsumerKey,
                                           TokenSecret = quickBooksSession.QBTokenSecret
                                       };
            oSession.ConsumerContext.UseHeaderForOAuthParameters = true;


            //Access Token is generated from storage here and saved into the OauthSession
            return oSession;
        }

        /// <summary>
        /// Creates the OAuthSession based on the consumer context generated here
        /// as well as the constants provided to us by Intuit for the IntuitAnywhere App  
        /// </summary>
        /// <returns>
        /// The Oauth Session
        /// </returns>
        public static IOAuthSession CreateOAuthSession()
        {
            //Generates the consumer context based on the constants for our IntuitAnywhere App 
            //In this case the Consumer is actually FoundOps
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = OauthConstants.ConsumerKey,
                ConsumerSecret = OauthConstants.ConsumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };

            //Generates the OAuth session based on the constants for our IntuitAnywhere App and the ConsumerContext created above 
            return new OAuthSession(consumerContext,
                                            OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlRequestToken,
                                            OauthConstants.IdFedOAuthBaseUrl,
                                            OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlAccessToken);
        }

        /// <summary>
        /// Checks the validity of token. Returns true if the token is valid, otherwise it returns false
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns>True if the token is valid and false if the token has expired</returns>
        public static bool CheckValidityOfToken(BusinessAccount currentBusinessAccount)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            //Required check for an existant QuickBooksXML. If it does not exist, then we know that there is no chance that there is a valid Token
            if (currentBusinessAccount.QuickBooksEnabled == false && currentBusinessAccount.QuickBooksSessionXml == null)
                return false;

            //Deserializes the QuickBooksSession class
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //Checks for null tokens on a BusinessAccount that has QuickBooks enabled
            if (quickBooksSession.QBToken == null || quickBooksSession.QBTokenSecret == null)
                return false;

            //Check validity of tokens
            //If the tokens are invalid return false, else return true
            var inverseOfValidity = GetUserInfo(currentBusinessAccount);

            if(inverseOfValidity)
            {
                //Set the token values to null so we know that even though the BusinessAccount has enabled QuickBooks, they no longer have a valid token and need to be re-prompted
                quickBooksSession.QBToken = null;
                quickBooksSession.QBTokenSecret = null;

                //Re-Serialize the QuickBooksSession class into XML
                currentBusinessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);

                //save the changes that were just made to the BusinessAccount
                coreEntitiesContainer.SaveChanges();
            }

            //Required to do inverse because the GetUserInfo method is set up to return false if authorization is not needed
            return !inverseOfValidity;
        }

        #endregion

        #region Base Url, User Info and Entity List

        /// <summary>
        /// Finds the BaseURL for the current QuickBooks user
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns>
        /// The BaseUrl for a check in WF. Null means that something is wrong with their QuickBooks Settings.
        /// A good BaseUrl means that it worked and that the workflow can continue.
        /// </returns>
        public static string GetBaseUrl(BusinessAccount currentBusinessAccount)
        {
            QuickBooksSession quickBooksSession;

            // Checks to be sure that both QuickBooks is enabled on the current account and that the account has a pre-established QuickBooksSession
            if (currentBusinessAccount.QuickBooksEnabled && currentBusinessAccount.QuickBooksSessionXml != null)
                quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);
            else
            {
                //Used in WF - empty string causes the conditional to fail and ends the WF for that BusinessAccount
                return "";
            }
            //URL for the QuickBooks Data Service for getting the BaseURL
            var serviceEndPoint = String.Format("https://qbo.intuit.com/qbo1/rest/user/v2/" + quickBooksSession.RealmId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sends and signs the request to the Data Service
            IConsumerRequest conReq = oSession.Request().Get().ForUrl(serviceEndPoint).SignWithToken();

            //OAuth parameters are needed in the header for this call
            conReq.Context.GenerateOAuthParametersForHeader();

            //Reads the response XML
            var responseString = conReq.ReadBody();

            //Splits the response XML into by line
            string[] responseArray = responseString.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray)
            {
                if (s.Contains("qbo:BaseURI>"))
                {
                    responseArray = s.Split('>');
                    return responseArray[1];
                }
            }

            //Will only reach here if there is an error getting the baseUrl
            //Used in WF - empty string causes the conditional to fail and ends the WF for that BusinessAccount
            return "";
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
            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

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

            #region Used for testing purposes only

            //GetInvoiceList(currentBusinessAccount);
            //var invoiceInfo = CreateNewInvoice(currentBusinessAccount);
            //UpdateInvoice(currentBusinessAccount, invoiceInfo);
            //var integer = Convert.ToInt32(invoiceInfo[1]);
            //integer++;
            //invoiceInfo[1] = integer.ToString();
            //DeleteInvoice(currentBusinessAccount, invoiceInfo);

            #endregion

            //A return value of false means that the attempt at authorization has succeeded
            //No further action needs to be taken to access QuickBooks Online data
            return false;
        }

        /// <summary>
        /// Makes a call to QuickBooks Data Services to get a list of invoices
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="entityType">The type of QuickBooks Entity you want to get a list of</param>
        /// <param name="baseUrl">The baseUrl </param>
        /// <param name="filter">Specifies the filter if one is provided</param>
        public static string GetEntityList(BusinessAccount currentBusinessAccount, string entityType, string baseUrl, string filter = null)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the List of Invoices
            //In this case we are only looking for the first 25 of them
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(baseUrl + @"/resource/" + entityType + "/v2/" + quickBooksSession.RealmId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUrl(serviceEndPoint);

            //Sets up the filter to send back the first page with 25 results on it
            String completeFilter = filter;
            if (filter == null)
                completeFilter = "PageNum=" + 1 + "&ResultsPerPage=" + 25;

            //Converts the filter created above from a string to a dictionary
            //In order to send the filter as part of the request it needs to be a dictionary
            String[] splitParams = completeFilter.Split('&');
            var paramCollection = splitParams.Select(t => t.Split('=')).ToDictionary(nameValueSplit => nameValueSplit[0], nameValueSplit => nameValueSplit[1]);

            //Adds the parameters of sending back Page 1 with 25 invoices on it
            consumerRequest = consumerRequest.WithFormParameters(paramCollection);

            //Signs the Request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/x-www-form-urlencoded").WithBody(completeFilter);

            //Reads the response XML
            return consumerRequest.ReadBody();
        }

        /// <summary>
        /// Gets the entity by id.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="entityType">The entity type for this call to the function</param>
        /// <param name="id">The specified Filter</param>
        /// <param name="baseUrl"> </param>
        /// <returns></returns>
        public static string GetEntityById(BusinessAccount currentBusinessAccount, string entityType, string baseUrl, string id = null)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks Data Service for getting the BaseURL
            var serviceEndPoint = String.Format(baseUrl + "/resource/" + entityType + "/v2" + quickBooksSession.RealmId + id);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sends and signs the request to the Data Service
            IConsumerRequest conReq = oSession.Request().Get().ForUrl(serviceEndPoint).SignWithToken();

            //OAuth parameters are needed in the header for this call
            conReq.Context.GenerateOAuthParametersForHeader();

            //Reads the response XML
            return conReq.ReadBody();
        }

        #endregion

        #region Create, Update and Delete

        #region Invoice CrUD

        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to create the Invoice that is specifed
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        public static string CreateNewInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice, string baseUrl)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the creating an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(baseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            var customerNameForFilter = currentInvoice.Client.DisplayName;

            #region Generates the XML body of the Post call

            #region Gets the ClientId

            var filter = "Filter=NAME" + " :EQUALS: " + customerNameForFilter;

            var clientXML = GetEntityList(currentBusinessAccount, "customers", baseUrl, filter);

            var clientId = "";

            //Splits the response XML by line
            string[] responseArray = clientXML.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray)
            {
                if (s.Contains("Id idDomain"))
                {
                    responseArray = s.Split('>');
                    clientId = responseArray[1];
                    break;
                }
            }

            #endregion

            //If the name on the Invoice doesnt match one of the Customer Names in QBO, Create a new Customer in QBO and use that one
            if (clientId == "")
                clientId = CreateCustomer(currentBusinessAccount, baseUrl, customerNameForFilter);

            var body = QuickBooksXml.InvoiceXml(currentInvoice, clientId, Operation.Create);

            #endregion

            //Signs the request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes(body));

            var response = "";

            try
            {
                response = consumerRequest.ReadBody();
            }
            catch
            {
            }

            //Reads the response XML
            return response;
        }

        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to update the specified invoice
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="coreEntitiesContainer">The CoreEntitesContainer</param>
        public static void UpdateInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice, CoreEntitiesContainer coreEntitiesContainer, string baseUrl)
        {
            currentInvoice = MergeChangesWithQuickBooks(currentBusinessAccount, currentInvoice, coreEntitiesContainer, baseUrl);

            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the updating an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(baseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId + "/" + currentInvoice.CustomerId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            #region Generates the XML body of the Post call

            var filter = "Name=" + ":EQUALS:" + currentInvoice.Client.DisplayName;

            var clientXML = GetEntityList(currentBusinessAccount, "customers", baseUrl, filter);

            var clientId = "";

            //Splits the response XML into by line
            string[] responseArray = clientXML.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray.Where(s => s.Contains(":Id>")))
            {
                responseArray = s.Split('>');
                clientId = responseArray[1];
                break;
            }

            var body = QuickBooksXml.InvoiceXml(currentInvoice, clientId, Operation.Update);

            #endregion

            //Signs the Request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes((string)body));

            //Reads the response XML
            var responseString = consumerRequest.ReadBody();
        }

        #region Delete - Not Allowed in Production Apps Yet
        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to delete the specified invoice
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        public static void DeleteInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice)
        {
            return;

            #region Intuit Doesn't Currently Allow Delete Functionality in Production Apps
            //var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            ////URL for the QuickBooks DataService for getting the deleting an Invoice
            ////Here we are accessing QuickBooks Online data
            //var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId + "/" + currentInvoice.CustomerId + "?methodx=delete");

            //var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            ////Sets up the Post Request bus does not actually send it out
            //IConsumerRequest consumerRequest = oSession.Request();
            //consumerRequest = consumerRequest.ForMethod("POST");
            //consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            //#region Generates the XML body of the Post call

            //var filter = "Name=" + ":EQUALS:" + currentInvoice.Client.DisplayName;

            //var clientXML = GetEntityList(currentBusinessAccount, "customers", filter);

            //var clientId = "";

            ////Splits the response XML into by line
            //string[] responseArray = clientXML.Split('<');

            ////Checks each line for the one containing the BaseURL
            //foreach (string s in responseArray)
            //{
            //    if (s.Contains(":Id>"))
            //    {
            //        responseArray = s.Split('>');
            //        clientId = responseArray[1];
            //        break;
            //    }
            //}

            //var body = QuickBooksXml.InvoiceXml(currentInvoice, clientId, Operation.Delete);

            //#endregion

            ////Signs the Request
            //consumerRequest = consumerRequest.SignWithToken();

            ////Sends the request with the body attached
            //consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes((string)body));

            ////Reads the response XML
            //var responseString = consumerRequest.ReadBody();
            #endregion
        }
        #endregion

        #endregion

        /// <summary>
        /// Creates the customer.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        private static string CreateCustomer(BusinessAccount currentBusinessAccount, string baseUrl, string displayName)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the creating an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(baseUrl + @"/resource/customer/v2/" + quickBooksSession.RealmId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            //Just creates a simple customer in their system as a place holder
            var body = QuickBooksXml.CustomerXml(displayName);

            //Signs the request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes(body));

            //Reads the response XML
            var responseString = consumerRequest.ReadBody();

            //Splits the response XML into by line
            string[] responseArray = responseString.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray.Where(s => s.Contains(":Id>")))
            {
                responseArray = s.Split('>');
                return responseArray[1];
            }

            return "";
        }

        #endregion

        #region Functions Used to Sync With QBO

        /// <summary>
        /// Merges the changes between our system and quick books.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="coreEntitiesContainer">The CoreEntitiesContainer</param>
        /// <param name="baseUrl"> </param>
        /// <returns></returns>
        private static Invoice MergeChangesWithQuickBooks(BusinessAccount currentBusinessAccount, Invoice currentInvoice, CoreEntitiesContainer coreEntitiesContainer, string baseUrl)
        {
            var response = GetEntityById(currentBusinessAccount, "invoice", baseUrl, currentInvoice.QuickBooksId);

            //Creates an Invoice to compare with instead of comparing to XML repeatedly
            var quickbooksInvoice = CreateInvoiceFromQuickbooksResponse(coreEntitiesContainer, response, currentBusinessAccount, baseUrl);

            currentInvoice = MergeInvoices(currentInvoice, quickbooksInvoice);

            //Save all changes made
            coreEntitiesContainer.SaveChanges();

            //This Invoice has been merged and is ready to be updated
            return currentInvoice;
        }

        /// <summary>
        /// Merges the invoices.
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="quickbooksInvoice">The quickbooks invoice.</param>
        /// <returns></returns>
        private static Invoice MergeInvoices(Invoice currentInvoice, Invoice quickbooksInvoice)
        {
            #region Invoice

            //This means that a property on the Invoice itself has changed
            if (currentInvoice.IsBillToLocationChanged || currentInvoice.IsDueDateChanged || currentInvoice.IsMemoChanged)
            {
                //Nothing is done here because of an issue with keeping changes the user made in QuickBooks
                //If they want to change the address that it is billed to, they can do it in our system
                //if (!currentInvoice.IsBillToLocationChanged)

                //If the DueDate has not changed in our system, take the one returned with the Invoice
                if (!currentInvoice.IsDueDateChanged)
                    currentInvoice.DueDate = quickbooksInvoice.DueDate;

                //If the Memo has not changed in our system, take the one returned with the Invoice
                if (!currentInvoice.IsMemoChanged)
                    currentInvoice.Memo = quickbooksInvoice.Memo;
            }

            #endregion

            #region SalesTerm

            //Since there is currently no way to edit SalesTerms in our System, this will only deal with the QuickBooksId as that is the only thing that changes
            if (!currentInvoice.IsSalesTermChanged)
            {
                //No changes were made to the sales term in our system so just take the one that was returned
                currentInvoice.SalesTerm = quickbooksInvoice.SalesTerm;
            }

            #endregion

            #region LineItems

            var items = currentInvoice.LineItems.Where(li => li.IsAmountChanged || li.IsDescriptionChanged);

            if (!items.Any())
                currentInvoice.LineItems = quickbooksInvoice.LineItems;

            #endregion

            return currentInvoice;
        }

        /// <summary>
        /// Creates the invoice from quickbooks response XML.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="response">The response.</param>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="baseUrl">The BaseUrl </param>
        /// <returns></returns>
        private static Invoice CreateInvoiceFromQuickbooksResponse(CoreEntitiesContainer coreEntitiesContainer, string response, BusinessAccount currentBusinessAccount, string baseUrl)
        {
            var splitResponse = response.Split('<');

            var qbInvoice = new Invoice();

            foreach (var item in splitResponse)
            {
                if (item.Contains("qbo:Id"))
                {
                    var split = item.Split('>');
                    qbInvoice.QuickBooksId = split[1];
                }
                if (item.Contains("Note>"))
                {
                    var split = item.Split('>');
                    qbInvoice.Memo = split[1];
                }
                if (item.Contains("DueDate>"))
                {
                    var split = item.Split('>');
                    //Now we split just the date part
                    var newDate = split[1].Split('-');

                    //Set the new Date in the database
                    qbInvoice.DueDate = new DateTime(Convert.ToInt32(newDate[0]), Convert.ToInt32(newDate[1]), Convert.ToInt32(newDate[2]));
                }

                #region SalesTerm

                if (item.Contains("qbo:SalesTermId"))
                {
                    var split = item.Split('>');
                    var exists = coreEntitiesContainer.SalesTerms.FirstOrDefault(st => st.QuickBooksId == split[1]);

                    //If it exists in our system, set the SalesTerm appropriately
                    if (exists != null)
                        qbInvoice.SalesTerm = exists;
                    //If not, make a new SalesTerm and set all the properties, then set the SalesTerm on the new Invoice
                    else
                    {
                        //If a SalesTerm with that Id doesnt exist, make it
                        //Get the SalesTerm with the matching Id from QBO
                        var salesTermsResponse = GetEntityById(currentBusinessAccount, "sales-term", baseUrl, split[1]);
                        var salesTermSplit = salesTermsResponse.Split('<');

                        var newSalesTerm = new SalesTerm();

                        #region Sets all the properties on the new SalesTerm

                        foreach (var sr in salesTermSplit)
                        {
                            if (sr.Contains("Id idDomain"))
                            {
                                var tempSplit = sr.Split('>');
                                newSalesTerm.QuickBooksId = tempSplit[1];
                            }

                            if (sr.Contains("SyncToken>"))
                            {
                                var tempSplit = sr.Split('>');
                                newSalesTerm.SyncToken = tempSplit[1];
                            }

                            if (sr.Contains("CreateTime>"))
                            {
                                var tempSplit = sr.Split('>');
                                newSalesTerm.CreateTime = tempSplit[1];
                            }

                            if (sr.Contains("LastUpdatedTime>"))
                            {
                                var tempSplit = sr.Split('>');
                                newSalesTerm.LastUpdatedTime = tempSplit[1];
                            }

                            if (sr.Contains("Name>"))
                            {
                                var tempSplit = sr.Split('>');
                                newSalesTerm.Name = tempSplit[1];
                            }

                            if (sr.Contains("DueDays>"))
                            {
                                var tempSplit = sr.Split('>');
                                if (!tempSplit[0].Contains('/'))
                                    newSalesTerm.DueDays = Convert.ToInt32(tempSplit[1]);
                            }
                        }

                        #endregion

                        //Save it to the BusinessAccount
                        currentBusinessAccount.SalesTerms.Add(newSalesTerm);

                        //Save the correct SalesTerm to the Invoice
                        qbInvoice.SalesTerm = newSalesTerm;

                        //Save the SalesTerm to the BusinessAccount as well so that it will be saved permanently
                        currentBusinessAccount.SalesTerms.Add(newSalesTerm);

                        coreEntitiesContainer.SaveChanges();
                    }
                }

                #endregion

            }

            #region LineItems

            splitResponse = Regex.Split(response, @"<Line>");

            foreach (var line in splitResponse)
            {
                var split = line.Split('<');
                var first = split.FirstOrDefault();

                //If the block of XML doesnt begin with "Desc" then we know it is not a Line item and thus can be skipped over in this section
                if (first != null && !first.Contains("Desc"))
                    continue;

                var newLineItem = new LineItem();

                foreach (var item in split)
                {
                    if (item.Contains("Desc"))
                    {
                        var tempSplit = item.Split('>');
                        newLineItem.Description = tempSplit[1];
                    }
                    if (item.Contains("Amount"))
                    {
                        var tempSplit = item.Split('>');
                        newLineItem.Amount = tempSplit[1];
                    }
                    if (item.Contains("ItemId"))
                    {
                        var tempSplit = item.Split('>');
                        newLineItem.QuickBooksId = tempSplit[1];
                    }
                }

                qbInvoice.LineItems.Add(newLineItem);
            }

            #endregion

            return qbInvoice;
        }

        /// <summary>
        /// Creates the sales terms from the quick books response.
        /// </summary>
        /// <param name="quickBooksSalesTermXml">The quick books sales term XML.</param>
        /// <returns></returns>
        public static IEnumerable<SalesTerm> CreateSalesTermsFromQuickBooksResponse(string quickBooksSalesTermXml)
        {
            var listOfSalesTerms = new ObservableCollection<SalesTerm>();

            var splitBySalesTerms = Regex.Split(quickBooksSalesTermXml, "<SalesTerm>");

            foreach (var salesTerm in splitBySalesTerms)
            {
                //Filters out the heading of the XML from being added as a SalesTerm
                if (salesTerm.Contains("intuit.com"))
                    continue;

                var newSalesTerm = new SalesTerm();

                var splitByLine = salesTerm.Split('<');

                foreach (var line in splitByLine)
                {
                    #region Set all Properties
                    if (line.Contains("Id idDomain"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.QuickBooksId = split[1];
                    }

                    if (line.Contains("SyncToken>"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.SyncToken = split[1];
                    }

                    if (line.Contains("CreateTime>"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.CreateTime = split[1];
                    }

                    if (line.Contains("LastUpdatedTime>"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.LastUpdatedTime = split[1];
                    }

                    if (line.Contains("Name>"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.Name = split[1];
                    }

                    if (line.Contains("DueDays>"))
                    {
                        var split = line.Split('>');
                        if (!split[0].Contains('/'))
                            newSalesTerm.DueDays = Convert.ToInt32(split[1]);
                    }
                    #endregion
                }
                listOfSalesTerms.Add(newSalesTerm);
            }

            return listOfSalesTerms;
        }

        /// <summary>
        /// Creates the invoices from quick books response after GetEntityList("invoices").
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="quickBooksInvoiceXml">The quick books invoice XML.</param>
        public static void CreateInvoicesFromQuickBooksResponse(BusinessAccount currentBusinessAccount, string baseUrl, string quickBooksInvoiceXml)
        {
            var splitByInvoices = Regex.Split(quickBooksInvoiceXml, "<Invoice>");

            var coreEntitiesContainer = new CoreEntitiesContainer();

            foreach (var invoice in splitByInvoices)
            {
                //Filters out the heading of the XML from being added as an Invoice
                if (invoice.Contains("intuit.com"))
                    continue;
                
                //Creates a new Invoice based on the XML sent to it
                var newInvoice = CreateInvoiceFromQuickbooksResponse(coreEntitiesContainer, invoice, currentBusinessAccount, baseUrl);
                
                //The QuickBooksId of the invoice in question
                var idToCheck = newInvoice.QuickBooksId;

                //Check to see if an invoice with that Id already exists in our system
                var existantInvoice = coreEntitiesContainer.Invoices.FirstOrDefault(i => i.QuickBooksId == idToCheck);

                //If the invoice does exists, merge it. Otherwise, add it to the list of InvoicesToAdd.
                if (existantInvoice != null)
                {
                    //Merge Here
                    MergeInvoices(existantInvoice, newInvoice);
                }
                else
                {
                    //Add invoice to the database
                    coreEntitiesContainer.Invoices.AddObject(newInvoice);                
                }
            }

            coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region Azure Tables (Add, Remove, GetList)

        /// <summary>
        /// Adds the item to the table only if the item does not already exist in the table.
        /// If it does exist, the method will update that item to the current details
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="changeType">The Type of Change required</param>
        public static void AddUpdateDeleteToTable(Invoice currentInvoice, Operation changeType)
        {
            var newItem = new InvoiceTableDataModel(currentInvoice.Id, changeType.ToString());

            //TODO: Check that this DataConnectionString is right
            var storageAccount = CloudStorageAccount.Parse(OauthConstants.StorageConnectionString);

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            // Create the table if there is not already a table with the name of the Business Account
            var tableClient = storageAccount.CreateCloudTableClient();

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "t" + currentInvoice.BusinessAccount.Id.ToString().Replace("-", "");

            try
            {
                tableClient.CreateTableIfNotExist(tableName);
            }
            catch { }

            //Query that checks to see if and object with the same InvoiceId exists in the Table specified
            var existsQuery = serviceContext.CreateQuery<InvoiceTableDataModel>(tableName).Where(
                e => e.InvoiceId == newItem.InvoiceId);

            //Gets the first and hopefully only item in the Table that matches
            var existingObject = existsQuery.FirstOrDefault();

            //If an item doesnt exist yet, add that object
            if (existingObject == null)
            {
                //Adds the new object to the Table
                serviceContext.AddObject(tableName, newItem);
            }
            //If an item exists already, update the item and save the changes
            else
            {
                //No need to update the InvoiceId becuase it should be the same
                //Update the change type because it could change from Update to Delete or vicea versa
                existingObject.ChangeType = newItem.ChangeType;

                serviceContext.UpdateObject(existingObject);
            }

            //Saves the Tables
            serviceContext.SaveChangesWithRetries();
        }

        /// <summary>
        /// Removes the object from the Azure table.
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        public static void RemoveFromTable(Invoice currentInvoice)
        {
            //Check that this DataConnectionString is right
            var storageAccount = CloudStorageAccount.Parse(OauthConstants.StorageConnectionString);

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            var newItem = new InvoiceTableDataModel (currentInvoice.Id);

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "t" + currentInvoice.BusinessAccount.Id.ToString().Replace("-", "");

            //Query that checks to see if and object with the same InvoiceId exists in the Table specified
            var existsQuery = serviceContext.CreateQuery<InvoiceTableDataModel>(tableName).Where(
                e => e.PartitionKey == newItem.InvoiceId.ToString());

            //Gets the first and hopefully only item in the Table that matches
            var existingObject = existsQuery.FirstOrDefault();

            if (existingObject != null)
                serviceContext.DeleteObject(existingObject);

            serviceContext.SaveChangesWithRetries();
        }

        /// <summary>
        /// Gets the list of objects from the specified table.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns></returns>
        public static IEnumerable<InvoiceTableDataModel> GetListFromTables(BusinessAccount currentBusinessAccount)
        {
            //Check that this DataConnectionString is right
            var storageAccount = CloudStorageAccount.Parse(OauthConstants.StorageConnectionString);

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            //Table Names must start with a letter. They also must be alphanumeric. http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
            var tableName = "t" + currentBusinessAccount.Id.ToString().Replace("-", "");

            //Gets all objects from the Azure table specified and returns the result
            return serviceContext.CreateQuery<InvoiceTableDataModel>(tableName);
        }

        #endregion
    }
}
