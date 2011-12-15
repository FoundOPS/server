using System;
using System.Linq;
using System.Windows.Markup;
using FoundOps.SLClient.UI.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.Tools
{
    /// <summary>
    /// Access point to the view models.
    /// </summary>
    public static class VM
    {
        /// <summary>
        /// Gets the LocationsVM.
        /// </summary>
        public static LocationsVM Locations
        {
            get
            {
                return (LocationsVM) ViewModelRepository.Instance.Resolver.GetViewModelByContract("LocationsVM", null, CreationPolicy.Shared).Value;
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