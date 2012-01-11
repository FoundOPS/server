using System;
using System.Linq;
using System.Windows.Markup;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.UI.ViewModels;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.Tools
{
    /// <summary>
    /// Access point to the view models.
    /// </summary>
    public static class VM
    {
        /// <summary>
        /// Gets the BusinessAccountsVM.
        /// </summary>
        public static BusinessAccountsVM BusinessAccounts
        {
            get
            {
                return (BusinessAccountsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("BusinessAccountsVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the ClientsVM.
        /// </summary>
        public static ClientsVM Clients
        {
            get
            {
                return (ClientsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ClientsVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the ClientTitlesVM.
        /// </summary>
        public static ClientTitlesVM ClientTitles
        {
            get
            {
                return (ClientTitlesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ClientTitlesVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the EmployeesVM.
        /// </summary>
        public static EmployeesVM Employees
        {
            get
            {
                return (EmployeesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("EmployeesVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the FieldsVM.
        /// </summary>
        public static FieldsVM Fields
        {
            get
            {
                return (FieldsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("FieldsVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the LocationsVM.
        /// </summary>
        public static LocationsVM Locations
        {
            get
            {
                return (LocationsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("LocationsVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the RegionsVM.
        /// </summary>
        public static RegionsVM Regions
        {
            get
            {
                return (RegionsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RegionsVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the RoutesVM.
        /// </summary>
        public static RoutesVM Routes
        {
            get
            {
                return (RoutesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RoutesVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the RoutesDragDropVM.
        /// </summary>
        public static RoutesDragDropVM RoutesDragDrop
        {
            get
            {
                return (RoutesDragDropVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("RoutesDragDropVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the RouteManifestVM.
        /// </summary>
        public static RouteManifestVM RouteManifest { get { return Routes.RouteManifestVM; } }

        /// <summary>
        /// Gets the ServiceTemplatesVM.
        /// </summary>
        public static ServiceTemplatesVM ServiceTemplates
        {
            get
            {
                return (ServiceTemplatesVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("ServiceTemplatesVM", null, CreationPolicy.Shared).Value;
            }
        }

        /// <summary>
        /// Gets the UserAccountsVM.
        /// </summary>
        public static UserAccountsVM UserAccounts
        {
            get
            {
                return (UserAccountsVM)ViewModelRepository.Instance.Resolver.GetViewModelByContract("UserAccountsVM", null, CreationPolicy.Shared).Value;
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