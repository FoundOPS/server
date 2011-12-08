using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.CoreEntities.M2M4Ria;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// A view model for all of the SelectedEmployees in Routes
    /// </summary>
    [ExportViewModel("SelectedEmployeesVM")]
    public class SelectedEmployeesVM : CoreSelectedEntitiesCollectionVM<Employee>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedEmployeesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="selectedEmployees">The selected employees.</param>
        [ImportingConstructor]
        public SelectedEmployeesVM(DataManager dataManager, IEntityCollection<Employee> selectedEmployees)
            : base(dataManager, selectedEmployees, dataManager.GetEntityListObservable<Employee>(DataManager.Query.Employees))
        {
            //Default this to active
            this.ControlsThatCurrentlyRequireThisVM.Add("This viewmodel always needs data");

            //SetupMainQuery for Employees
            this.SetupMainQuery(DataManager.Query.Employees);
        }

        // Logic

        protected override void OnAddEntity(Employee newEmployee)
        {
            //Setup the OwnedPerson
            newEmployee.OwnedPerson = new Person { FirstName = "First", LastName = "Last" };

            //Add the Employee to the BusinessAccount's Employees
            ((BusinessAccount)ContextManager.OwnerAccount).Employees.Add(newEmployee);

            //Add the newEmployee to the SelectedEntities
            this.SelectedEntities.Add(newEmployee);
        }
    }
}
