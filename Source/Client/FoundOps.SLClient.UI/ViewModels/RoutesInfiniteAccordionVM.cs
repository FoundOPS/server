using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel.Composition;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Displays the proper routes depending on the context.
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
            ////b) select the closest Route to today
            //SelectedEntity = Collection.FirstOrDefault(r => r.Date >= DateTime.Now.Date);

            //SetupContextDataLoading();
        }

        #region Logic

        protected override void OnAddEntity(Route newEntity)
        {
            throw new Exception("Not supposed to add routes in the RouteLog.");
        }

        protected override void OnDeleteEntity(Route newEntity)
        {
            throw new Exception("Not supposed to delete routes in the RouteLog.");
        }

        #endregion
    }
}
