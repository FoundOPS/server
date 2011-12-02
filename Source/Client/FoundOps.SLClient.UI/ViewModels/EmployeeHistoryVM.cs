using System;
using System.ComponentModel;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Context.Services;
using System.ComponentModel.Composition;
using FoundOps.SLClient.Data.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying EmployeeHistory
    /// </summary>
    [ExportViewModel("EmployeeHistoryVM")]
    public class EmployeeHistoryVM : CoreEntityCollectionInfiniteAccordionVM<EmployeeHistoryEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeHistoryVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        [ImportingConstructor]
        public EmployeeHistoryVM(DataManager dataManager)
            : base(dataManager)
        {
            this.SetupMainQuery(DataManager.Query.EmployeeHistory);
        }

        //Logic

        protected override bool EntityIsPartOfView(EmployeeHistoryEntry entity, bool isNew)
        {
            if (isNew)
                return true;

            var employeeContext = ContextManager.GetContext<Employee>();

            if (employeeContext != null && entity.EmployeeId == employeeContext.Id)
                return true;

            return false;
        }

        protected override void OnAddEntity(EmployeeHistoryEntry newEntity)
        {
            newEntity.Date = DateTime.Now.Date;
            newEntity.Employee = ContextManager.GetContext<Employee>();

            //Jump to the proper context, if not already
            if (!IsInDetailsView)
                MoveToDetailsView.Execute(null);
        }
    }
}
