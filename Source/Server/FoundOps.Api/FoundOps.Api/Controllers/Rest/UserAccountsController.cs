using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Http;
using UserAccount = FoundOps.Api.Models.UserAccount;

namespace FoundOps.Api.Controllers.Rest
{
    public class UserAccountsController : BaseApiController
    {
        /// <summary>
        /// REQUIRES: Admin access to the role.
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <returns>UserAccounts with access to the role's business account</returns>
        [Queryable]
        public IQueryable<UserAccount> GetAllUserAccounts(Guid roleId)
        {
            var business = GetBusinessAccount(roleId, new[] { RoleType.Administrator }, new[] { "Employees", "OwnedRoles", "MemberParties" });

            var userAccounts = business.OwnedRoles.SelectMany(r => r.MemberParties).OfType<Core.Models.CoreEntities.UserAccount>();

            //if the user is not a foundops user, do not return FoundOPS accounts
            if (!CoreEntitiesContainer.CanAdministerFoundOPS())
                userAccounts = userAccounts.Where(ua => !ua.EmailAddress.Contains("foundops.com"));

            var apiUserAccounts = userAccounts.OrderBy(ua => ua.LastName + " " + ua.FirstName)
                .Select(UserAccount.Convert).ToList();

            //find the employee id for each user account
            foreach (var ua in apiUserAccounts)
            {
                var employee = business.Employees.FirstOrDefault(e => e.LinkedUserAccountId == ua.Id);
                if (employee != null)
                    ua.EmployeeId = employee.Id;
            }

            return apiUserAccounts.AsQueryable();
        }

        // POST api/userAccounts
        public void Post([FromBody]string value)
        {

        }

        // PUT api/userAccounts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/userAccounts/5
        public void Delete(int id)
        {
        }
    }
}
