using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.Navigator.Panes.InfiniteAccordion
{
    //This contains the page wrappers for each of the blocks.
    //A page wrapper is required if you want a block to control navigation.

#pragma warning disable 1591 //Ignore comment warninng. One comment is enough.
    public class BusinessAccountsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public BusinessAccountsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(BusinessAccount))
        {
        }
    }

    public class ClientsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ClientsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Client))
        {
        }
    }

    public class EmployeesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public EmployeesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Employee))
        {
        }
    }

    public class EmployeeHistoryMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public EmployeeHistoryMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Employee))
        {
        }
    }

    public class LocationsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public LocationsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Location))
        {
        }
    }
    
    public class RegionsMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public RegionsMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Region))
        {
        }
    }

    public class ServicesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ServicesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(ServiceHolder))
        {
        }
    }

    public class ServiceTemplatesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public ServiceTemplatesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(ServiceTemplate))
        {
        }
    }

    public class RecurringServiceMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public RecurringServiceMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(RecurringService))
        {
        }
    }

    public class VehiclesMainPage : InfiniteAccordionPageWrapper
    {
        [ImportingConstructor]
        public VehiclesMainPage(Blocks managerBlocks)
            : base(managerBlocks, typeof(Vehicle))
        {
        }
    }

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
