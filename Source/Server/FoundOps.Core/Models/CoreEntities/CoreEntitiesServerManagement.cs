using System;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using FoundOps.Common.NET;
using FoundOps.Common.Server;
using FoundOps.Core.Server.Blocks;
using Microsoft.SqlServer.Management.Common;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Core.Models.CoreEntities
{
    public class CoreEntitiesServerManagement
    {
        #region ConnectionString, Paths and Parameters

        private static readonly string SqlConnectionString = UserSpecificResourcesWrapper.ConnectionString("CoreConnectionString");
        private static readonly string ContainerConnectionString = UserSpecificResourcesWrapper.ConnectionString("CoreEntitiesContainer");

        private static readonly string RootDirectory = UserSpecificResourcesWrapper.GetString("RootDirectory");

        private static readonly string ClearCoreEntitiesDatabaseScriptLocation =
            RootDirectory + @"\FoundOps.Core\Models\CoreEntities\ClearCoreEntities.edmx.sql";

        private static readonly string CreateCoreEntitiesDatabaseScriptLocation =
            RootDirectory + @"\FoundOps.Core\Models\CoreEntities\CoreEntities.edmx.sql";

        #endregion

        public static void ClearCreateCoreEntitiesDatabase()
        {
#if DEBUG
            var clearFile = new FileInfo(ClearCoreEntitiesDatabaseScriptLocation);
            var createFile = new FileInfo(CreateCoreEntitiesDatabaseScriptLocation);
            string clearScript = clearFile.OpenText().ReadToEnd();
            string createScript = createFile.OpenText().ReadToEnd();
            using (var conn = new SqlConnection(SqlConnectionString))
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection(conn));
                server.ConnectionContext.ExecuteNonQuery(clearScript);
                server.ConnectionContext.ExecuteNonQuery(createScript);
            }
            clearFile.OpenText().Close();
            createFile.OpenText().Close();
#endif
        }

        public static void PopulateBlocks(CoreEntitiesContainer coreEntitiesContainer)
        {
            foreach (var block in BlocksData.AllBlocks)
            {
                block.Link = !String.IsNullOrEmpty(block.FileName) ? String.Format("/Block/GetBlock/{0}", block.FileName) : String.Format("/{0}", block.NavigateUri);
                coreEntitiesContainer.Blocks.AddObject(block);
            }
        }

        public static void ClearCreateCoreEntitiesDatabaseAndPopulateDesignData()
        {
#if DEBUG
            ClearCreateCoreEntitiesDatabase();

            var container = new CoreEntitiesContainer(ContainerConnectionString);

            //Setup blocks
            PopulateBlocks(container);

            //Setup Business Accounts (and ServiceTemplates)
            var businessAccountsDesignData = new BusinessAccountsDesignData();

            //Setup User Accounts
            var userAccountsDesignData = new UserAccountsDesignData
                                             {
                                                 //Set Passwords
                                                 Andrew = { PasswordHash = EncryptionTools.Hash("f00sballchamp") },
                                                 Jon = { PasswordHash = EncryptionTools.Hash("seltzer") },
                                                 Oren = { PasswordHash = EncryptionTools.Hash("nachoman") },
                                                 Zach = { PasswordHash = EncryptionTools.Hash("curlyhair") },
                                                 David = { PasswordHash = EncryptionTools.Hash("starofdavid") },
                                                 Linda = { PasswordHash = EncryptionTools.Hash("auntie") },
                                                 Terri = { PasswordHash = EncryptionTools.Hash("iam1randompassword") },
                                                 AlanMcClure = { PasswordHash = EncryptionTools.Hash("youngster") }
                                             };


            //Setup roles
            new RolesDesignData(businessAccountsDesignData, userAccountsDesignData);

            //Populate ServiceProvider Design Data
            foreach (var serviceProvider in businessAccountsDesignData.DesignServiceProviders)
            {
                var employeesDesignData = new EmployeesDesignData(serviceProvider);
                foreach (var employee in employeesDesignData.DesignEmployees)
                    serviceProvider.Employees.Add(employee);

                //Add Vehicles (and Vehicle Maintenance, and VehicleTypes)
                var vehiclesDesignData = new VehiclesDesignData(serviceProvider);
                foreach (var vehicle in vehiclesDesignData.DesignVehicles)
                    serviceProvider.Vehicles.Add(vehicle);

                //Add Regions
                var regionsDesignData = new RegionsDesignData();
                foreach (var region in regionsDesignData.DesignRegions)
                    serviceProvider.Regions.Add(region);

                //Add Clients (and ContactInfo, Locations, and RecurringServices)
                var clientsDesignData = new ClientsDesignData(serviceProvider, regionsDesignData);
                foreach (var client in clientsDesignData.DesignClients)
                    serviceProvider.Clients.Add(client);

                //Add Contacts
                var contactsDesignData = new ContactsDesignData(clientsDesignData);
                foreach (var contact in contactsDesignData.DesignContacts)
                    serviceProvider.Contacts.Add(contact);

                //container.SaveChanges();

                //Add Routes (with RouteTasks, Technicians)
                var routesDesignData = new RoutesDesignData(serviceProvider, clientsDesignData);
                foreach (var route in routesDesignData.DesignRoutes)
                    serviceProvider.Routes.Add(route);

                //container.SaveChanges();

                //Add Services
                //var servicesDesignData = new ServicesDesignData(businessAccount, clientsDesignData.DesignClient);
                //foreach (var service in servicesDesignData.DesignServices)
                //    businessAccount.ServicesToProvide.Add(service);
            }

            container.SaveChanges();
#endif
        }

    }
}
