using System.Activities;
using System.Collections.Generic;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class SendInvoicesToQuickBooks : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }
        public InArgument<IEnumerable<Invoice>> CurrentInvoices { get; set; }
        public InArgument<string> BaseUrl { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);
            var currentInvoices = CurrentInvoices.Get<IEnumerable<Invoice>>(context);
            var baseUrl = BaseUrl.Get<string>(context);

            //Iterates through the invoices. Sends a request to create an invoice for each. 
            //Then it checks if the response is an empty string. If it isn't, the Invoice is removed from the Tables
            foreach (Invoice invoice in currentInvoices)
            {
                var response = QuickBooksTools.CreateNewInvoice(currentBusinessAccount, invoice, baseUrl);
                if (response != "") 
                    QuickBooksTools.RemoveFromTable(invoice);
            }
        }
    }
}
