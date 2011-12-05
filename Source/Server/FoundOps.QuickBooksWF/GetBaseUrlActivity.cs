using System.Activities;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{
    public sealed class GetBaseUrlActivity : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        public CoreEntitiesContainer CoreEntitiesContainer;

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            CoreEntitiesContainer = new CoreEntitiesContainer();

            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);
            QuickBooksTools.GetBaseUrl(currentBusinessAccount, CoreEntitiesContainer);
        }
    }
}
