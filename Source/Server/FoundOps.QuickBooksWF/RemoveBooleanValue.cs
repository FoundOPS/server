using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{
    //Temporary class to deal with ReadyToInvoice not being set by user at this time
    public sealed class RemoveBooleanValue : CodeActivity
    {
        // Define an activity input argument of type RouteTask
        public InArgument<RouteTask> CurrentRouteTask { get; set; }

        // Define an activity input argument of type RouteTask
        public OutArgument<bool> BooleanHolder { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var coreEntitiesContainer = new CoreEntitiesContainer();

            //gets the current route task
            var currentRouteTask = CurrentRouteTask.Get<RouteTask>(context);

            //Temporarily required check. Condition should always return false, ReadyToInvoice should be set to false when the route task is generated
            if (currentRouteTask.ReadyToInvoice == null)
                currentRouteTask.ReadyToInvoice = false;

            //Save
            coreEntitiesContainer.SaveChanges();

            BooleanHolder.Set(context, false);
        }
    }
}
