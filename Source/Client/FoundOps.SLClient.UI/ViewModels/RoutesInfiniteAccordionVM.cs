using System;
using System.Linq;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using MEFedMVVM.ViewModelLocator;
using FoundOps.SLClient.Data.Services;
using FoundOps.SLClient.Data.ViewModels;
using System.ComponentModel.Composition;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    [ExportViewModel("RoutesInfiniteAccordionVM")]
    public class RoutesInfiniteAccordionVM : InfiniteAccordionVM<Route>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesInfiniteAccordionVM"/> class.
        /// </summary>
        [ImportingConstructor]
        public RoutesInfiniteAccordionVM()
        {
            SetupMainQuery(DataManager.Query.RouteLog, null, "Date");


            //Whenever the Employee or Vehicle Conntext changes:
            //a) update the filter
            //b) select the closest Route to today
            this.ContextManager.GetContextObservable<Employee>().AsGeneric()
                .Merge(this.ContextManager.GetContextObservable<Vehicle>().AsGeneric())
                .ObserveOnDispatcher().Subscribe(_ =>
            {
                if (EditableCollectionView == null)
                    return;
                //b) select the closest Route to today
                SelectedEntity = Collection.FirstOrDefault(r => r.Date >= DateTime.Now.Date);
            });
        }

        #region Logic

        protected override bool EntityIsPartOfView(Route route, bool isNew)
        {
            var entityIsPartOfView = true;

            var employeeContext = this.ContextManager.GetContext<Employee>();

            if (employeeContext != null)
                entityIsPartOfView = route.Technicians.Contains(employeeContext);

            var vehicleContext = this.ContextManager.GetContext<Vehicle>();

            if (vehicleContext != null)
                entityIsPartOfView = entityIsPartOfView && route.Vehicles.Contains(vehicleContext);

            return entityIsPartOfView;
        }

        protected override void OnAddEntity(Route newEntity)
        {
            throw new Exception("Not supposed to do this here.");
        }
        protected override void OnDeleteEntity(Route newEntity)
        {
            throw new Exception("Not supposed to do this here.");
        }

        protected override bool BeforeSaveCommand()
        {
            return true;
        }

        #endregion
    }
}
