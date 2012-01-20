using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{
    //Temporary class to deal with ReadyToInvoice not being set by user at this time
    public sealed class SetReadyToInvoice : CodeActivity
    {
        // Define an activity input argument of type RouteTask
        public InArgument<IEnumerable<Service>> CurrentServices { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            //gets the current route task
            var currentServices = CurrentServices.Get<IEnumerable<Service>>(context);

            //sets the routetasks property ReadyToInvoice to true
            foreach (var routeTask in currentServices.SelectMany(service => service.RouteTasks))
            {
                routeTask.ReadyToInvoice = true;
            }

            //Save
            coreEntitiesContainer.SaveChanges();
        }
    }
}
