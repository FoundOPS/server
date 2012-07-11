using System.Data.Entity;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Security.Authentication;

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
        /// Gets the roles the current user has access to
        /// NOTE: Role includes MemberParties, and OwnerParty
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown if user is not logged in</exception>
        public static IQueryable<Role> AdminRolesCurrentUserHasAccessTo(this CoreEntitiesContainer coreEntitiesContainer)
        {
#if DEBUG //Skip security in debug mode
            return coreEntitiesContainer.Roles.Where(r => r.RoleTypeInt == (int)RoleType.Administrator).Include(r => r.OwnerBusinessAccount).Include(r => r.MemberParties);
#endif

            var roles = (from user in CurrentUserAccountQueryable(coreEntitiesContainer) //there will only be one
                         from role in coreEntitiesContainer.Roles.Where(r => r.RoleTypeInt == (int)RoleType.Administrator)
                         where role.OwnerBusinessAccountId == user.Id || role.MemberParties.Any(a => a.Id == user.Id)
                         select role).Include(r => r.OwnerBusinessAccount).Include(r => r.MemberParties);

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
            var ownerParty = from role in AdminRolesCurrentUserHasAccessTo(coreEntitiesContainer)
                             where role.Id == roleId
                             select role.OwnerBusinessAccount;

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
            var ownerParty = from role in AdminRolesCurrentUserHasAccessTo(coreEntitiesContainer)
                             where role.Id == roleId
                             select role.OwnerBusinessAccount;

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
            return (from role in AdminRolesCurrentUserHasAccessTo(coreEntitiesContainer)
                    select role.OwnerBusinessAccount).OfType<BusinessAccount>();
        }

        #endregion

        #endregion

        #region User Accounts

        #region Access & Administration methods

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
            return AdminRolesCurrentUserHasAccessTo(coreEntitiesContainer).Select(r => r.OwnerBusinessAccount).Any(p => p.Id == partyId || p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        /// <summary>
        /// Returns if the current user has FoundOPS admin access.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static bool CurrentUserCanAdministerFoundOPS(this CoreEntitiesContainer coreEntitiesContainer)
        {
            return AdminRolesCurrentUserHasAccessTo(coreEntitiesContainer).Select(r => r.OwnerBusinessAccount).Any(p => p.Id == BusinessAccountsConstants.FoundOpsId);
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
