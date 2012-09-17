using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System;
using System.ComponentModel.Composition;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying EmployeeHistory
    /// </summary>
    [ExportViewModel("EmployeeHistoryVM")]
    public class EmployeeHistoryVM : InfiniteAccordionVM<EmployeeHistoryEntry, EmployeeHistoryEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeHistoryVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public EmployeeHistoryVM()
            : base(new[] { typeof(Employee) })
        {
            SetupDataLoading();
        }

        /// <summary>
        /// Used in the constructor to setup data loading.
        /// </summary>
        private void SetupDataLoading()
        {
            SetupContextDataLoading(roleId => DomainContext.GetEmployeeHistoryEntriesForRoleQuery(roleId), 
                new[] { new ContextRelationshipFilter("EmployeeId", typeof(Employee), v => ((Employee)v).Id) });
        }

        #region Logic

        protected override void OnAddEntity(EmployeeHistoryEntry newEntity)
        {
            newEntity.Date = Manager.Context.UserAccount.Now().Date;
            newEntity.Employee = ContextManager.GetContext<Employee>();

            //Jump to the proper context, if not already
            if (!IsInDetailsView)
                MoveToDetailsView.Execute(null);
        }

        #endregion
    }
}
