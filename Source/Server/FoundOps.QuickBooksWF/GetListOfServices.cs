using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{

    public sealed class GetListOfServices : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<Client> CurrentClient { get; set; }

        // Define an activity output argument of a list of RouteTasks
        public OutArgument<IEnumerable<Service>> ListOfServices { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentClient = CurrentClient.Get<Client>(context);

            //Gets a list of RouteTasks from the given BusinessAccount where the RouteTasks date is yesterday's date
            //Also, it filters out the RouteTasks that were not generated off of a ServiceTemplate
            //Only RouteTasks that have a ServiceTemplate will have the information necessary to create an invoice
            var services = currentClient.ServicesToRecieve.Where(
                s => s.ServiceDate == DateTime.Now.AddDays(-1).Date).ToArray();

            ListOfServices.Set(context, services);
        }
    }
}