using System;
using ReactiveUI;
using System.Collections;
using System.Reactive.Linq;
using System.Collections.Generic;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using System.ComponentModel.Composition;
using Microsoft.Windows.Data.DomainServices;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Employees
    /// </summary>
    [ExportViewModel("EmployeesVM")]
    public class EmployeesVM : InfiniteAccordionVM<Employee>, IAddToDeleteFromSource<Employee>
    {
        #region Public Properties

        #region Implementation of IAddToDeleteFromSource

        //Want to use the default comparer. So this does not need to be set.
        public IEqualityComparer<object> CustomComparer { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable> _loadedEmployees;
        public IEnumerable ExistingItemsSource { get { return _loadedEmployees.Value; } }

        public string MemberPath { get { return "DisplayName"; } }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Employee> CreateNewItem { get; private set; }

        #endregion

        private readonly ObservableAsPropertyHelper<PartyVM> _selectedEmployeePersonVM;
        /// <summary>
        /// Gets the selected Employee's PartyVM
        /// </summary>
        public PartyVM SelectedEmployeePersonVM { get { return _selectedEmployeePersonVM.Value; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public EmployeesVM()
        {
            //Setup the selected Employee's OwnedPerson PartyVM whenever the selected employee changes
            _selectedEmployeePersonVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson))
                .ToProperty(this, x => x.SelectedEmployeePersonVM);

            //Load the employees
            var loadedEmployees = SetupMainQuery(DataManager.Query.Employees, null, "DisplayName");

            #region Implementation of IAddToDeleteFromSource<Employee>

            //Whenever loadedEmployees changes notify ExistingItemsSource changed
            _loadedEmployees = loadedEmployees.ToProperty(this, x => x.ExistingItemsSource);

            CreateNewItem = name =>
            {
                var newEmployee = new Employee { OwnedPerson = new Person { DisplayName = name } };

                //Add the new entity to the OwnerAccount
                ((BusinessAccount)ContextManager.OwnerAccount).Employees.Add(newEmployee);

                //Add the new entity to the EntityList behind the DCV
                ((EntityList<Employee>)ExistingItemsSource).Add(newEmployee);

                return newEmployee;
            };

            #endregion
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

        protected override Employee AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}