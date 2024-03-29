﻿using System.Activities;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.QuickBooksWF
{

    public sealed class GenerateInvoiceFromRouteTask : CodeActivity
    {
        // Define an activity input argument of type BunsinessAccount
        public InArgument<BusinessAccount> CurrentBusinessAccount { get; set; }

        // Define an activity input argument of type RouteTask
        public InArgument<RouteTask> CurrentRouteTask { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            var currentBusinessAccount = CurrentBusinessAccount.Get<BusinessAccount>(context);

            var currentRouteTask = CurrentBusinessAccount.Get<RouteTask>(context);

            var invoice = new Invoice();
        }
    }
}
