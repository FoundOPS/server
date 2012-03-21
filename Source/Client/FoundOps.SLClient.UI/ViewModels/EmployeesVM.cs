﻿using System.ServiceModel.DomainServices.Client;
using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
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

        public string MemberPath { get { return "DisplayName"; } }

        /// <summary>
        /// A method to update the ExistingItemsSource with suggestions remotely loaded.
        /// </summary>
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

        #region Constructor

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

                //Add the new entity to the EntitySet so it is tracked by the DomainContext
                Context.Employees.Add(newEmployee);

                return newEmployee;
            };

            ManuallyUpdateSuggestions = (searchText, autoCompleteBox) =>
              SearchSuggestionsHelper(autoCompleteBox, () => Manager.Data.Context.SearchEmployeesForRoleQuery(Manager.Context.RoleId, searchText));

            #endregion
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupTopEntityDataLoading(roleId => Context.GetEmployeesForRoleQuery(roleId));

            SetupDetailsLoading(selectedEntity => Context.GetEmployeeDetailsForRoleQuery(ContextManager.RoleId, selectedEntity.Id));
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