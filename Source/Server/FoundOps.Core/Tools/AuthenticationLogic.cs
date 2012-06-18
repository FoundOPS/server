using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Data.Objects;
using System.Security.Authentication;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Core.Tools
{
    public static class AuthenticationLogic
    {
        #region Helpers

        /// <summary>
        /// Gets a random 8 character uppercase alphanumeric code.
        /// </summary>
        /// <returns></returns>
        public static string GetCode()
        {
            var builder = new StringBuilder();
            var random = new Random();
            builder.Clear();
            for (var i = 0; i < 8; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            var code = builder.ToString().ToUpper();
            return code;
        }

        #endregion

        #region Roles

        /// <summary>
        /// Returns the Administrator roles of rolesToFilter
        /// </summary>
        /// <param name='rolesToFilter'>The roles that you want to filter</param>
        public static IQueryable<Role> AdministratorRoles(IQueryable<Role> rolesToFilter)
        {
            //TODO: Setup better Administrator role logic, when roles develop
            //TODO: Add logic for User administrators
            var filteredRoles = rolesToFilter.Where(r => r.RoleTypeInt == (int)RoleType.Administrator);

            return filteredRoles;
        }

        /// <summary>
        /// Gets the roles the current user has access to
        /// NOTE: Role includes MemberParties, and OwnerParty
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown if user is not logged in</exception>
        public static IQueryable<Role> RolesCurrentUserHasAccessTo(this CoreEntitiesContainer coreEntitiesContainer)
        {
            var roles = (ObjectQuery<Role>) //cast as an ObjectQuery to defer Blocks and OwnerParty include until last minute
                        from user in CurrentUserAccountQueryable(coreEntitiesContainer) //there will only be one
                        from role in coreEntitiesContainer.Roles.Include("OwnerParty").Include("MemberParties")
                        where role.OwnerPartyId == user.Id || role.MemberParties.Any(a => a.Id == user.Id)
                        select role;

            return roles;
        }

        #region OwnerParty

        /// <summary>
        /// Gets the owner party of a role.
        /// It will return one entity as an IQueryable for performance.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static IQueryable<Party> OwnerPartyOfRoleQueryable(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var ownerParty = from role in RolesCurrentUserHasAccessTo(coreEntitiesContainer)
                             where role.Id == roleId
                             select role.OwnerParty;

            return ownerParty;
        }

        /// <summary>
        /// Gets the owner party of a role.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static Party OwnerPartyOfRole(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            return coreEntitiesContainer.OwnerPartyOfRoleQueryable(roleId).FirstOrDefault();
        }

        /// <summary>
        /// Gets the BusinessAccount owner of a role.
        /// It will return one entity as an IQueryable for performance.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns></returns>
        public static IQueryable<BusinessAccount> BusinessAccountOwnerOfRoleQueryable(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var ownerParty = from role in RolesCurrentUserHasAccessTo(coreEntitiesContainer)
                             where role.Id == roleId
                             select role.OwnerParty;

            return ownerParty.OfType<BusinessAccount>();
        }

        /// <summary>
        /// Gets the BusinessAccount owner of a role.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        public static BusinessAccount BusinessAccountOwnerOfRole(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var accountForRole = coreEntitiesContainer.OwnerPartyOfRole(roleId);
            return accountForRole as BusinessAccount;
        }

        /// <summary>
        /// Gets the business accounts the current user has access to.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static IQueryable<BusinessAccount> BusinessAccountsQueryable(this CoreEntitiesContainer coreEntitiesContainer)
        {
            return (from role in RolesCurrentUserHasAccessTo(coreEntitiesContainer)
                    select role.OwnerParty).OfType<BusinessAccount>();
        }

        #endregion

        #endregion

        #region User Accounts

        #region Access & Administration methods

        /// <summary>
        /// Returns if the current user can access the party.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="partyId">The party id.</param>
        /// <returns></returns>
        public static bool CurrentUserCanAccessParty(this CoreEntitiesContainer coreEntitiesContainer, Guid partyId)
        {
            //Return if the current user can administer the party or
            //if the current user is a FoundOPS admin (then they should have access to administer the party)
            return RolesCurrentUserHasAccessTo(coreEntitiesContainer).Select(r => r.OwnerParty).Any(p => p.Id == partyId || p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        /// <summary>
        /// Returns the parties the current user can administer.
        /// </summary>
        /// <param name="coreEntitiesContainer">The container.</param>
        /// <returns></returns>
        public static IQueryable<Party> PartiesCurrentUserCanAdminister(this CoreEntitiesContainer coreEntitiesContainer)
        {
            return AdministratorRoles(RolesCurrentUserHasAccessTo(coreEntitiesContainer)).Select(r => r.OwnerParty);
        }

        /// <summary>
        /// Returns if the current user can administer the party.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="partyId">The party id.</param>
        /// <returns></returns>
        public static bool CurrentUserCanAdministerParty(this CoreEntitiesContainer coreEntitiesContainer, Guid partyId)
        {
            //Return if the current user can administer the party or
            //if the current user is a FoundOPS admin (then they should have access to administer the party)
            return PartiesCurrentUserCanAdminister(coreEntitiesContainer).Any(p => p.Id == partyId || p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        /// <summary>
        /// Returns if the current user has FoundOPS admin access.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static bool CurrentUserCanAdministerFoundOPS(this CoreEntitiesContainer coreEntitiesContainer)
        {
            return PartiesCurrentUserCanAdminister(coreEntitiesContainer).Any(p => p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        #endregion

        #region Get methods

        /// <summary>
        /// Gets the current user accounts email address.
        /// </summary>
        /// <returns></returns>
        public static string CurrentUserAccountsEmailAddress()
        {
#if DEBUG
            if (ServerConstants.AutomaticLoginFoundOPSAdmin)
                return "jperl@foundops.com";

            if (ServerConstants.AutomaticLoginOPSManager)
                return "david@gotgrease.net";
#endif

            return HttpContext.Current.User.Identity.Name;
        }

        /// <summary>
        /// Gets the current user account in a queryable format.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static IQueryable<UserAccount> CurrentUserAccountQueryable(CoreEntitiesContainer coreEntitiesContainer)
        {
            return UserAccountQueryable(coreEntitiesContainer, CurrentUserAccountsEmailAddress());
        }

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static UserAccount CurrentUserAccount(CoreEntitiesContainer coreEntitiesContainer)
        {
            return CurrentUserAccountQueryable(coreEntitiesContainer).First();
        }

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static UserAccount GetUserAccount(CoreEntitiesContainer container, Guid userId)
        {
            return container.Parties.OfType<UserAccount>().FirstOrDefault(userParty => userParty.Id == userId);
        }

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="emailAddress">The user's email address.</param>
        /// <returns></returns>
        public static UserAccount GetUserAccount(CoreEntitiesContainer container, string emailAddress)
        {
            return UserAccountQueryable(container, emailAddress).FirstOrDefault();
        }

        /// <summary>
        /// Gets the user account for an email address in a queryable format.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public static IQueryable<UserAccount> UserAccountQueryable(CoreEntitiesContainer container, string emailAddress)
        {
            return container.Parties.OfType<UserAccount>().Where(userAccount => userAccount.EmailAddress == emailAddress);
        }

        #endregion

        #endregion
    }
}
