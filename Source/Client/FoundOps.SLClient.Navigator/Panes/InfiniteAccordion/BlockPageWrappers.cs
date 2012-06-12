using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.Blocks;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;

namespace FoundOps.SLClient.Navigator.Panes.InfiniteAccordion
{
    //This contains the page wrappers for each of the blocks.
    //A page wrapper is required if you want a block to control navigation.

#pragma warning disable 1591 //Ignore comment warninng. One comment is enough.
    [ExportPage("BusinessAccounts")]
    public class BusinessAccountsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public BusinessAccountsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(BusinessAccount))
        {
        }
    }

    [ExportPage("Clients")]
    public class ClientsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ClientsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Client))
        {
        }
    }

    [ExportPage("Employees")]
    public class EmployeesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public EmployeesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Employee))
        {
        }
    }

    [ExportPage("EmployeeHistory")]
    public class EmployeeHistoryMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public EmployeeHistoryMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Employee))
        {
        }
    }

    [ExportPage("Locations")]
    public class LocationsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public LocationsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Location))
        {
        }
    }

    [ExportPage("Regions")]
    public class RegionsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public RegionsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Region))
        {
        }
    }

    [ExportPage("Services")]
    public class ServicesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ServicesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(ServiceHolder))
        {
        }
    }

    [ExportPage("ServiceTemplates")]
    public class ServiceTemplatesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ServiceTemplatesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(ServiceTemplate))
        {
        }
    }

    [ExportPage("RecurringService")]
    public class RecurringServiceMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public RecurringServiceMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(RecurringService))
        {
        }
    }

    [ExportPage("Vehicles")]
    public class VehiclesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public VehiclesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Vehicle))
        {
        }
    }

    [ExportPage("VehicleMaintenance")]
    public class VehicleMaintenanceMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public VehicleMaintenanceMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(VehicleMaintenanceLogEntry))
        {
        }
    }
#pragma warning restore 1591
}
