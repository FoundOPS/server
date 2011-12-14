using System;
using System.IO;
using System.ServiceModel;
using System.Xaml;
using System.Activities.Tracking;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using System.ServiceModel.Activities.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoundOps.QuickBooksWF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CallWorkflow();
            
            //var workflowRuntime = new System.Workflow.Runtime.WorkflowRuntime();
            //create the workflow instance and start it

            //WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof (FoundOps.QuickBooksWF.GetData));

            //var proxy = new ServiceReference1.GetHereClient();
            //proxy.Open();
            //proxy.GetHere();

            //proxy.Close();

            //Works, but only to call individual CodeActivities
            //var objects = WorkflowInvoker.Invoke(new CreateInvoice());

            //serviceClient.Open();

            //var gdr= new GetDataRequest();

            //proxy.Close();
        }

        private void CallWorkflow()
        {
            const string baseAddress = "http://localhost:37069/QuickbooksWF";

            var serviceImplementation = XamlServices.Load(@"C:\FoundOps\GitHub\Source\Server\FoundOps.QuickBooksWF\Create.xamlx") as WorkflowService;

            if(serviceImplementation == null)
            {
                throw new NullReferenceException(String.Format("Unable to load service definition"));
            }

            using (var host = new WorkflowServiceHost(serviceImplementation, new Uri(baseAddress)))
            {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });

                host.AddServiceEndpoint("ServiceDefinition", new BasicHttpBinding(), baseAddress);

                var fileTrackingProfile = new TrackingProfile();
                fileTrackingProfile.Queries.Add(new WorkflowInstanceQuery
                                                    {
                                                        States = { "*" }
                                                    });
                fileTrackingProfile.Queries.Add(new ActivityStateQuery()
                                                    {
                                                        ActivityName = "*",
                                                        States = { "*" },
                                                        // You can use the following to specify specific stages:
                                                        // States = {
                                                        // ActivityStates.Executing,
                                                        // ActivityStates.Closed
                                                        //},
                                                        Variables =
                                                            {
                                                                {"*"}
                                                            } // or you can enter specific variable names instead of “*”

                                                    });
                fileTrackingProfile.Queries.Add(new CustomTrackingQuery()
                                                    {
                                                        ActivityName = "*",
                                                        Name = "*"
                                                    });


                host.Description.Behaviors.Add(new WorkflowIdleBehavior()
                                                   {
                                                       TimeToPersist = TimeSpan.FromSeconds(5),
                                                       TimeToUnload = TimeSpan.FromSeconds(20)
                                                   });


                host.Open();
            }
        }

        public WorkflowService ToWorkflowService (string value)
{
    WorkflowService service = null;

    // 1. assume value is Xaml
    string xaml = value;

    // 2. if value is file path,
    if (File.Exists (value))
    {
        // 2a. read contents to xaml
        xaml = File.ReadAllText (value);
    }

    // 3. build service
    using (var xamlReader = new StringReader (xaml))
    {
        object untypedService = null;

        // NOTE: XamlServices, NOT ActivityXamlServices
        untypedService = XamlServices.Load (xamlReader);

        if (untypedService is WorkflowService)
        {
            service = (WorkflowService)(untypedService);
        }
        else
        {
            throw new ArgumentException (
                string.Format (
                "Unexpected error reading WorkflowService from " + 
                "value [{0}] and Xaml [{1}]. Xaml does not define a " + 
                "WorkflowService, but an instance of [{2}].", 
                value, 
                xaml, 
                untypedService.GetType ()));
        }
    }

    return service;
}

    }
}

