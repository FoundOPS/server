using System.Activities;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{
    public sealed class GetListOfObjectsFromTables : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        public CoreEntitiesContainer CoreEntitiesContainer;

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            //Gets the current BusinessAccount from the Workflow
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);

            //Calls the method that will return the list of objects
            var listOfObjects = QuickBooksTools.GetListFromTables(currentBusinessAccount);

            foreach (var invoiceObject in listOfObjects)
            {
                //Queries the database for an invoice with the same Id as the one stored in 
                var invoiceExists = coreEntitiesContainer.Invoices.Where(i => invoiceObject.InvoiceId == i.Id);

                var invoice = invoiceExists.FirstOrDefault();

                //If no invoice exists with the specified Id number, do nothing and return
                if (invoice == null) return;

                //If the delete operation was specified, delete the invoice from QBO
                //If the update operation was specified, update the invoice in QBO
                switch (invoiceObject.ChangeType)
                {
                    #region Delete - Currently Not Supported
                    //case Operation.Delete:
                    //    QuickBooksTools.DeleteInvoice(currentBusinessAccount, invoice);
                    //    break;
                    #endregion
                    case Operation.Update:
                        QuickBooksTools.UpdateInvoice(currentBusinessAccount, invoice, coreEntitiesContainer);
                        break;
                }

                //If for some reason a create has been added. Leave it there and it will be handled at night with the rest of the create \
                //Else, remove the invoice from the table
                if (invoiceObject.ChangeType != Operation.Create)
                    QuickBooksTools.RemoveFromTable(invoice);
            }
        }
    }
}