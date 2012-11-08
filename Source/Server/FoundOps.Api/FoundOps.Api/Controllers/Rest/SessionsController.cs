using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;
using FoundOps.Api.Tools;
using FoundOps.Core.Models;
using FoundOps.Core.Models.Authentication;
using FoundOps.Core.Models.Azure;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Security;

namespace FoundOps.Api.Controllers.Rest
{
    public class SessionsController : BaseApiController
    {
        private readonly CoreEntitiesMembershipProvider _coreEntitiesMembershipProvider;
        public SessionsController()
        {
            _coreEntitiesMembershipProvider = new CoreEntitiesMembershipProvider(CoreEntitiesContainer);
        }

        // GET api/session
        /// <summary>
        /// Gets the session information for the user
        /// It will default to returning true if authenticated, and false if not authenticated
        /// Accepts header: OpsDetails: "True". This will return the entire session object, or throw an unauthorized exception
        /// </summary>
        /// <param name="isMobile">(Optional) Whether or not the user is on a mobile devic which affects urls. Defaults to false</param>
        public HttpResponseMessage Get(bool isMobile = false)
        {
            //if the user is not expecting a detailed response, return whether the user is authenticated currently or not
            if (!Request.Headers.Contains("ops-details") || !Request.Headers.GetValues("ops-details").Contains("true"))
            {
                var isAuthenticated = !string.IsNullOrEmpty(AuthenticationLogic.CurrentUsersEmail());
                return Request.CreateResponse(HttpStatusCode.Accepted, isAuthenticated);
            }

            Request.CheckAuthentication();

            var currentUsersEmail = AuthenticationLogic.CurrentUsersEmail();

            const string sql = @"SELECT ua.*, r.*, ba.*, b.* FROM dbo.Parties_UserAccount ua
                        INNER JOIN dbo.Roles r
                        ON r.Id IN (
						                        SELECT RoleMembership_Id FROM dbo.PartyRole 
						                        WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
				                           ) 
                        AND ua.EmailAddress = @emailAddress
                        INNER JOIN dbo.Parties_BusinessAccount ba
                        ON r.OwnerBusinessAccountId = ba.Id
                        INNER JOIN dbo.Blocks b
                        ON b.Id IN (
						                        SELECT Blocks_Id FROM dbo.RoleBlock 
						                        WHERE Roles_Id IN (
											                        SELECT RoleMembership_Id FROM dbo.PartyRole 
											                        WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
										                          ) 
						                        AND r.Id = dbo.RoleBlock.Roles_Id
					                        )
                        AND r.Id IN (
						                        SELECT RoleMembership_Id FROM dbo.PartyRole 
						                        WHERE MemberParties_Id = (SELECT Id FROM dbo.Parties_UserAccount WHERE EmailAddress = @emailAddress)
					                        )";

            UserAccount currentUser = null;
            Role currentRole = null;

            UserAccount user;

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@emailAddress", currentUsersEmail);

                user = conn.Query<UserAccount, Role, BusinessAccount, Block, UserAccount>(sql, (userAccount, role, businessAccount, block) =>
                {
                    if (currentUser == null)
                    {
                        currentUser = userAccount;
                    }
                    if (currentRole == null || currentRole.Id != role.Id)
                    {
                        currentRole = role;
                        currentRole.OwnerBusinessAccount = businessAccount;
                        var test = businessAccount.Name;
                        currentUser.RoleMembership.Add(role);
                    }
                    currentRole.Blocks.Add(block);

                    return userAccount;
                }, new { emailAddress = currentUsersEmail }).FirstOrDefault();

                conn.Close();
            }

            //var user = CoreEntitiesContainer.CurrentUserAccount().Include(ua => ua.RoleMembership)
            //    .Include("RoleMembership.Blocks").Include("RoleMembership.OwnerBusinessAccount").First();

            //apply timezone

            //Load all of the party images for the owner's of roles, and the current user account
            var partyIds = user.RoleMembership.Select(r => r.OwnerBusinessAccountId).Distinct()
                .Union(new[] { new Guid?(user.Id) }).ToArray();

            var partyImages = CoreEntitiesContainer.Files.OfType<PartyImage>()
                .Where(pi => partyIds.Contains(pi.Id)).Distinct().ToList();

            //Go through each party image and get the url with the shared access key
            var partyImageUrls = new Dictionary<Guid, string>();
            foreach (var partyImage in partyImages)
            {
                var imageUrl = partyImage.RawUrl + AzureServerHelpers.GetBlobUrlHelper(partyImage.OwnerParty.Id, partyImage.Id);
                partyImageUrls.Add(partyImage.OwnerParty.Id, imageUrl);
            }

            var roles = user.RoleMembership.Distinct().OrderBy(r => r.OwnerBusinessAccount.DisplayName);
            var sections = roles.SelectMany(r => r.Blocks).Where(s => !s.HideFromNavigation).Distinct().OrderBy(b => b.Name).ToList();

            //remove all silverlight sections if this is a mobile session
            if (isMobile)
                sections.RemoveAll(s => s.IsSilverlight.HasValue && s.IsSilverlight.Value);

            dynamic config = new JObject();
            config.name = user.FirstName + " " + user.LastName;
            config.email = user.EmailAddress;
            config.settingsUrl = "#view/personalSettings.html";
            config.logOutUrl = isMobile ? "#view/logout.html" : "../Account/LogOut";

            config.avatarUrl = user.PartyImage != null
                                   ? partyImageUrls[user.PartyImage.Id]
                                   : "img/emptyPerson.png";

            //the user's timezone offset in minutes
            config.userTimeZoneMinutes = user.UserTimeZoneOffset.TotalMinutes;

            var jRoles = new List<JObject>();
            //Go through each of the user's roles
            foreach (var role in roles)
            {
                dynamic jRole = new JObject();
                jRole.id = role.Id;
                jRole.name = role.OwnerBusinessAccount.Name;
                jRole.type = role.RoleType.ToString();

                //TODO: Take a good look at this and determine its future?
                //Set the business's logo
                //if (role.OwnerBusinessAccount.PartyImage != null)
                //{
                //    jRole.businessLogoUrl = partyImageUrls[role.OwnerBusinessAccount.PartyImage.Id];
                //}

                var availableSections = role.Blocks.Where(s => !s.HideFromNavigation).OrderBy(r => r.Name).ToList();
                //remove all silverlight sections if this is a mobile session
                if (isMobile)
                    availableSections.RemoveAll(s => s.IsSilverlight.HasValue && s.IsSilverlight.Value);

                var availableSectionNames = availableSections.Select(b => b.Name);

                //Add the available sections's names for the roles
                jRole.sections = new JArray(availableSectionNames);

                jRoles.Add(jRole);
            }
            config.roles = new JArray(jRoles);

            var jSections = new List<JObject>();
            foreach (var section in sections)
            {
                dynamic jSection = new JObject();
                jSection.name = section.Name;
                jSection.color = section.Color;

                if (!string.IsNullOrEmpty(section.Url))
                {
                    jSection.url = section.Url;
                }

                jSection.iconUrl = section.IconUrl;
                jSection.hoverIconUrl = section.HoverIconUrl;

                if (section.IsSilverlight.HasValue && section.IsSilverlight.Value)
                {
                    jSection.isSilverlight = true;
                }
                jSections.Add(jSection);
            }
            config.sections = new JArray(jSections);

            return Request.CreateResponse(HttpStatusCode.OK, (JObject)config);
        }

        // PUT api/session/email=me@email.com&pass=myPass
        /// <summary>
        /// Log the user in
        /// </summary>
        /// <returns>True if succesful, false if not succesful</returns>
        public bool Put(string email, string pass)
        {
            //try to authenticate
            if (_coreEntitiesMembershipProvider.ValidateUser(email, pass))
            {
                //succesfully authenticated
                FormsAuthentication.SetAuthCookie(email, true);
                return true;
            }

            //did not succesfuly authenticate
            return false;
        }

        // DELETE api/session/5
        /// <summary>
        /// Log out the user
        /// </summary>
        public void Delete()
        {
            FormsAuthentication.SignOut();
        }
    }
}
