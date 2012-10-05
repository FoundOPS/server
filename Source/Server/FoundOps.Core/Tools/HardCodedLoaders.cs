using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;

namespace FoundOps.Core.Tools
{
    public static class HardCodedLoaders
    {
        /// <summary>
        /// Loads a service template
        /// </summary>
        /// <param name="container">The container to attach it to</param>
        /// <param name="serviceTemplateId">Optional filter parameter</param>
        /// <param name="clientId">Optional filter parameter</param>
        /// <param name="serviceProviderId">Optional filter parameter</param>
        /// <param name="levelInt">Optional filter parameter</param>
        /// <returns>The applicable service templates</returns>
        public static ServiceTemplate[] LoadServiceTemplateWithDetails(CoreEntitiesContainer container, Guid? serviceTemplateId, Guid? clientId, Guid? serviceProviderId, int? levelInt)
        {
            using (var db = new DbContext(ServerConstants.SqlConnectionString))
            {
                db.Database.Connection.Open();

                var loadServiceTemplate = db.Database.Connection.CreateCommand();
                loadServiceTemplate.CommandText = "GetServiceTemplatesAndFields";
                loadServiceTemplate.CommandType = CommandType.StoredProcedure;

                #region Add Parameters

                var parameter = new SqlParameter
                {
                    ParameterName = "serviceProviderContextId",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = serviceProviderId ?? (object) DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "clientContextId",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = clientId ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "serviceTemplateContextId",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = serviceTemplateId ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "levelInt",
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = levelInt ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                #endregion

                var reader = loadServiceTemplate.ExecuteReader();

                var serviceTemplates = container.Translate<ServiceTemplate>(reader, "ServiceTemplates", MergeOption.AppendOnly).ToArray();

                reader.NextResult();

                var fields = container.Translate<Field>(reader, "Fields", MergeOption.AppendOnly);

                reader.NextResult();

                var dateTimeFields = container.Translate<DateTimeField>(reader, "Fields", MergeOption.AppendOnly).ToList();

                reader.NextResult();

                var numericFields = container.Translate<NumericField>(reader, "Fields", MergeOption.AppendOnly).ToList();

                reader.NextResult();

                var textBoxFields = container.Translate<TextBoxField>(reader, "Fields", MergeOption.AppendOnly).ToList();

                reader.NextResult();

                var optionsFields = container.Translate<OptionsField>(reader, "Fields", MergeOption.AppendOnly).ToList();

                reader.NextResult();

                var locationFields = container.Translate<LocationField>(reader, "Fields", MergeOption.AppendOnly).ToList();

                reader.NextResult();

                var locations = container.Translate<Location>(reader, "Locations", MergeOption.AppendOnly).ToList();

                db.Database.Connection.Close();

                return serviceTemplates;
            }
        }
    }
}
