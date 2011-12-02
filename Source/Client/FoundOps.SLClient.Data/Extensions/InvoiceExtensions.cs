using System.Linq;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

// Partial classes must be in the same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Invoice
    {
        private readonly string[] listOfProperties = new string[] { "Amount", "Description", "Memo", "BillToLocation", "QuickBooksId" };

        private bool _trackingChanges;

        public EntityGraph EntityGraph
        {
            get
            {
                var graphShape =
                    new EntityGraphShape().Edge<Invoice, LineItem>(invoice => invoice.LineItems)
                    .Edge<Invoice, SalesTerm>(invoice => invoice.SalesTerm);

                return new EntityGraph(this, graphShape);
            }
        }

        partial void OnCreation()
        {
            TrackChanges();
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
            {
                TrackChanges();
            }
        }

        /// <summary>
        /// Track this Entity's Changes
        /// </summary>
        public void TrackChanges()
        {
            if (_trackingChanges)
                return;

            _trackingChanges = true;

            //Track changes on this service
            this.EntityGraph.PropertyChanged += (sender, e) =>
            {
                if (!listOfProperties.Contains(e.PropertyName))
                    return;

                #region Invoice Properties

                //The Invoice itself has changed
                if (sender is Invoice)
                {
                    if (e.PropertyName == "Memo")
                        IsMemoChanged = true;

                    if (e.PropertyName == "BillToLocation")
                        IsBillToLocationChanged = true;

                    if (e.PropertyName == "DueDate")
                        IsDueDateChanged = true;
                }

                #endregion

                #region SalesTerm Properties

                //The Invoice's SalesTerm has changed
                if (sender is SalesTerm)
                {
                    if (e.PropertyName == "QuickBooksId")
                        IsSalesTermChanged = true;
                }

                #endregion

                #region LineItems Properties

                //One of this Invoice's LineItems has changed
                if (sender is LineItem)
                {
                    if (e.PropertyName == "IsAmountChanged")
                        ((LineItem)sender).IsAmountChanged = true;

                    if (e.PropertyName == "IsDescriptionChanged")
                        ((LineItem)sender).IsDescriptionChanged = true;
                }

                #endregion

            };
        }
    }
}
