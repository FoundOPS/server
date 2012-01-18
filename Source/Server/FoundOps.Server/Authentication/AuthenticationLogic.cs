using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Security.Authentication;
using System.Web;
using System.Text;
using System.Linq;
using FoundOps.Common.Server;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Server.Authentication
{
    public static class AuthenticationLogic
    {
        public static string CurrentUserAccountsEmailAddress()
        {
#if DEBUG
            if (UserSpecificResourcesWrapper.GetBool("AutomaticLoginJonathan"))
                return "jperl@foundops.com";
#endif

            return HttpContext.Current.User.Identity.Name;
        }

        public static IQueryable<UserAccount> CurrentUserAccountQueryable(CoreEntitiesContainer coreEntitiesContainer)
        {
            return GetUserAccountQueryable(coreEntitiesContainer, CurrentUserAccountsEmailAddress());
        }

        public static string GetCode()
        {
            var builder = new StringBuilder();
            var random = new Random();
            builder.Clear();
            for (int i = 0; i < 8; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            var code = builder.ToString().ToUpper();
            return code;
        }

        public static IQueryable<UserAccount> GetUserAccountQueryable(CoreEntitiesContainer container, string emailAddress)
        {
            return container.Parties.OfType<UserAccount>().Where(userAccount => userAccount.EmailAddress == emailAddress);
        }

        public static UserAccount GetUserAccount(CoreEntitiesContainer container, string emailAddress)
        {
            return GetUserAccountQueryable(container, emailAddress).FirstOrDefault();
        }

        public static UserAccount GetUserAccount(CoreEntitiesContainer container, Guid userId)
        {
            return container.Parties.OfType<UserAccount>().Where(userParty => userParty.Id == userId).FirstOrDefault();
        }

        /// <summary>
        ///  Role includes MemberPartys and OwnerParty
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown if user is not logged in</exception>
        public static IQueryable<Role> RolesCurrentUserHasAccessTo(this CoreEntitiesContainer coreEntitiesContainer)
        {
            var currentUser = AuthenticationLogic.CurrentUserAccountQueryable(coreEntitiesContainer).FirstOrDefault();

            return RolesCurrentUserHasAccessTo(currentUser, coreEntitiesContainer);
        }

        public static IQueryable<Role> RolesCurrentUserHasAccessTo(UserAccount currentUser, CoreEntitiesContainer coreEntitiesContainer)
        {
            if (currentUser == null)
                return null;

            return ((ObjectQuery<Role>)
                coreEntitiesContainer.Roles.Include("MemberParties").Where(r => r.OwnerPartyId == currentUser.Id || r.MemberParties.Any(a => a.Id == currentUser.Id)))
                .Include("Blocks").Include("OwnerParty");
        }

        /// <param name='rolesToFilter'>The roles that you want to filter</param>
        /// <summary>
        /// Returns the Administrator roles of rolesToFilter
        /// </summary>
        public static IQueryable<Role> AdministratorRoles(IQueryable<Role> rolesToFilter)
        {
            //TODO: Setup better Administrator role logic, when roles develop
            //TODO: Add logic for User administrators
            var filteredRoles = rolesToFilter.Where(r => r.RoleTypeInt == (int)RoleType.Administrator);

            return filteredRoles;
        }

        public static IQueryable<Party> PartiesCurrentUserCanAdminister(this CoreEntitiesContainer coreEntitiesContainer)
        {
            var rolesCurrentUserHasAccessTo = RolesCurrentUserHasAccessTo(coreEntitiesContainer);
            return AdministratorRoles(rolesCurrentUserHasAccessTo).Select(r => r.OwnerParty);
        }

        public static bool CurrentUserCanAdministerThisParty(this CoreEntitiesContainer coreEntitiesContainer, Guid partyId)
        {
            var partiesCurrenUserCanAdminister = PartiesCurrentUserCanAdminister(coreEntitiesContainer).ToArray();

            return partiesCurrenUserCanAdminister.Any(p => p.Id == partyId);
        }

        public static Party PartyForRole(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var role = RolesCurrentUserHasAccessTo(coreEntitiesContainer).FirstOrDefault(r => r.Id == roleId);

            if (role == null || role.OwnerPartyId == null)
                return null;

            var ownerParty = role.OwnerParty;
            ownerParty.PartyImageReference.Load();
            return ownerParty;
        }

        public static Business BusinessForRole(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var accountForRole = coreEntitiesContainer.PartyForRole(roleId);

            if (!(accountForRole is Business))
                return null;

            var ownerParty = (Business)accountForRole;
            return ownerParty;
        }

        public static BusinessAccount BusinessAccountForRole(this CoreEntitiesContainer coreEntitiesContainer, Guid roleId)
        {
            var accountForRole = coreEntitiesContainer.PartyForRole(roleId);

            if (!(accountForRole is BusinessAccount))
                return null;

            var ownerParty = (BusinessAccount)accountForRole;
            return ownerParty;
        }
    }
}
