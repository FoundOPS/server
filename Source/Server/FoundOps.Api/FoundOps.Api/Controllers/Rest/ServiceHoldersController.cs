using Dapper;
using FoundOps.Api.Tools;
using FoundOps.Common.Tools.ExtensionMethods;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.ServiceEntites;
using FoundOps.Core.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace FoundOps.Api.Controllers.Rest
{
    public class ServiceHoldersController : BaseApiController
    {
        /// <summary>
        /// Get service holders with fields 
        /// </summary>
        /// <param name="roleId">The role</param>
        /// <param name="clientContext">The Id of the client to filter by</param>
        /// <param name="recurringServiceContext">The Id of the recurring service to filter by</param>
        /// <param name="startDate">The start date (in the user's time zone)</param>
        /// <param name="endDate">The end date (in the user's time zone)</param>
        /// <param name="serviceType">The service type to filter by</param>
        /// <param name="single">Only return the types</param>
        /// <returns>A queryable of dictionaries that resemble record type javascript objects when serialized</returns>
        public IQueryable<Dictionary<string, Object>> Get(Guid roleId, string serviceType, Guid? clientContext, Guid? recurringServiceContext,
            DateTime startDate, DateTime endDate, bool single = false)
        {
            var currentBusinessAccount = CoreEntitiesContainer.Owner(roleId).FirstOrDefault();
            if (currentBusinessAccount == null)
                throw new Exception("Bad Request");

            //Insert the first row to be a dictionary of the column's types
            //certain values are not fields, hardcode those
            var columnTypes = new Dictionary<string, object> { { "OccurDate", "date" }, { "RecurringServiceId", "guid" }, { "ServiceId", "guid" }, { "ClientName", "string" } };

            var parameters = new DynamicParameters();
            parameters.Add("@businessAccountId", currentBusinessAccount.Id);
            parameters.Add("@serviceType", serviceType);

            FieldJavaScript[] javaScriptFields;

            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                javaScriptFields = conn.Query<FieldJavaScript>("GetFieldsInJavaScriptFormat", parameters, commandType: CommandType.StoredProcedure).ToArray();

                if (javaScriptFields.Select(f => f.Name).Distinct().Count() != javaScriptFields.Count())
                    throw Request.InternalError("Duplicate fields");

                foreach (var field in javaScriptFields)
                {
                    columnTypes.Add(field.Name, field.Type);
                }

                conn.Close();
            }

            if (single) //just return the types
            {
                return new List<Dictionary<string, object>> { columnTypes }.AsQueryable();
            }

            #region Load Service Templates and Fields

            ServiceTemplateWithDate[] serviceTemplates;
            ILookup<Guid, ISimpleField> simpleNumericFields;
            ILookup<Guid, ISimpleField> simpleTextFields;
            ILookup<Guid, ISimpleField> simpleOptionsFields;
            ILookup<Guid, ISimpleField> simpleLocationFields;
            ILookup<Guid, ISimpleField> simpleSignatureFields;
            ILookup<Guid, ISimpleField> simpleTaskStatuses;

            using (var db = new DbContext(ServerConstants.SqlConnectionString))
            {
                db.Database.Connection.Open();

                var loadServiceTemplate = db.Database.Connection.CreateCommand();
                loadServiceTemplate.CommandText = "GetServiceTemplatesWithDateAndDetails";
                loadServiceTemplate.CommandType = CommandType.StoredProcedure;

                #region Add Parameters

                var parameter = new SqlParameter
                {
                    ParameterName = "serviceProviderIdContext",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = currentBusinessAccount.Id
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "clientIdContext",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = clientContext ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "recurringServiceIdContext",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    Value = recurringServiceContext ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "firstDate",
                    DbType = DbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = startDate
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "lastDate",
                    DbType = DbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = endDate
                };

                loadServiceTemplate.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "serviceTypeContext",
                    DbType = DbType.String,
                    Direction = ParameterDirection.Input,
                    Value = serviceType ?? (object)DBNull.Value
                };

                loadServiceTemplate.Parameters.Add(parameter);

                #endregion

                var reader = loadServiceTemplate.ExecuteReader();

                serviceTemplates = CoreEntitiesContainer.Translate<ServiceTemplateWithDate>(reader).ToArray();

                reader.NextResult();

                simpleTaskStatuses = CoreEntitiesContainer.Translate<SimpleStatusField>(reader).ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                reader.NextResult();

                simpleNumericFields = CoreEntitiesContainer.Translate<SimpleNumericField>(reader).AsParallel().ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                reader.NextResult();

                simpleTextFields = CoreEntitiesContainer.Translate<SimpleTextField>(reader).AsParallel().ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                reader.NextResult();

                simpleSignatureFields = CoreEntitiesContainer.Translate<SimpleSignatureField>(reader).AsParallel().ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                reader.NextResult();

                simpleOptionsFields = CoreEntitiesContainer.Translate<SimpleOptionsField>(reader).AsParallel().ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                reader.NextResult();

                simpleLocationFields = CoreEntitiesContainer.Translate<SimpleTextField>(reader).AsParallel().ToLookup(f => f.ServiceTemplateId, f => (ISimpleField)f);

                db.Database.Connection.Close();
            }

            #endregion

            var result = serviceTemplates.AsParallel().Select(st =>
            {
                var dictionary = new Dictionary<string, Object>
                        {
                            {"OccurDate", st.OccurDate},
                            {"RecurringServiceId", st.RecurringServiceId},
                            {"ServiceId", st.ServiceId},
                            {"ClientName", st.ClientName}
                        };

                var id = st.ServiceId.HasValue ? st.ServiceId.Value : st.RecurringServiceId.Value;

                var fields = simpleTaskStatuses[id].Distinct(ts => ts.Name).Union(simpleNumericFields[id]).Union(simpleTextFields[id]).Union(simpleSignatureFields[id]).Union(
                        simpleOptionsFields[id]).Union(simpleLocationFields[id]).ToDictionary(f => f.Name);

                foreach (var name in javaScriptFields.Select(f => f.Name))
                    dictionary.Add(name, fields.ContainsKey(name) ? fields[name].ObjectValue : null);

                return dictionary;
            }).ToList();

            result.Insert(0, columnTypes);

            return result.AsQueryable();
        }
    }
}
