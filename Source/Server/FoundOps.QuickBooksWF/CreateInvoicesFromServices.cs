using System;
using System.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Core.Models.QuickBooks;

namespace FoundOps.QuickBooksWF
{

    public sealed class CreateInvoicesFromServices : CodeActivity
    {
        // Define an activity input argument of type RouteTask
        public InArgument<IEnumerable<Service>> CurrentServices { get; set; }

        // Define an activity input argument of type RouteTask
        public InArgument<Client> CurrentClient { get; set; }

        // Define an activity output argument of an Invoice
        public OutArgument<IEnumerable<Invoice>> Invoices { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            //Gets the current client
            var currentClient = CurrentClient.Get<Client>(context);

            //Gets the current list of RouteTasks 
            var currentServices = CurrentServices.Get<IEnumerable<Service>>(context);

            //Derrives a list of distinct ServiceTemplates based on the collection of RouteTasks we have
            var distinctServiceTemplates = currentServices.Select(s => s.ServiceTemplate.Name).Distinct();

            //List of new Invoices that need to be created
            var newInvoices = new List<Invoice>();

            foreach (var serviceTemplate in distinctServiceTemplates)
            {
                //Gets a list of all the RouteTasks that have the current ServiceTemplate
                var servicesForCurrentInvoice = currentServices.Where(s => s.ServiceTemplate.Name == serviceTemplate).ToArray();

                //Since all tasks have the same ServiceTemplate, we can just get the first one and use all of its information
                var firstService = servicesForCurrentInvoice.FirstOrDefault();

                //Resharper made me add this in. FirstsTasks will never be null in this situation, but Resharper has no knowledge of previous CodeActivities
                if (firstService == null) continue;

                //var newInvoice = firstTask.Service.ServiceTemplate.Invoice;

                //Creates a new Invoice based on the current Tasks
                //TODO: Replace this with info from ServiceTemplates
                var newInvoice = new Invoice
                  {
                      BillToLocation = currentClient.DefaultBillingLocation,
                      BusinessAccount = firstService.ServiceProvider,
                      Client = currentClient,
                      Memo = "test"
                  };

                //TODO: Add line items in based on the services rendered
                #region Adding fake LineItems to the new Invoice
                
                foreach (var service in servicesForCurrentInvoice)
                {
                    var amount = new Random().Next(250).ToString(CultureInfo.InvariantCulture);

                    var lineItem = new LineItem { Amount = amount, Description = service.ServiceTemplate.Name };
                    newInvoice.LineItems.Add(lineItem);
                }

                #endregion

                //Adds the Invoice created above to the list of Invoices that will be returned from this Activity
                newInvoices.Add(newInvoice);

                //Adds the create notification to the Azure Table
                QuickBooksTools.AddUpdateDeleteToTable(newInvoice, Operation.Create);
            }
            
            //Saves the new Invoices
            Invoices.Set(context, newInvoices);
        }
    }
}
