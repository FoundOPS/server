using FoundOps.SLClient.Navigator;
using FoundOps.SLClient.UI.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Markup;

namespace FoundOps.SLClient.UI.Tools
{
    /// <summary>
    /// Access point to the view models.
    /// </summary>
    public static class VM
    {
        public static BusinessAccountsVM BusinessAccounts
        {
            get
            {
                return (BusinessAccountsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("BusinessAccountsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static ClientsVM Clients
        {
            get
            {
                return (ClientsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ClientsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static EmployeesVM Employees
        {
            get
            {
                return (EmployeesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("EmployeesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static FieldsVM Fields
        {
            get
            {
                return (FieldsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("FieldsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static DispatcherFilterVM DispatcherFilter
        {
            get
            {
                return (DispatcherFilterVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("DispatcherFilterVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static ImportDataVM ImportData
        {
            get
            {
                return (ImportDataVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ImportDataVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static LocationsVM Locations
        {
            get
            {
                return (LocationsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("LocationsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static RegionsVM Regions
        {
            get
            {
                return (RegionsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RegionsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static RecurringServicesVM RecurringServices
        {
            get
            {
                return (RecurringServicesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RecurringServicesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static RoutesVM Routes
        {
            get
            {
                return (RoutesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RoutesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static RoutesInfiniteAccordionVM RoutesInfiniteAccordion
        {
            get
            {
                return (RoutesInfiniteAccordionVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RoutesInfiniteAccordionVM", null, CreationPolicy.Shared).Value;
            }
        }

        private static RouteManifestVM _routeManifestVM;
        public static RouteManifestVM RouteManifest
        {
            get
            {
                return _routeManifestVM ??
                       (_routeManifestVM = (RouteManifestVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RouteManifestVM", null, CreationPolicy.Shared).Value);
            }
        }

        public static ServiceTemplatesVM ServiceTemplates
        {
            get
            {
                return (ServiceTemplatesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ServiceTemplatesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static ServicesVM Services
        {
            get
            {
                return (ServicesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ServicesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static TaskBoardVM TaskBoard
        {
            get
            {
                return (TaskBoardVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("TaskBoardVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static UserAccountsVM UserAccounts
        {
            get
            {
                return (UserAccountsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("UserAccountsVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static VehiclesVM Vehicles
        {
            get
            {
                return (VehiclesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("VehiclesVM", null, CreationPolicy.Shared).Value;
            }
        }

        public static NavigationVM Navigation
        {
            get
            {
                return (NavigationVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("NavigationVM", null, CreationPolicy.Shared).Value;
            }
        }
    }

    /// <summary>
    /// Gets the VM.
    /// </summary>
    public class GetVM : MarkupExtension
    {
        /// <summary>
        /// The type of VM to retrieve.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var viewModelName = Type.ToString().Split('.').Last();
            return ViewModelRepository.Instance.Resolver.GetViewModelByContract(viewModelName, null, CreationPolicy.Shared).Value;
        }
    }
}