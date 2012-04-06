using FoundOps.Core.Models.CoreEntities;
using GalaSoft.MvvmLight.Command;

namespace FoundOps.SLClient.UI.ViewModels
{
    public interface IAddDeleteSelectedClient
    {

        /// <summary>
        /// Add the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Client> AddSelectedClientCommand { get; set; }

        /// <summary>
        /// Delete the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Client> DeleteSelectedClientCommand { get; set; }
    }

    public interface IAddDeleteSelectedEmployee
    {

        /// <summary>
        /// Add the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Employee> AddSelectedEmployeeCommand { get; set; }

        /// <summary>
        /// Delete the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Employee> DeleteSelectedEmployeeCommand { get; set; }
    }

    public interface IAddDeleteSelectedLocation
    {
        /// <summary>
        /// Add the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Location> AddSelectedLocationCommand { get; set; }

        /// <summary>
        /// Delete the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Location> DeleteSelectedLocationCommand { get; set; }
    }

    public interface IAddDeleteSelectedRegion
    {
        /// <summary>
        /// Add the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Region> AddSelectedRegionCommand { get; set; }

        /// <summary>
        /// Delete the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Region> DeleteSelectedRegionCommand { get; set; }
    }

    public interface IAddDeleteSelectedVehicle
    {
        /// <summary>
        /// Add the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Vehicle> AddSelectedRegionCommand { get; set; }

        /// <summary>
        /// Delete the Selected CommandParameter (of Type T)
        /// </summary>
        RelayCommand<Vehicle> DeleteSelectedRegionCommand { get; set; }
    }
}
