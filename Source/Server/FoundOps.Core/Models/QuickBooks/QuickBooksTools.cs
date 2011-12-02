using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
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

            public static readonly string GrantUrl = "http://localhost:31820/QuickBooks/OAuthGrantLogin";

            public static readonly string ConsumerKey = ConfigurationManager.AppSettings["consumerKey"];
            public static readonly string ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"];

            public static readonly string OauthCallbackUrl = "http://localhost:31820/QuickBooks/OAuthGrantHandler";
        }

        #endregion

        #region OAuth Session and Access Token

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

            //Generates the OAuth session based on the constants for our IntuitAnywhere App and the ConsumerContext created above 
            OAuthSession oSession = new OAuthSession(consumerContext, OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlRequestToken,
                                                     OauthConstants.AuthorizeUrl,
                                                     OauthConstants.IdFedOAuthBaseUrl + OauthConstants.UrlAccessToken);

            oSession.ConsumerContext.UseHeaderForOAuthParameters = true;

            //Access Token is generated from storage here and saved into the OauthSession
            oSession.AccessToken = new TokenBase
            {
                Token = currentBusinessAccount.QuickBooksAccessToken,
                ConsumerKey = OauthConstants.ConsumerKey,
                TokenSecret = currentBusinessAccount.QuickBooksAccessTokenSecret
            };
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
        /// Exchanges the request token attained earlier for the access token needed to make calls to QuickBooks
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="coreEntitiesContainer">The current CoreEntitiesContainer</param>
        public static void GetAccessToken(BusinessAccount currentBusinessAccount, CoreEntitiesContainer coreEntitiesContainer)
        {
            //Deserializes QuicBooksSessionXml from the Database to a quickBooksSession Class
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //Creates the OAuth Session
            var clientSession = CreateOAuthSession();

            //The AccessToken is attained by exchanging the request token and the OAuthVerifier that we attained earlier
            IToken accessToken = clientSession.ExchangeRequestTokenForAccessToken(quickBooksSession.Token, quickBooksSession.OAuthVerifier);

            //Saving the Token to private storage
            currentBusinessAccount.QuickBooksAccessToken = accessToken.Token;
            currentBusinessAccount.QuickBooksAccessTokenSecret = accessToken.TokenSecret;

            coreEntitiesContainer.SaveChanges();
        }

        #endregion

        #region Base Url, User Info and Entity List

        /// <summary>
        /// Finds the BaseURL for the current QuickBooks user
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        public static void GetBaseUrl(BusinessAccount currentBusinessAccount, CoreEntitiesContainer coreEntitiesContainer)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

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
                    quickBooksSession.BaseUrl = responseArray[1];
                    break;
                }
            }

            //Ensures that the BaseURL is saved for later use
            currentBusinessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);
            coreEntitiesContainer.SaveChanges();
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
        /// <param name="filter">Specifies the filter if one is provided</param>
        private static string GetEntityList(BusinessAccount currentBusinessAccount, string entityType, string filter = null)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the List of Invoices
            //In this case we are only looking for the first 25 of them
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + @"/resource/" + entityType + "/v2/" + quickBooksSession.RealmId);

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
        /// Gets the invoice by id.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="entityType">The entity type for this call to the function</param>
        /// <param name="id">The specified Filter</param>
        /// <returns></returns>
        public static string GetEntityById(BusinessAccount currentBusinessAccount, string entityType, string id = null)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks Data Service for getting the BaseURL
            var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + "/resource/" + entityType + "/v2" + quickBooksSession.RealmId + id);

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

        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to create the Invoice that is specifed
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        public static string CreateNewInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the creating an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            #region Generates the XML body of the Post call

            #region Gets the ClientId

            var filter = "Name=" + ":EQUALS:" + currentInvoice.Client.DisplayName;

            var clientXML = GetEntityList(currentBusinessAccount, "customers", filter);

            var clientId = "";

            //Splits the response XML into by line
            string[] responseArray = clientXML.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray)
            {
                if (s.Contains(":Id>"))
                {
                    responseArray = s.Split('>');
                    clientId = responseArray[1];
                    break;
                }
            }

            #endregion

            var body = QuickBooksXml.InvoiceXml(currentInvoice, clientId, Operation.Create);

            #endregion

            //Signs the request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes((string)body));

            //Reads the response XML
            return consumerRequest.ReadBody();
        }

        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to update the specified invoice
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="coreEntitiesContainer">The CoreEntitesContainer</param>
        public static void UpdateInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice, CoreEntitiesContainer coreEntitiesContainer)
        {
            currentInvoice = MergeChangesWithQuickBooks(currentBusinessAccount, currentInvoice, coreEntitiesContainer);

            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the updating an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId + "/" + currentInvoice.CustomerId);

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            #region Generates the XML body of the Post call

            var filter = "Name=" + ":EQUALS:" + currentInvoice.Client.DisplayName;

            var clientXML = GetEntityList(currentBusinessAccount, "customers", filter);

            var clientId = "";

            //Splits the response XML into by line
            string[] responseArray = clientXML.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray)
            {
                if (s.Contains(":Id>"))
                {
                    responseArray = s.Split('>');
                    clientId = responseArray[1];
                    break;
                }
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

        /// <summary>
        /// Makes a call to the QuickBooks Online Data Services to delete the specified invoice
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        public static void DeleteInvoice(BusinessAccount currentBusinessAccount, Invoice currentInvoice)
        {
            var quickBooksSession = SerializationTools.Deserialize<QuickBooksSession>(currentBusinessAccount.QuickBooksSessionXml);

            //URL for the QuickBooks DataService for getting the deleting an Invoice
            //Here we are accessing QuickBooks Online data
            var serviceEndPoint = String.Format(quickBooksSession.BaseUrl + @"/resource/invoice/v2/" + quickBooksSession.RealmId + "/" + currentInvoice.CustomerId + "?methodx=delete");

            var oSession = CreateOAuthSessionAndAccessToken(currentBusinessAccount);

            //Sets up the Post Request bus does not actually send it out
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = consumerRequest.ForMethod("POST");
            consumerRequest = consumerRequest.ForUri(new Uri(serviceEndPoint));

            #region Generates the XML body of the Post call

            var filter = "Name=" + ":EQUALS:" + currentInvoice.Client.DisplayName;

            var clientXML = GetEntityList(currentBusinessAccount, "customers", filter);

            var clientId = "";

            //Splits the response XML into by line
            string[] responseArray = clientXML.Split('<');

            //Checks each line for the one containing the BaseURL
            foreach (string s in responseArray)
            {
                if (s.Contains(":Id>"))
                {
                    responseArray = s.Split('>');
                    clientId = responseArray[1];
                    break;
                }
            }

            var body = QuickBooksXml.InvoiceXml(currentInvoice, clientId, Operation.Delete);

            #endregion

            //Signs the Request
            consumerRequest = consumerRequest.SignWithToken();

            //Sends the request with the body attached
            consumerRequest.Post().WithRawContentType("application/xml").WithRawContent(Encoding.ASCII.GetBytes((string)body));

            //Reads the response XML
            var responseString = consumerRequest.ReadBody();
        }

        #endregion

        /// <summary>
        /// Merges the changes between our system and quick books.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="coreEntitiesContainer">The CoreEntitiesContainer</param>
        /// <returns></returns>
        private static Invoice MergeChangesWithQuickBooks(BusinessAccount currentBusinessAccount, Invoice currentInvoice, CoreEntitiesContainer coreEntitiesContainer)
        {
            var response = GetEntityById(currentBusinessAccount, "invoice", currentInvoice.Id.ToString());

            //Creates an Invoice to compare with instead of comparing to XML repeatedly
            var quickbooksInvoice = CreateInvoiceFromQuickbooksResponse(response);

            #region Invoice

            //This means that a property on the Invoice itself has changed
            if (currentInvoice.IsBillToLocationChanged || currentInvoice.IsDueDateChanged || currentInvoice.IsMemoChanged)
            {
                //var response = GetInvoiceById(currentBusinessAccount, currentInvoice);
                string[] responseArray = response.Split('<');

                //Checks each line for the one containing the BaseURL
                foreach (string s in responseArray)
                {
                    if (!currentInvoice.IsBillToLocationChanged)
                    {
                        //Nothing is done here because of an issue with keeping changes the user made in QuickBooks
                        //If they want to change the address that it is billed to, they can do it in our system
                    }
                    if (!currentInvoice.IsDueDateChanged)
                    {
                        //Comes in as "DueDate>2011-08-19"
                        var tempSplit = s.Split('>');

                        //Now we split just the date part
                        var newDate = tempSplit[1].Split('-');

                        //Set the new Date in the database
                        currentInvoice.DueDate = new DateTime(Convert.ToInt32(newDate[0]), Convert.ToInt32(newDate[1]), Convert.ToInt32(newDate[2]));
                    }
                    if (!currentInvoice.IsMemoChanged)
                    {
                        //Comes in as "Memo>This is a test"
                        var tempSplit = s.Split('>');

                        //Just saves the memo portion
                        currentInvoice.Memo = tempSplit[1];
                    }
                }
            }

            #endregion

            #region SalesTerm

            //Since there is currently no way to edit SalesTerms in our System, this will only deal with the QuickBooksId as that is the only thing that changes
            if (!currentInvoice.IsSalesTermChanged)
            {
                string[] responseArray = response.Split('<');

                //Checks each line for the one containing the SalesTermId
                foreach (string s in responseArray.Where(s => s.Contains("qbo:SalesTermId>")))
                {
                    responseArray = s.Split('>');
                    //Set the Invoice to the SalesTerm with that Id
                    var salesTerm = coreEntitiesContainer.SalesTerms.Where(st => st.QuickBooksId == responseArray[1]).FirstOrDefault();

                    //If a SalesTerm was found, set it to the SalesTerm for the current Invoice
                    if (salesTerm != null)
                    {
                        currentInvoice.SalesTerm = salesTerm;
                        break;
                    }
                }

                //If a SalesTerm with that Id doesnt exist, make it
                //Get the SalesTerm with the matching Id from QBO
                var salesTermsResponse = GetEntityById(currentBusinessAccount, "sales-term", responseArray[1]);

                var splitResponse = salesTermsResponse.Split('<');

                var newSalesTerm = new SalesTerm();

                #region Sets all the properties on the new SalesTerm

                foreach (var sr in splitResponse)
                {
                    if (sr.Contains("qbo:Id"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.QuickBooksId = split[1];
                    }

                    if (sr.Contains("SyncToken"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.SyncToken = split[1];
                    }

                    if (sr.Contains("CreateTime"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.CreateTime = split[1];
                    }

                    if (sr.Contains("LastUpdatedTime"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.LastUpdatedTime = split[1];
                    }

                    if (sr.Contains("Name"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.Name = split[1];
                    }

                    if (sr.Contains("DueDays"))
                    {
                        var split = sr.Split('>');
                        newSalesTerm.DueDays = Convert.ToInt32(split[1]);
                    }
                }

                #endregion

                //Save it to the BusinessAccount
                currentBusinessAccount.SalesTerms.Add(newSalesTerm);

                //Save the correct SalesTerm to the Invoice
                currentInvoice.SalesTerm = newSalesTerm;
            }

            #endregion

            #region LineItems


            foreach (var lineItem in currentInvoice.LineItems)
            {
                //Check the invoice from qbo for the Id and make sure it matches one of the ids we have in our system
                if (response.Contains(String.Format("<ItemId>" + lineItem.QuickBooksId + "</ItemId>")))
                {
                    //Splits to get each line item
                    var splitLineResponse = Regex.Split(response, "<Line>");

                    foreach (var item in splitLineResponse)
                    {
                        //Gets us the Item Id of the LineItem
                        var splitResponse = Regex.Split(item, "<ItemId>");

                        //Isolates the itemId
                        splitResponse = splitResponse[1].Split('<');

                        var correctLineItem = currentInvoice.LineItems.Where(li => li.QuickBooksId == splitResponse[0]).FirstOrDefault();

                        //If a line item was found, check that no properties have changed in our system and merge changes accordingly
                        if (correctLineItem != null)
                        {
                            #region Check for changes in amount
                            
                            if (!lineItem.IsAmountChanged)
                            {
                                var splitAmountResponse = item.Split('<');
                                foreach (var attribute in splitAmountResponse)
                                {
                                    if (attribute.Contains("Amount>"))
                                    {
                                        var splitAttribute = attribute.Split('>');
                                        correctLineItem.Amount = splitAttribute[1];
                                    }
                                }
                            }

                            #endregion

                            #region Check for changes in the description

                            if (!lineItem.IsDescriptionChanged)
                            {
                                var splitDescriptionResponse = item.Split('<');
                                foreach (var attribute in splitDescriptionResponse)
                                {
                                    if (attribute.Contains("Desc>"))
                                    {
                                        var splitAttribute = attribute.Split('>');
                                        correctLineItem.Amount = splitAttribute[1];
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }


            #endregion

            //Save all changes made
            coreEntitiesContainer.SaveChanges();

            //This Invoice has been merged and is ready to be updated
            return currentInvoice;
        }

        private static Invoice CreateInvoiceFromQuickbooksResponse(string response)
        {
            var splitResponse = response.Split('<');

            var qbInvoice = new Invoice();

            foreach (var item in splitResponse)
            {
                if(item.Contains("qbo:Id"))
                {
                    var split = item.Split('>');
                    qbInvoice.QuickBooksId = split[1];
                }
                if (item.Contains("Note"))
                {
                    var split = item.Split('>');
                    qbInvoice.Memo = split[1];
                }
                if (item.Contains("DueDate"))
                {
                    var split = item.Split('>');
                    //Now we split just the date part
                    var newDate = split[1].Split('-');

                    //Set the new Date in the database
                    qbInvoice.DueDate = new DateTime(Convert.ToInt32(newDate[0]), Convert.ToInt32(newDate[1]), Convert.ToInt32(newDate[2]));
                }
                if (item.Contains("qbo:SalesTermId"))
                {
                    var split = item.Split('>');
                    qbInvoice.SalesTerm.QuickBooksId = split[1];
                }

                //still need to figure out how to do this for line items......................................
            }


            return qbInvoice;
        }


        /// <summary>
        /// Creates the authorization URL for QuickBooks based on the OAuth session
        /// </summary>
        /// <param name="callbackUrl">The callback URL.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="currentBusinessAccount">The current bunsiness account.</param>
        /// <returns> returns the authorization URL. </returns>
        public static string GetAuthorizationUrl(string callbackUrl, string roleId, BusinessAccount currentBusinessAccount)
        {
            var session = CreateOAuthSession();

            //Creates a new instance of the QuickBooksSession class to be later serialized into the Database
            var quickBooksSession = new QuickBooksSession { Token = (TokenBase)session.GetRequestToken() };

            //Serializing the QuickBooks Session class into the Database
            currentBusinessAccount.QuickBooksSessionXml = SerializationTools.Serialize(quickBooksSession);

            //Creates the Authorized URL for QuickBooks that is based on the OAuth session created above
            var authUrl = OauthConstants.AuthorizeUrl + "?oauth_token=" + quickBooksSession.Token.Token + "&oauth_callback=" + UriUtility.UrlEncode(callbackUrl);

            return authUrl;
        }

        #region Azure Tables

        /// <summary>
        /// Adds the item to the table only if the item does not already exist in the table.
        /// If it does exist, the method will update that item to the current details
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="changeType">The Type of Change required</param>
        public static void AddUpdateDeleteToTable(Invoice currentInvoice, Operation changeType)
        {
            var newItem = new InvoiceTableDataModel { InvoiceId = currentInvoice.Id, ChangeType = changeType };

            //Check that this DataConnectionString is right
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            // Create the table if there is not already a table with the name of the Business Account
            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(currentInvoice.BusinessAccount.Name);

            //Query that checks to see if and object with the same InvoiceId exists in the Table specified
            var existsQuery = serviceContext.CreateQuery<InvoiceTableDataModel>(currentInvoice.BusinessAccount.Name).Where(
                e => e.InvoiceId == newItem.InvoiceId);

            //Gets the first and hopefully only item in the Table that matches
            var existingObject = existsQuery.FirstOrDefault();

            //If an item doesnt exist yet, add that object
            if (existingObject == null)
            {
                //Adds the new object to the Table
                serviceContext.AddObject(currentInvoice.BusinessAccount.Name, newItem);
            }
            //If an item exists already, update the item and save the changes
            else
            {
                //No need to update the InvoiceId becuase it should be the same
                //Update the change type becuase it could change from Update to Delete or vicea versa
                existingObject.ChangeType = newItem.ChangeType;

                serviceContext.UpdateObject(existingObject);
            }

            //Saves the Tables
            serviceContext.SaveChanges();
        }

        /// <summary>
        /// Removes the object from the Azure table.
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        public static void RemoveFromTable(Invoice currentInvoice)
        {
            //Check that this DataConnectionString is right
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            var newItem = new InvoiceTableDataModel { InvoiceId = currentInvoice.Id };

            //Query that checks to see if and object with the same InvoiceId exists in the Table specified
            var existsQuery = serviceContext.CreateQuery<InvoiceTableDataModel>(currentInvoice.BusinessAccount.Name).Where(
                e => e.InvoiceId == newItem.InvoiceId);

            //Gets the first and hopefully only item in the Table that matches
            var existingObject = existsQuery.FirstOrDefault();

            if (existingObject != null)
                serviceContext.DeleteObject(existingObject);
        }

        /// <summary>
        /// Gets the list of objects from the specified table.
        /// </summary>
        /// <param name="currentBusinessAccount">The current business account.</param>
        /// <returns></returns>
        public static IQueryable<InvoiceTableDataModel> GetListFromTables(BusinessAccount currentBusinessAccount)
        {
            //Check that this DataConnectionString is right
            var storageAccount =
                CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));

            var serviceContext = new InvoiceTableDataServiceContext(storageAccount.TableEndpoint.ToString(),
                                                                    storageAccount.Credentials);

            //Gets all objects from the Azure table specified and returns the result
            return serviceContext.CreateQuery<InvoiceTableDataModel>(currentBusinessAccount.Name);
        }

        #endregion
    }
}
