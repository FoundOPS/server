using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using FoundOps.Api.Models;
using FoundOps.Api.Tools;
using FoundOps.Core.Models;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Tools;
using Client = FoundOps.Core.Models.CoreEntities.Client;
using Location = FoundOps.Core.Models.CoreEntities.Location;

namespace FoundOps.Api.Controllers.Mvc
{
    public class ImporterController : Controller
    {
        private readonly CoreEntitiesContainer _coreEntitiesContainer;
        private readonly Guid _newEntityGuid = new Guid("10000000-0000-0000-0000-000000000000");

        public ImporterController()
        {
            _coreEntitiesContainer = new CoreEntitiesContainer();
            _coreEntitiesContainer.ContextOptions.LazyLoadingEnabled = false;
        }

        public Suggestions ValidateInput(Guid roleId, string[] headers, List<Cell[]> rows)
        {
            var businessAccount = _coreEntitiesContainer.Owner(roleId, new[] { RoleType.Administrator }).FirstOrDefault();
            if (businessAccount == null)
                return null;                

            const string sql = @"SELECT * FROM dbo.Locations WHERE BusinessAccountId = @id
SELECT * FROM dbo.Clients WHERE BusinessAccountId = @id";

            Location[] locations;
            Client[] clients;


            using (var conn = new SqlConnection(ServerConstants.SqlConnectionString))
            {
                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@id", businessAccount.Id);

                using (var data = conn.QueryMultiple(sql, new { id = businessAccount.Id }))
                { 
                    locations = data.Read<Location>().ToArray(); 

                    clients = data.Read<Client>().ToArray();
                }

                conn.Close();
            }

            return null;
        }

        public Suggestions ValidateEntites(ImportRow[] rows)
        {
            var suggestionToReturn = new Suggestions();

            foreach (var row in rows)
            {
                var rowSuggestions = new RowSuggestions();



            }
            return null;
        }

    }
}
