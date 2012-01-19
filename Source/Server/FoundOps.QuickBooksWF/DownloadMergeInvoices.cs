using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class DownloadMergeInvoices : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        // Define an activity output argument of a list of RouteTasks
        public InArgument<String> BaseUrl { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);

            var baseUrl = BaseUrl.Get<string>(context);

            //Gets the BaseUrl
            var xmlInvoiceList = QuickBooksTools.GetEntityList(currentBusinessAccount, "invoices", baseUrl);

            //Creates an invoice from the XML response
            //Checks for an invoice with the same QuickBooksId in the database
            //A)If one exists, Merge
            //B)Add a new invoice to the database
            QuickBooksTools.CreateInvoicesFromQuickBooksResponse(currentBusinessAccount, baseUrl, xmlInvoiceList);
        }
    }
}
