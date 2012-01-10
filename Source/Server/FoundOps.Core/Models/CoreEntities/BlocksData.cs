using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Server.Blocks
{
    /// <summary>
    /// Constant Block values.
    /// </summary>
    public static class BlockConstants
    {
        #region PartyBlocks

        /// <summary>
        /// The Settings block Id
        /// </summary>
        public static Guid SettingsBlockId = new Guid("6E7E2F51-5925-4AF1-9F9A-663165BA7B68");

        /// <summary>
        /// The Logout block Id
        /// </summary>
        public static Guid LogoutBlockId = new Guid("B8ED1EF7-C43A-43DF-AF26-F5FB5BC0DE65");

        /// <summary>
        /// The BusinessAdministrator block ids.
        /// </summary>
        public static IEnumerable<Guid> BusinessAdministratorBlockIds = new[] { SettingsBlockId };

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<Guid> UserAccountBlockIds = new[] { LogoutBlockId, SettingsBlockId };

        #endregion

        #region Manager Blocks

        /// <summary>
        /// The Clients block Id
        /// </summary>
        public static Guid ClientBlockId = new Guid("076E5CC5-E1DF-47A0-BCE7-DBCF3281895A");
        /// <summary>
        /// The Contacts block Id
        /// </summary>
        public static Guid ContactsBlockId = new Guid("{C1D81ECE-A5D7-420D-9BCF-AA9298F98EBC}");

        /// <summary>
        /// The Dispatchers block Id
        /// </summary>
        public static Guid DispatcherBlockId = new Guid("C6C6C562-AFF4-4555-94DC-6C53A90A6B27");
        /// <summary>
        /// The Employees block Id
        /// </summary>
        public static Guid EmployeesBlockId = new Guid("5F4424A8-5327-4346-B9C3-884DB399F6D6");

        /// <summary>
        /// The ImportData block Id
        /// </summary>
        public static Guid ImportDataBlockId = new Guid("8886E699-BE2F-4891-B6BB-563F1802050E");

        /// <summary>
        /// The Locations block Id
        /// </summary>
        public static Guid LocationsBlockId = new Guid("23DE47A8-F56E-4A2F-96EB-F7E3C7A6BBA5");
        /// <summary>
        /// The Regions block Id
        /// </summary>
        public static Guid RegionsBlockId = new Guid("02FC9C08-299D-484F-8939-07C985656ACE");
        /// <summary>
        /// The Services block Id
        /// </summary>
        public static Guid ServicesBlockId = new Guid("DACEF1E4-2C96-4E9A-892E-B3AA1FC9CE6C");

        /// <summary>
        /// The Vehicles block Id
        /// </summary>
        public static Guid VehiclesBlockId = new Guid("5BB29361-D22E-40E8-9C49-50DA4E506940");
        /// <summary>
        /// The VehicleMaintenance block Id
        /// </summary>
        public static Guid VehicleMaintenanceBlockId = new Guid("25B59398-5ECF-4F6D-B67B-6E8B4406BC5D");

        /// <summary>
        /// The Manager blocks' ids
        /// </summary>
        public static IEnumerable<Guid> ManagerBlockIds = new[]
                                                              {
                                                                  ClientBlockId, ContactsBlockId, DispatcherBlockId,
                                                                  EmployeesBlockId, //ImportDataBlockId,
                                                                  LocationsBlockId, RegionsBlockId, ServicesBlockId,
                                                                  VehiclesBlockId, VehicleMaintenanceBlockId,
                                                                  LogoutBlockId
                                                              };

        #endregion

        #region Public Blocks

        /// <summary>
        /// The CompanyHome block Id
        /// </summary>
        public static Guid CompanyHomeBlockId = new Guid("E69BDD63-A73F-4903-9AEC-2B92724A00FE");

        /// <summary>
        /// The Public blocks' ids.
        /// </summary>
        public static IEnumerable<Guid> PublicBlockIds = new[] { CompanyHomeBlockId };

        #endregion

        #region FoundOPS Administrative Console Blocks

        /// <summary>
        /// The BusinessAccounts block Id.
        /// </summary>
        public static Guid BusinessAccountsBlockId = new Guid("EDFD7576-B91E-457A-B237-0B0CEF4A2B70");

        /// <summary>
        /// The ServiceTemplates block Id.
        /// </summary>
        public static Guid ServiceTemplatesBlockId = new Guid("10A8524A-1AA1-403E-95EF-ACC9F8CCBE5D");

        /// <summary>
        /// The AdministrativeConsole blocks' ids.
        /// </summary>
        public static IEnumerable<Guid> AdministrativeConsoleBlockIds = new[] { BusinessAccountsBlockId, ServiceTemplatesBlockId };
        
        #endregion
    }

    /// <summary>
    /// The static class of Block entity information.
    /// Should not be used in Silverlight or else it will cause issues with the entity tracking.
    /// TODO: Remove from silverlight project.
    /// </summary>
    public static class BlocksData
    {
        /// <summary>
        /// Gets all the blocks.
        /// </summary>
        public static List<Block> AllBlocks { get; private set; }

        static BlocksData()
        {
            SetupPartyBlocks();

            SetupManagerBlocks();
            SetupPublicBlocks();

            SetupAdministrativeConsoleBlocks();

            SetupAllBlocks();
        }

        #region PartyBlocks

        /// <summary>
        /// Contains a page to setup UserAccountSettings and BusinessAccountSettings.
        /// </summary>
        public static Block SettingsBlock { get; private set; }

        /// <summary>
        /// Gets the logout block.
        /// </summary>
        public static Block LogoutBlock { get; private set; }

        /// <summary>
        /// Gets the user account blocks.
        /// </summary>
        public static List<Block> UserAccountBlocks { get; private set; }
        /// <summary>
        /// Gets the business administrator blocks.
        /// </summary>
        public static List<Block> BusinessAdministratorBlocks { get; private set; }

        private static void SetupPartyBlocks()
        {
            SettingsBlock = new Block
            {
                Id = BlockConstants.SettingsBlockId,
                Name = "Settings",
                NavigateUri = "Settings"
            };

            #region UserAccount

            LogoutBlock = new Block()
            {
                Id = BlockConstants.LogoutBlockId,
                Name = "Logout",
                NavigateUri = @"Account/LogOff"
            };

            UserAccountBlocks = new List<Block> { LogoutBlock, SettingsBlock };

            #endregion

            #region BusinessAccount

            BusinessAdministratorBlocks = new List<Block>
                                              {
                                                  SettingsBlock
                                              };

            #endregion
        }

        #endregion

        #region Manager

        /// <summary>
        /// Gets the clients block.
        /// </summary>
        public static Block ClientsBlock { get; private set; }
        /// <summary>
        /// Gets the contacts block.
        /// </summary>
        public static Block ContactsBlock { get; private set; }
        /// <summary>
        /// Gets the dispatcher block.
        /// </summary>
        public static Block DispatcherBlock { get; private set; }
        /// <summary>
        /// Gets the employees block.
        /// </summary>
        public static Block EmployeesBlock { get; private set; }

        /// <summary>
        /// Gets the import data block.
        /// </summary>
        public static Block ImportDataBlock { get; private set; }

        /// <summary>
        /// Gets the locations block.
        /// </summary>
        public static Block LocationsBlock { get; private set; }
        /// <summary>
        /// Gets the regions block.
        /// </summary>
        public static Block RegionsBlock { get; private set; }
        /// <summary>
        /// Gets the services block.
        /// </summary>
        public static Block ServicesBlock { get; private set; }

        /// <summary>
        /// Gets the vehicles block.
        /// </summary>
        public static Block VehiclesBlock { get; private set; }
        /// <summary>
        /// Gets the vehicle maintenance block.
        /// </summary>
        public static Block VehicleMaintenanceBlock { get; private set; }

        /// <summary>
        /// The list of manager blocks.
        /// </summary>
        public static List<Block> ManagerBlocks { get; set; }

        private static void SetupManagerBlocks()
        {
            ClientsBlock = new Block
            {
                Id = BlockConstants.ClientBlockId,
                Name = "Clients",
                NavigateUri = "Clients"
            };

            ContactsBlock = new Block
            {
                Id = BlockConstants.ContactsBlockId,
                Name = "Contacts",
                NavigateUri = "Contacts"
            };

            DispatcherBlock = new Block
            {
                Id = BlockConstants.DispatcherBlockId,
                Name = "Dispatcher",
                NavigateUri = "Dispatcher"
            };

            EmployeesBlock = new Block
            {
                Id = BlockConstants.EmployeesBlockId,
                Name = "Employees",
                NavigateUri = "Employees"
            };

            ImportDataBlock = new Block
            {
                Id = BlockConstants.ImportDataBlockId,
                Name = "Import Data",
                NavigateUri = "ImportData"
            };

            LocationsBlock = new Block
            {
                Id = BlockConstants.LocationsBlockId,
                Name = "Locations",
                NavigateUri = "Locations"
            };

            RegionsBlock = new Block
            {
                Id = BlockConstants.RegionsBlockId,
                Name = "Regions",
                NavigateUri = "Regions"
            };

            ServicesBlock = new Block
            {
                Id = BlockConstants.ServicesBlockId,
                Name = "Services",
                NavigateUri = "Services"
            };


            VehiclesBlock = new Block
            {
                Id = BlockConstants.VehiclesBlockId,
                Name = "Vehicles",
                NavigateUri = "Vehicles"
            };

            VehicleMaintenanceBlock = new Block
            {
                Id = BlockConstants.VehicleMaintenanceBlockId,
                Name = "VehicleMaintenance",
                NavigateUri = "Vehicle Maintenance",
                HideFromNavigation = true
            };

            ManagerBlocks = new List<Block>
                                {
                                    ClientsBlock,
                                    ContactsBlock,
                                    EmployeesBlock,
                                    ServicesBlock,
                                    LocationsBlock,
                                    RegionsBlock,
                                    DispatcherBlock,
                                    EmployeesBlock,
                                    VehiclesBlock,
                                    VehicleMaintenanceBlock,
                                    //ImportDataBlock,
                                    LogoutBlock
                                };
        }

        #endregion

        #region Public Blocks

        /// <summary>
        /// Gets the company home block.
        /// </summary>
        public static Block CompanyHomeBlock { get; private set; }
        /// <summary>
        /// Gets the public blocks.
        /// </summary>
        public static List<Block> PublicBlocks { get; private set; }

        private static void SetupPublicBlocks()
        {
            CompanyHomeBlock = new Block
            {
                Id = BlockConstants.CompanyHomeBlockId,
                Name = "Company Home",
                NavigateUri = "CompanyHome",
                LoginNotRequired = true
            };

            PublicBlocks = new List<Block> { CompanyHomeBlock };
        }

        #endregion

        #region FoundOPS Administrative Console Blocks

        /// <summary>
        /// Gets the business accounts block.
        /// </summary>
        public static Block BusinessAccountsBlock { get; private set; }
        /// <summary>
        /// Gets the service templates block.
        /// </summary>
        public static Block ServiceTemplatesBlock { get; private set; }
        /// <summary>
        /// Gets the administrative console blocks.
        /// </summary>
        public static List<Block> AdministrativeConsoleBlocks { get; private set; }

        private static void SetupAdministrativeConsoleBlocks()
        {
            BusinessAccountsBlock = new Block()
            {
                Id = BlockConstants.BusinessAccountsBlockId,
                Name = "Business Accounts",
                NavigateUri = "BusinessAccounts"
            };

            ServiceTemplatesBlock = new Block()
            {
                Id = BlockConstants.ServiceTemplatesBlockId,
                Name = "Service Templates",
                NavigateUri = "ServiceTemplates"
            };

            AdministrativeConsoleBlocks = new List<Block> { BusinessAccountsBlock, ServiceTemplatesBlock };
        }

        #endregion

        private static void SetupAllBlocks()
        {
            AllBlocks = new List<Block>();

            AllBlocks.AddRange(ManagerBlocks);
            AllBlocks.AddRange(BusinessAdministratorBlocks);
            AllBlocks.AddRange(UserAccountBlocks);

            AllBlocks.AddRange(AdministrativeConsoleBlocks);

            AllBlocks.AddRange(PublicBlocks);
        }
    }
}