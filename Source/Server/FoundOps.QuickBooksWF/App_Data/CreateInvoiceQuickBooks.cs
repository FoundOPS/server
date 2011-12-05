using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class CreateInvoiceQuickBooks : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }
        public InArgument<BusinessAccount> CurrentInvoice { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);
            var currentInvoice = CurrentInvoice.Get<Invoice>(context);

            QuickBooksTools.CreateNewInvoice(currentBusinessAccount, currentInvoice);
        }
    }
}
