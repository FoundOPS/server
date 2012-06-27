using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{

    public sealed class GetListOfClientsWithServices : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        // Define an activity output argument of a list of RouteTasks
        public OutArgument<IEnumerable<Client>> ListOfClientsWithTasks { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);

            //Returns a list of clients that have a scheduled RouteTasks from one day prior to today
            var clientsWithTasks = currentBusinessAccount.Clients.Where(c => c.ServicesToRecieve.FirstOrDefault(s => s.ServiceDate == DateTime.Now.AddDays(-1).Date) != null);

            ListOfClientsWithTasks.Set(context, clientsWithTasks);
        }
    }
}
