using FoundOps.Api.Models;
using FoundOps.Common.Tools;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace FoundOps.Api.Controllers.Rest
{
    public class ColumnsController : BaseApiController
    {
        /// <summary>
        /// Get the column configurations of a user for a role.
        /// </summary>
        /// <param name="roleId">The role</param>
        public IEnumerable<ColumnConfiguration> GetColumnConfigurations(Guid roleId)
        {
            var userAccount = CoreEntitiesContainer.CurrentUserAccount().First();

            List<ColumnConfiguration> columnConfigurations;

            try
            {
                columnConfigurations = SerializationTools.Deserialize<List<ColumnConfiguration>>(userAccount.ColumnConfigurations);
            }
            catch
            {
                columnConfigurations = new List<ColumnConfiguration>();
                userAccount.ColumnConfigurations = SerializationTools.Serialize(columnConfigurations);
                CoreEntitiesContainer.SaveChanges();
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
            var userAccount = CoreEntitiesContainer.CurrentUserAccount().First();

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
            CoreEntitiesContainer.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }
    }
}