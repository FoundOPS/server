using System.Collections.Generic;
using System.Linq;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{
    public sealed class GetListOfBusinessAccounts : CodeActivity
    {
        // Define an activity output argument of a list of BusinessAccounts
        public OutArgument<IEnumerable<BusinessAccount>> ListOfBusinessAccounts { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            ListOfBusinessAccounts.Set(context, coreEntitiesContainer.Parties.OfType<BusinessAccount>().ToArray());
        }
    }
}
