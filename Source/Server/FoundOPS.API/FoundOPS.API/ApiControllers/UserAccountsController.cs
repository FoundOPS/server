using FoundOPS.Api.Api;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using System.Web.Http;
using UserAccount = FoundOPS.Api.Models.UserAccount;

namespace FoundOps.Api.ApiControllers
{
    public class UserAccountsController : BaseApiController
    {
        // GET api/userAccounts
        /// <summary>
        /// Return the UserAccount's with access to this role's business account.
        /// REQUIRES: Admin access to the role.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Queryable]
        public IQueryable<UserAccount> Get(Guid roleId)
        {
            var business = GetBusinessAccount(roleId, new[] { RoleType.Administrator}, new[] { "Employees", "OwnedRoles", "MemberParties" });

            var userAccounts = business.OwnedRoles.SelectMany(r => r.MemberParties).OfType<FoundOps.Core.Models.CoreEntities.UserAccount>();

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
