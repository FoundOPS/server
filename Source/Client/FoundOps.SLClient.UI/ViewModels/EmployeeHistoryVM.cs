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
    public class EmployeeHistoryVM : InfiniteAccordionVM<EmployeeHistoryEntry>
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
            SetupContextDataLoading(roleId => Context.GetEmployeeHistoryEntriesForRoleQuery(roleId),
                                    new[]
                                        {
                                            new ContextRelationshipFilter
                                                {
                                                    EntityMember = "EmployeeId",
                                                    FilterValueGenerator = v => ((Employee) v).Id,
                                                    RelatedContextType = typeof (Employee)
                                                }
                                        });
        }

        #region Logic

        protected override void OnAddEntity(EmployeeHistoryEntry newEntity)
        {
            newEntity.Date = DateTime.Now.Date;
            newEntity.Employee = ContextManager.GetContext<Employee>();

            //Jump to the proper context, if not already
            if (!IsInDetailsView)
                MoveToDetailsView.Execute(null);
        }

        #endregion
    }
}
