using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Employees
    /// </summary>
    [ExportViewModel("EmployeesVM")]
    public class EmployeesVM : InfiniteAccordionVM<Employee, Employee>, IAddToDeleteFromSource<Employee>
    {
        #region Public Properties

        #region Implementation of IAddToDeleteFromSource

        public string MemberPath { get { return "DisplayName"; } }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        public Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; private set; }

        /// <summary>
        /// A function to create a new item from a string.
        /// </summary>
        public Func<string, Employee> CreateNewItem { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public EmployeesVM()
        {
            SetupDataLoading();

            #region Implementation of IAddToDeleteFromSource<Employee>

            CreateNewItem = name =>
            {
                var newEmployee = new Employee { OwnedPerson = new Person { DisplayName = name } };

                //Add the new entity to the OwnerAccount
                ((BusinessAccount)ContextManager.OwnerAccount).Employees.Add(newEmployee);

                //Add the new entity to the EntitySet so it is tracked by the DomainContext
                DomainContext.Employees.Add(newEmployee);

                return newEmployee;
            };

            ManuallyUpdateSuggestions = autoCompleteBox =>
              SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.DomainContext.SearchEmployeesForRoleQuery(Manager.Context.RoleId, autoCompleteBox.SearchText));

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => DomainContext.GetEmployeesForRoleQuery(roleId));

            SetupDetailsLoading(selectedEntity => DomainContext.GetEmployeeDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
        }

        #endregion

        #region Logic

        protected override Employee AddNewEntity(object commandParameter)
        {
            //Reuse the CreateNewItem method
            return CreateNewItem("");
        }

        #endregion
    }
}