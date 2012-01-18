using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class CreateInvoiceQuickBooks : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }
        public InArgument<Invoice> CurrentInvoice { get; set; }
        public InArgument<string> BaseUrl { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);
            var currentInvoice = CurrentInvoice.Get<Invoice>(context);
            var baseUrl = BaseUrl.Get<string>(context);

            var response = QuickBooksTools.CreateNewInvoice(currentBusinessAccount, currentInvoice, baseUrl);

            //Checks for an Error Code in the response XML
            //If it does not exist in the response XML, signals that a problem occurred and we do not remove it from the table
            if ((response.Contains("<errcode>0</errcode>")))
            {
                //At this point all has gone to plan, remove it from the Azure Table
                QuickBooksTools.RemoveFromTable(currentInvoice);
            }
        }
    }
}
