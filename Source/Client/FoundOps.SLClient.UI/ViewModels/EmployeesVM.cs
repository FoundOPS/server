using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;

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

        public IEnumerable ExistingItemsSource { get { return Context.Employees; } }

        public string MemberPath { get { return "DisplayName"; } }

        public Action<string, AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

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
            SetupDataLoading();

            //Setup the selected Employee's OwnedPerson PartyVM whenever the selected employee changes
            _selectedEmployeePersonVM =
                SelectedEntityObservable.Where(se => se != null && se.OwnedPerson != null).Select(se => new PartyVM(se.OwnedPerson))
                .ToProperty(this, x => x.SelectedEmployeePersonVM);

            #region Implementation of IAddToDeleteFromSource<Employee>

            CreateNewItem = name =>
            {
                var newEmployee = new Employee { OwnedPerson = new Person { DisplayName = name } };

                //Add the new entity to the OwnerAccount
                ((BusinessAccount)ContextManager.OwnerAccount).Employees.Add(newEmployee);

                //Add the new entity to the EntityList behind the DCV
                ((EntitySet<Employee>)ExistingItemsSource).Add(newEmployee);

                return newEmployee;
            };

            #endregion
        }

        /// <summary>
        /// Setups the data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => Context.GetEmployeesForRoleQuery(roleId));

            LoadOperation<Employee> detailsLoadOperation = null;
            //Whenever the location changes load the employee details
            SelectedEntityObservable.Where(se => se != null).Subscribe(selectedEntity =>
            {
                if (detailsLoadOperation != null && detailsLoadOperation.CanCancel)
                    detailsLoadOperation.Cancel();

                selectedEntity.DetailsLoading = true;
                detailsLoadOperation = Context.Load(Context.GetEmployeeDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id),
                                 loadOp => selectedEntity.DetailsLoading = false, null);
            });
        }


        #region Logic

        protected override Employee AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}