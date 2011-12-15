using ReactiveUI;
using System.Reactive.Linq;
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

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedEmployeePersonVM;
        /// <summary>
        /// Gets the selected Employee's PartyVM
        /// </summary>
        public PartyVM SelectedEmployeePersonVM { get { return _selectedEmployeePersonVM.Value; } }


        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="partyDataService">The party data service.</param>
        [ImportingConstructor]
        public EmployeesVM(DataManager dataManager, IPartyDataService partyDataService)
            : base(dataManager)
        {
            //Setup the selected Employee's OwnedPerson PartyVM whenever the selected employee changes
            _selectedEmployeePersonVM =
                SelectedEntityObservable.Where(se => se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson, this.DataManager))
                .ToProperty(this, x => x.SelectedEmployeePersonVM);

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

        #endregion
    }
}