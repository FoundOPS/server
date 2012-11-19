using FoundOps.Api.Models;
using FoundOps.Common.Tools;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    public class ColumnConfigurationsController : BaseApiController
    {
        /// <summary>
        /// Get the user's column configurations for a role
        /// </summary>
        /// <param name="roleId">The role</param>
        public IEnumerable<ColumnConfiguration> Get(Guid roleId)
        {
            var userAccount = CoreEntitiesContainer.CurrentUserAccount();

            var columnConfigurations = new List<ColumnConfiguration>();

            if (!string.IsNullOrEmpty(userAccount.ColumnConfigurations))
            {
                try
                {
                    columnConfigurations = SerializationTools.Deserialize<List<ColumnConfiguration>>(userAccount.ColumnConfigurations);
                }
                catch
                {
                    columnConfigurations = new List<ColumnConfiguration>();
                    //if there is a problem with deserializing the column configurations (the xml got corrupt somehow): reset them
                    userAccount.ColumnConfigurations = SerializationTools.Serialize(columnConfigurations);
                    SaveWithRetry();
                }
            }

            return columnConfigurations.Where(c => c.RoleId == roleId);
        }

        /// <summary>
        /// Update the column configurations for a user for a role
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="columnConfigurations">The new column configurations</param>
        public void Put(Guid roleId, List<ColumnConfiguration> columnConfigurations)
        {
            var userAccount = CoreEntitiesContainer.CurrentUserAccount();

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
            SaveWithRetry();
        }
    }
}
