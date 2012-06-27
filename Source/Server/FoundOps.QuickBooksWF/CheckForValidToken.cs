using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class CheckForValidToken : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        public OutArgument<bool> ValidLogin { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);

            //Checks the validity of the token saved on the currentBusinessAccount
            //Also, makes sure that the account has QuickBooksEnabled
            var validLogin = QuickBooksTools.CheckValidityOfToken(currentBusinessAccount);

            //Sets the OutArgument
            ValidLogin.Set(context, validLogin);

        }
    }
}
