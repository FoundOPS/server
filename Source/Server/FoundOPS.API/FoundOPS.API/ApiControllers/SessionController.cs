using FoundOPS.Api.Models;
using FoundOps.Common.Tools;
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
using System.Web.Http;
using System.Web.Security;

namespace FoundOps.Api.ApiControllers
{
    /// <summary>
    /// An API controller which allows users to authenticate.
    /// </summary>
    public class SessionController : ApiController
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        private readonly CoreEntitiesMembershipProvider _coreEntitiesMembershipProvider;
        public SessionController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
            _coreEntitiesMembershipProvider = new CoreEntitiesMembershipProvider(_coreEntitiesContainer);
        }

        #region Authentication

        /// <summary>
        /// Logs in the specified email address.
        /// TODO: login attempt tracking.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="pass">The password.</param>
        /// <param name="redirectUrl">(Optional) If specified, it will redirect here on success.</param>
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage Login(string email, string pass, string redirectUrl = "")
        {
            if (_coreEntitiesMembershipProvider.ValidateUser(email, pass))
            {
                FormsAuthentication.SetAuthCookie(email, true);

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    var response = Request.CreateResponse(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri(redirectUrl);
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.Accepted, true);
            }

            //return status code accepted to ajax jquery can get the false response
            return Request.CreateResponse(HttpStatusCode.Accepted, false);
        }

        /// <summary>
        /// Log Out the current user.
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public bool LogOut()
        {
            FormsAuthentication.SignOut();
            return true;
        }

        #endregion

        #region User Information

        /// <summary>
        /// Check if the user is logged on
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public bool Authenticated()
        {
            return _coreEntitiesContainer.CurrentUserAccount() != null;
        }

        /// <summary>
        /// Gets the session information for the user
        /// </summary>
        /// <param name="isMobile">Whether or not the user is on a mobile device. This affects urls.</param>
        [AcceptVerbs("GET", "POST")]
        public JObject Get(bool isMobile)
        {
            var user = _coreEntitiesContainer.CurrentUserAccount().Include(ua => ua.RoleMembership)
                .Include("RoleMembership.Blocks").Include("RoleMembership.OwnerBusinessAccount").First();

            //apply timezone

            //Load all of the party images for the owner's of roles, and the current user account
            var partyIds = user.RoleMembership.Select(r => r.OwnerBusinessAccountId).Distinct()
                .Union(new[] { new Guid?(user.Id) }).ToArray();

            var partyImages = _coreEntitiesContainer.Files.OfType<PartyImage>()
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

                //Set the business's logo
                if (role.OwnerBusinessAccount.PartyImage != null)
                {
                    jRole.businessLogoUrl = partyImageUrls[role.OwnerBusinessAccount.PartyImage.Id];
                }

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

            return config;
        }

        /// <summary>
        /// Get the column configurations of a user for a role.
        /// </summary>
        /// <param name="roleId">The role</param>
        public IEnumerable<ColumnConfiguration> GetColumnConfigurations(Guid roleId)
        {
            var userAccount = _coreEntitiesContainer.CurrentUserAccount().First();

            List<ColumnConfiguration> columnConfigurations;

            try
            {
                columnConfigurations = SerializationTools.Deserialize<List<ColumnConfiguration>>(userAccount.ColumnConfigurations);
            }
            catch
            {
                columnConfigurations = new List<ColumnConfiguration>();
                userAccount.ColumnConfigurations = SerializationTools.Serialize(columnConfigurations);
                _coreEntitiesContainer.SaveChanges();
            }

            return columnConfigurations.Where(c => c.RoleId == roleId);
        }

        /// <summary>
        /// Update the column configurations for a user for a role.
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="columnConfigurations">The new column configurations</param>
        public HttpResponseMessage UpdateColumnConfigurations(Guid roleId, List<ColumnConfiguration> columnConfigurations)
        {
            var userAccount = _coreEntitiesContainer.CurrentUserAccount().First();

            var configurations = new List<ColumnConfiguration>();
            if (userAccount.ColumnConfigurations != null)
                configurations = SerializationTools.Deserialize<List<ColumnConfiguration>>(userAccount.ColumnConfigurations);

            //remove old configurations for this role
            configurations.RemoveAll(c => c.RoleId == roleId);

            foreach (var config in columnConfigurations)
            {
                config.RoleId = roleId;
            }

            //add the new ones
            configurations.AddRange(columnConfigurations);

            userAccount.ColumnConfigurations = SerializationTools.Serialize(configurations);
            _coreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        #endregion
    }
}