using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class GetInvoiceFromRouteTask : CodeActivity
    {
        // Define an activity input argument of type RouteTask
        public InArgument<RouteTask> CurrentRouteTask { get; set; }

        // Define an activity output argument of an Invoice
        public OutArgument<Invoice> Invoice { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentRouteTask = CurrentRouteTask.Get<RouteTask>(context);

            var newInvoice = currentRouteTask.Service.ServiceTemplate.Invoice;

            if (newInvoice == null)
            {
                newInvoice = new Invoice
                                 {
                                     BillToLocation = currentRouteTask.Client.DefaultBillingLocation,
                                     BusinessAccount = currentRouteTask.OwnerBusinessAccount,
                                     Client = currentRouteTask.Service.Client,
                                     Memo = "test"
                                 };

                var lineItem = new LineItem { Amount = "100", Description = "TESTING" };

                newInvoice.LineItems.Add(lineItem);

                lineItem = new LineItem { Amount = "256", Description = "1234567890" };

                newInvoice.LineItems.Add(lineItem);
            }

            //Adds the create notification to the Azure Table
            QuickBooksTools.AddUpdateDeleteToTable(newInvoice, Operation.Create);

            Invoice.Set(context, newInvoice);
        }
    }
}
