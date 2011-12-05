using System;
using System.Collections.Generic;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.Core.Server.Blocks
{
    public class BlocksData
    {
        public List<Block> AllBlocks { get; private set; }

        public BlocksData()
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
        public Block SettingsBlock { get; private set; }

        public Block LogoutBlock { get; private set; }

        /// <summary>
        /// The Ids of the default UserAccount Role's Blocks
        /// </summary>
        public static Guid[] DefaultUserAccountBlockIds = new[] { new Guid("B8ED1EF7-C43A-43DF-AF26-F5FB5BC0DE65"), new Guid("B8ED1EF7-C43A-43DF-AF26-F5FB5BC0DE65")};

        public List<Block> UserAccountBlocks { get; private set; }
        public List<Block> BusinessAdministratorBlocks { get; private set; }

        public void SetupPartyBlocks()
        {
            SettingsBlock = new Block
            {
                Id = new Guid("6E7E2F51-5925-4AF1-9F9A-663165BA7B68"),
                Name = "Settings",
                NavigateUri = "Settings"
            };

            #region UserAccount

            LogoutBlock = new Block()
            {
                Id = new Guid("B8ED1EF7-C43A-43DF-AF26-F5FB5BC0DE65"),
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

        public Block ClientsBlock { get; private set; }
        public Block ContactsBlock { get; private set; }
        public Block LocationsBlock { get; private set; }
        public Block RegionsBlock { get; private set; }
        public Block ServicesBlock { get; private set; }



        public Block DispatcherBlock { get; private set; }

        public Block EmployeesBlock { get; private set; }
        public Block ImportDataBlock { get; private set; }

        public List<Block> ManagerBlocks { get; set; }

        public Block VehiclesBlock { get; private set; }
        public Block VehicleMaintenanceBlock { get; private set; }

        public void SetupManagerBlocks()
        {
            ClientsBlock = new Block
                               {
                                   Id = new Guid("076E5CC5-E1DF-47A0-BCE7-DBCF3281895A"),
                                   Name = "Clients",
                                   NavigateUri = "Clients"
                               };

            ContactsBlock = new Block
                                {
                                    Id = new Guid("{C1D81ECE-A5D7-420D-9BCF-AA9298F98EBC}"),
                                    Name = "Contacts",
                                    NavigateUri = "Contacts"
                                };

            DispatcherBlock = new Block
            {
                Id = new Guid("C6C6C562-AFF4-4555-94DC-6C53A90A6B27"),
                Name = "Dispatcher",
                NavigateUri = "Dispatcher"
            };

            EmployeesBlock = new Block
            {
                Id = new Guid("5F4424A8-5327-4346-B9C3-884DB399F6D6"),
                Name = "Employees",
                NavigateUri = "Employees"
            };

            ImportDataBlock = new Block
            {
                Id = new Guid("8886E699-BE2F-4891-B6BB-563F1802050E"),
                Name = "Import Data",
                NavigateUri = "ImportData"
            };

            LocationsBlock = new Block
                                 {
                                     Id = new Guid("23DE47A8-F56E-4A2F-96EB-F7E3C7A6BBA5"),
                                     Name = "Locations",
                                     NavigateUri = "Locations"
                                 };

            RegionsBlock = new Block
                                {
                                    Id = new Guid("02FC9C08-299D-484F-8939-07C985656ACE"),
                                    Name = "Regions",
                                    NavigateUri = "Regions"
                                };

            ServicesBlock = new Block
                                {
                                    Id = new Guid("DACEF1E4-2C96-4E9A-892E-B3AA1FC9CE6C"),
                                    Name = "Services",
                                    NavigateUri = "Services"
                                };


            VehiclesBlock = new Block
            {
                Id = new Guid("5BB29361-D22E-40E8-9C49-50DA4E506940"),
                Name = "Vehicles",
                NavigateUri = "Vehicles"
            };

            VehicleMaintenanceBlock = new Block
            {
                Id = new Guid("25B59398-5ECF-4F6D-B67B-6E8B4406BC5D"),
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
                                    ImportDataBlock,
                                    LogoutBlock
                                };
        }

        #endregion

        #region Public Blocks

        public Block CompanyHomeBlock { get; private set; }
        public List<Block> PublicBlocks { get; set; }

        public void SetupPublicBlocks()
        {
            CompanyHomeBlock = new Block()
                                   {
                                       Id = new Guid("E69BDD63-A73F-4903-9AEC-2B92724A00FE"),
                                       Name = "Company Home",
                                       NavigateUri = "CompanyHome",
                                       LoginNotRequired = true
                                   };

            PublicBlocks = new List<Block>
                                         {
                                             CompanyHomeBlock
                                         };
        }

        #endregion

        #region FoundOPS Administrative Console Blocks

        public Block BusinessAccountsBlock { get; private set; }
        public Block ServiceTemplatesBlock { get; private set; }
        public List<Block> AdministrativeConsoleBlocks { get; private set; }

        public void SetupAdministrativeConsoleBlocks()
        {
            BusinessAccountsBlock = new Block()
            {
                Id = new Guid("EDFD7576-B91E-457A-B237-0B0CEF4A2B70"),
                Name = "Business Accounts",
                NavigateUri = "BusinessAccounts"
            };

            ServiceTemplatesBlock = new Block()
            {
                Id = new Guid("10A8524A-1AA1-403E-95EF-ACC9F8CCBE5D"),
                Name = "Service Templates",
                NavigateUri = "ServiceTemplates"
            };

            AdministrativeConsoleBlocks = new List<Block>
                                         {
                                             BusinessAccountsBlock,
                                             ServiceTemplatesBlock
                                         };
        }

        #endregion

        private void SetupAllBlocks()
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