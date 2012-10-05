using System;
using System.Linq;
using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Core.Tools;

namespace FoundOps.Api.Controllers.Rest
{
    public class EmployeesController : BaseApiController
    {
        /// <summary>
        /// Gets all employees for a Business account
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<Employee> Get(Guid? roleId)
        {
            //If there is no RoleId passed, we can assume that the user is not authorized to see all the User Settings
            if (!roleId.HasValue)
                throw Request.NotAuthorized();

            //If they are not an admin, they do not have the ability to view Users
            var businessAccount = CoreEntitiesContainer.Owner(roleId.Value).FirstOrDefault();
            if (businessAccount == null)
                throw Request.NotAuthorized();

            var employees = CoreEntitiesContainer.Employees.Where(e => e.EmployerId == businessAccount.Id).ToList();

            //Add blank employee
            var newEmployee = new FoundOps.Core.Models.CoreEntities.Employee
            {
                //Identify it with an empty guid
                Id = new Guid(),
                FirstName = "None",
                LastName = ""
            };

            employees.Add(newEmployee);

            return employees.Select(Employee.ConvertModel).OrderBy(e => e.FirstName).AsQueryable();
        }
    }
}
