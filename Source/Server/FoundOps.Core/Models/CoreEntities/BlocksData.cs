using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities
{
    /// <summary>
    /// Constant Block values.
    /// </summary>
    public static class BlockConstants
    {
        #region Mobile Blocks

        /// <summary>
        /// The Routes block Id
        /// </summary>
        public static Guid RoutesBlockId = new Guid("6ABF7716-C4B8-442D-88C2-FFBB1C62598E");

        /// <summary>
        /// The mobile block ids
        /// </summary>
        public static IEnumerable<Guid> MobileBlockIds = new[]
            {
                RoutesBlockId
            };

        #endregion

        #region Regular Blocks

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
        /// The Feedback and Support Id
        /// </summary>
        public static Guid FeedbackSupportBlockId = new Guid("E2EAA8CB-5C45-42DA-8060-F962C2E2B47F");

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
        public static IEnumerable<Guid> RegularBlockIds = new[]
            {
                ClientBlockId, ContactsBlockId, DispatcherBlockId,
                EmployeesBlockId, FeedbackSupportBlockId, ImportDataBlockId,
                LocationsBlockId, RegionsBlockId, ServicesBlockId,
                VehiclesBlockId, VehicleMaintenanceBlockId
            };

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
        public static IEnumerable<Guid> AdministrativeConsoleBlockIds = new[] { BusinessAccountsBlockId };

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
            SetupRegularBlocks();

            SetupMobileBlocks();

            SetupAdministrativeConsoleBlocks();

            SetupAllBlocks();
        }

        #region Mobile Blocks

        /// <summary>
        /// Gets the routes block.
        /// </summary>
        public static Block RoutesBlock { get; private set; }

        /// <summary>
        /// Gets the mobile application blocks.
        /// </summary>
        public static List<Block> MobileBlocks { get; private set; }

        private static void SetupMobileBlocks()
        {
            RoutesBlock = new Block
            {
                Id = BlockConstants.RoutesBlockId,
                Color = "green",
                Name = "Routes",
                HideFromNavigation = true,
                IconUrl = "img/routes.png",
                HoverIconUrl = "img/routesColor.png"
            };

            //RoutesBlock is hidden for now
            MobileBlocks = new List<Block> { RoutesBlock };
        }

        #endregion

        #region Regular

        /// <summary>
        /// Gets the clients block.
        /// </summary>
        public static Block ClientsBlock { get; private set; }

        /// <summary>
        /// Gets the dispatcher block.
        /// </summary>
        public static Block DispatcherBlock { get; private set; }

        /// <summary>
        /// Gets the employees block.
        /// </summary>
        public static Block EmployeesBlock { get; private set; }

        /// <summary>
        /// Gets the uservoice block.
        /// </summary>
        public static Block FeebackSupportBlock { get; private set; }

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
        /// The list of regular blocks.
        /// </summary>
        public static List<Block> RegularBlocks { get; set; }

        private static void SetupRegularBlocks()
        {
            ClientsBlock = new Block
            {
                Id = BlockConstants.ClientBlockId,
                Name = "Clients",
                Color = "blue",
                IconUrl = "img/clients.png",
                HoverIconUrl = "img/clientsColor.png",
                IsSilverlight = true
            };

            DispatcherBlock = new Block
            {
                Id = BlockConstants.DispatcherBlockId,
                Name = "Dispatcher",
                Color = "green",
                IconUrl = "img/dispatcher.png",
                HoverIconUrl = "img/dispatcherColor.png",
                IsSilverlight = true
            };

            EmployeesBlock = new Block
            {
                Id = BlockConstants.EmployeesBlockId,
                Name = "Employees",
                Color = "red",
                IconUrl = "img/employees.png",
                HoverIconUrl = "img/employeesColor.png",
                IsSilverlight = true
            };

            FeebackSupportBlock = new Block
            {
                Id = BlockConstants.FeedbackSupportBlockId,
                Name = "Feedback and Support",
                Color = "blue",
                IconUrl = "img/uservoice.png",
                HoverIconUrl = "img/uservoiceColor.png"
            };

            ImportDataBlock = new Block
            {
                Id = BlockConstants.ImportDataBlockId,
                Name = "Import Data",
                Color = "black",
                HideFromNavigation = true,
                IsSilverlight = true
            };

            LocationsBlock = new Block
            {
                Id = BlockConstants.LocationsBlockId,
                Name = "Locations",
                Color = "orange",
                IconUrl = "img/locations.png",
                HoverIconUrl = "img/locationsColor.png",
                IsSilverlight = true
            };

            RegionsBlock = new Block
            {
                Id = BlockConstants.RegionsBlockId,
                Name = "Regions",
                Color = "orange",
                IconUrl = "img/regions.png",
                HoverIconUrl = "img/regionsColor.png",
                IsSilverlight = true
            };

            ServicesBlock = new Block
            {
                Id = BlockConstants.ServicesBlockId,
                Name = "Services",
                Color = "green",
                IconUrl = "img/services.png",
                HoverIconUrl = "img/servicesColor.png",
                IsSilverlight = true
            };

            VehiclesBlock = new Block
            {
                Id = BlockConstants.VehiclesBlockId,
                Name = "Vehicles",
                Color = "red",
                IconUrl = "img/vehicles.png",
                HoverIconUrl = "img/vehiclesColor.png",
                IsSilverlight = true
            };

            VehicleMaintenanceBlock = new Block
            {
                Id = BlockConstants.VehicleMaintenanceBlockId,
                Name = "Vehicle Maintenance",
                Color = "red",
                HideFromNavigation = true,
                IsSilverlight = true
            };

            RegularBlocks = new List<Block>
            {
               ClientsBlock,
               DispatcherBlock,
               EmployeesBlock,
               FeebackSupportBlock,
               ImportDataBlock, 
               LocationsBlock,
               RegionsBlock,
               ServicesBlock,
               EmployeesBlock,
               VehiclesBlock,
               VehicleMaintenanceBlock
            };
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
            BusinessAccountsBlock = new Block
            {
                Id = BlockConstants.BusinessAccountsBlockId,
                Name = "Business Accounts",
                Color = "black",
                IconUrl = "img/businessAccounts.png",
                HoverIconUrl = "img/businessAccountsColor.png",
                IsSilverlight = true
            };

            ServiceTemplatesBlock = new Block
            {
                Id = BlockConstants.ServiceTemplatesBlockId,
                Name = "Service Templates",
                Color = "green",
                HideFromNavigation = true,
                IsSilverlight = true
            };

            AdministrativeConsoleBlocks = new List<Block> { BusinessAccountsBlock };
        }

        #endregion

        private static void SetupAllBlocks()
        {
            AllBlocks = new List<Block>();

            AllBlocks.AddRange(MobileBlocks);
            AllBlocks.AddRange(RegularBlocks);
            AllBlocks.AddRange(AdministrativeConsoleBlocks);
        }
    }
}