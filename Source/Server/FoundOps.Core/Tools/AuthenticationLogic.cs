using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.DesignData;
using System;
using System.Data.Entity;
using System.Web;
using System.Linq;
using System.Security.Authentication;

namespace FoundOps.Core.Tools
{
    public static class AuthenticationLogic
    {
        #region Roles

        /// <summary>
        /// Gets the owner party of a role.
        /// It will return one entity as an IQueryable for performance.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="allowedRoleTypes">Return null if the user does not have access to one of these role types.</param>
        public static IQueryable<T> Owner<T>(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId, RoleType[] allowedRoleTypes) where T : Party
        {
            var ownerParty = from role in RolesCurrentUserHasAccessTo(coreEntitiesContainer, allowedRoleTypes)
                             where role.Id == roleId
                             select role.OwnerBusinessAccount;

            return ownerParty.OfType<T>();
        }

        /// <summary>
        /// Override of Owner (gets the owner party of a role). Defaults allowedRoleTypes to Administrator and Regular roles.
        /// It will return one entity as an IQueryable for performance.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        public static IQueryable<T> Owner<T>(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId) where T : Party
        {
            return coreEntitiesContainer.Owner<T>(roleId, new[] { RoleType.Administrator, RoleType.Regular });
        }

        /// <summary>
        /// Override of Owner (gets the owner party of a role). Defaults allowedRoleTypes to Administrator and Regular roles, and defaults to the BusinessAccount type.
        /// It will return one entity as an IQueryable for performance.
        /// Returns null if the user does not have access to the BusinessAccount.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="roleId">The role id.</param>
        public static IQueryable<BusinessAccount> Owner(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            return coreEntitiesContainer.Owner<BusinessAccount>(roleId);
        }

        /// <summary>
        /// Gets the roles the current user has access to
        /// NOTE: Role includes MemberParties, and OwnerParty
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="allowedRoleTypes">Return null if the user does not have access to one of these role types.</param>
        /// <exception cref="AuthenticationException">Thrown if user is not logged in</exception>
        public static IQueryable<Role> RolesCurrentUserHasAccessTo(this CoreEntitiesContainer coreEntitiesContainer, RoleType[] allowedRoleTypes)
        {
            var allowedRoleTypeInts = allowedRoleTypes.Select(t => (int)t);

            var roles = (from user in CurrentUserAccount(coreEntitiesContainer) //there will only be one
                         from role in coreEntitiesContainer.Roles.Where(r => allowedRoleTypeInts.Contains(r.RoleTypeInt))
#if !DEBUG //Skip security in debug mode
                         where role.MemberParties.Any(a => a.Id == user.Id)
#endif
                         select role).Include(r => r.OwnerBusinessAccount).Include(r => r.MemberParties);

            return roles;
        }

        #endregion

        #region User Accounts

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="coreEntitiesContainer">The coreEntitiesContainer.</param>
        /// <param name="emailAddress">The user's email address.</param>
        /// <returns></returns>
        public static UserAccount GetUserAccount(this CoreEntitiesContainer coreEntitiesContainer, string emailAddress)
        {
            var email = emailAddress.Trim();
            return coreEntitiesContainer.Parties.OfType<UserAccount>().FirstOrDefault(userAccount => userAccount.EmailAddress == email);
        }

        /// <summary>
        /// Gets the current user account in a queryable format.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static IQueryable<UserAccount> CurrentUserAccount(this CoreEntitiesContainer coreEntitiesContainer)
        {
            string currentUsersEmail = "";
#if DEBUG
            if (ServerConstants.AutomaticLoginFoundOPSAdmin)
                currentUsersEmail = "jperl@foundops.com";
#else
            currentUsersEmail = HttpContext.Current.User.Identity.Name;
#endif
            currentUsersEmail = currentUsersEmail.Trim();
            return coreEntitiesContainer.Parties.OfType<UserAccount>().Where(userAccount => userAccount.EmailAddress == currentUsersEmail);
        }

        /// <summary>
        /// Returns if the current user can administer the party.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <param name="partyId">The party id.</param>
        /// <returns></returns>
        public static bool CanAdminister(this CoreEntitiesContainer coreEntitiesContainer, Guid partyId)
        {
            //Return if the current user can administer the party or
            //if the current user is a FoundOPS admin (then they should have access to administer the party)
            return RolesCurrentUserHasAccessTo(coreEntitiesContainer, new[] { RoleType.Administrator }).Select(r => r.OwnerBusinessAccount).Any(p => p.Id == partyId || p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        /// <summary>
        /// Returns if the current user has FoundOPS admin access.
        /// </summary>
        /// <param name="coreEntitiesContainer">The core entities container.</param>
        /// <returns></returns>
        public static bool CanAdministerFoundOPS(this CoreEntitiesContainer coreEntitiesContainer)
        {
            return RolesCurrentUserHasAccessTo(coreEntitiesContainer, new[] { RoleType.Administrator }).Select(r => r.OwnerBusinessAccount).Any(p => p.Id == BusinessAccountsConstants.FoundOpsId);
        }

        #endregion
    }
}
