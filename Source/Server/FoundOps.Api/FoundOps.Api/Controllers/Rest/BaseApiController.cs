using System.Net;
using System.Net.Http;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace FoundOps.Api.Controllers.Rest
{
    public abstract class BaseApiController : ApiController
    {
        protected readonly CoreEntitiesContainer CoreEntitiesContainer;
        protected BaseApiController()
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();
            CoreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Will get the business account with access to the role or throw a not authorized error
        /// </summary>
        /// <param name="roleId">The roleId to find the business account from</param>
        /// <param name="roleTypes">The roles with access. Defaults to admin and regular</param>
        /// <param name="includes">Defaults to nothing. Includes off the BusinessAccount entity</param>
        protected BusinessAccount GetBusinessAccount(Guid? roleId, RoleType[] roleTypes = null, string[] includes = null)
        {
            if (roleTypes == null)
                roleTypes = new[] { RoleType.Administrator, RoleType.Regular };

            //If there is no RoleId passed, we can assume that the user is not authorized to see all the User Settings
            if (!roleId.HasValue || roleId.Value == Guid.Empty)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            var businessAccountQuery = CoreEntitiesContainer.Owner(roleId.Value, roleTypes);

            if (includes != null)
                businessAccountQuery = includes.Aggregate(businessAccountQuery, (current, include) => current.Include(include));

            var businessAccount = businessAccountQuery.FirstOrDefault();
            if (businessAccount == null)
                throw CommonExceptions.NotAuthorizedBusinessAccount;

            return businessAccount;
        }
    }
}