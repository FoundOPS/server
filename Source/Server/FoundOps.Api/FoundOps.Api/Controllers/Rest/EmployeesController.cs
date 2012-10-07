using FoundOps.Api.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Linq;
using Employee = FoundOps.Api.Models.Employee;

namespace FoundOps.Api.Controllers.Rest
{
    public class EmployeesController : BaseApiController
    {
        /// <summary>
        /// Gets all employees for a Business account
        /// REQUIRES: Admin access to role
        /// </summary>
        public IQueryable<Employee> Get(Guid roleId)
        {
            var businessAccount = CoreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var employees = CoreEntitiesContainer.Employees.Where(e => e.EmployerId == businessAccount.Id).ToList();
            return employees.Select(Employee.ConvertModel).OrderBy(e => e.FirstName).AsQueryable();
        }
    }
}
