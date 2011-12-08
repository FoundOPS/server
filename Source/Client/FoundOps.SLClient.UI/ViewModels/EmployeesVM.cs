using System.ComponentModel;
using MEFedMVVM.ViewModelLocator;
using System.ComponentModel.Composition;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Employees
    /// </summary>
    [ExportViewModel("EmployeesVM")]
    public class EmployeesVM : CoreEntityCollectionInfiniteAccordionVM<Employee>
    {
        //Public Properties
        private PersonVM _selectedEmployeePersonVM;
        /// <summary>
        /// Gets the selected employee person VM.
        /// </summary>
        public PersonVM SelectedEmployeePersonVM
        {
            get { return _selectedEmployeePersonVM; }
            private set
            {
                if (SelectedEmployeePersonVM != null)
                    SelectedEmployeePersonVM.PropertyChanged -= SelectedEmployeePersonVMPropertyChanged; _selectedEmployeePersonVM = value;
                if (SelectedEmployeePersonVM != null)
                    SelectedEmployeePersonVM.PropertyChanged += SelectedEmployeePersonVMPropertyChanged;
                this.RaisePropertyChanged("SelectedEmployeePersonVM");
            }
        }

        void SelectedEmployeePersonVMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPerson")
            {
                if (SelectedEmployeePersonVM.SelectedPerson == null)
                {
                    SelectedEntity = null;
                    return;
                }

                if (SelectedEntity != SelectedEmployeePersonVM.SelectedPerson.OwnerEmployee)
                {
                    SelectedEntity = SelectedEmployeePersonVM.SelectedPerson.OwnerEmployee;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="partyDataService">The party data service.</param>
        [ImportingConstructor]
        public EmployeesVM(DataManager dataManager, IPartyDataService partyDataService)
            : base(dataManager)
        {
            SelectedEmployeePersonVM = new PersonVM(dataManager, partyDataService);

            //Load the employees
            SetupMainQuery(DataManager.Query.Employees, null, "DisplayName");
        }

        #region Logic

        protected override bool EntityIsPartOfView(Employee entity, bool isNew)
        {
            if (isNew)
                return true;

            var entityIsPartOfView = true;

            //Setup filters

            var businessAccountContext = ContextManager.GetContext<BusinessAccount>();

            if (businessAccountContext != null)
                entityIsPartOfView = entity.EmployerId == businessAccountContext.Id;

            return entityIsPartOfView;
        }

        protected override void OnAddEntity(Employee newEmployee)
        {
            newEmployee.OwnedPerson = new Person();
            ((BusinessAccount)ContextManager.OwnerAccount).Employees.Add(newEmployee);
        }

        protected override void OnSelectedEntityChanged(Employee oldValue, Employee newValue)
        {
            base.OnSelectedEntityChanged(oldValue, newValue);

            SelectedEmployeePersonVM.SelectedPerson = newValue != null ? newValue.OwnedPerson : null;
        }

        #endregion
    }
}